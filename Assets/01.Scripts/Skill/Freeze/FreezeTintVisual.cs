using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTintVisual : MonoBehaviour
{
    [Header("Tint")]
    [SerializeField] private Color freezeColor = new Color(0.45f, 0.85f, 1f, 1f);
    [SerializeField] private float fadeIn = 0.08f;
    [SerializeField] private float fadeOut = 0.20f;

    private SpriteRenderer[] renderers;
    private Color[] baseColors;
    private Coroutine running;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        baseColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            baseColors[i] = renderers[i].color;
    }

    public void Play(float duration)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(CoTint(duration));
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.IceSound, duration);
    }

    IEnumerator CoTint(float duration)
    {
        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeIn);
            ApplyBlend(a);
            yield return null;
        }
        ApplyBlend(1f);

        float hold = Mathf.Max(0f, duration - fadeIn - fadeOut);
        if (hold > 0f) yield return new WaitForSeconds(hold);

        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / fadeOut);
            ApplyBlend(a);
            yield return null;
        }
        ApplyBlend(0f);
        running = null;
    }

    private void ApplyBlend(float a)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            var baseCol = baseColors[i];
            var target = Color.Lerp(baseCol, freezeColor, a);
            target.a = baseCol.a;
            renderers[i].color = target;
        }
    }
}

