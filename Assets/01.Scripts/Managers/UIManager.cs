using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Serializable]
    public class SkillSlot
    {
        public string displayName;
        public Button button;
        public Image cooldownFill;            
        public TextMeshProUGUI cooldownText;  
        public MonoBehaviour skillBehaviour;  
        [HideInInspector] public ISkill skill;
    }

    [SerializeField] private List<SkillSlot> slots;
    [SerializeField] private bool blockRaycastWhenCooling = true;

    void Awake()
    {
        foreach (var s in slots)
        {

            s.skill = s.skillBehaviour as ISkill;
            s.button.onClick.RemoveAllListeners();
            s.button.onClick.AddListener(() => OnClickSkill(s));


            UpdateSlotUI(s);
        }
    }

    void Update()
    {
        foreach (var s in slots)
        {
            if (s.skill == null) continue;
            UpdateSlotUI(s);
        }
    }

    private void OnClickSkill(SkillSlot s)
    {
        if (s.skill == null) return;
        if (s.skill.GetRemainingCooldown() > 0f) return;

        bool casted = s.skill.TryCast();
        if (!casted && s.button) StartCoroutine(Pulse(s.button.transform)); // 타겟 없음 등 실패 피드백
    }

    private void UpdateSlotUI(SkillSlot s)
    {
        float remain = s.skill.GetRemainingCooldown();
        float cd = Mathf.Max(0.0001f, s.skill.Cooldown);
        float ratio = Mathf.Clamp01(remain / cd);

        if (s.cooldownFill)
        {
            s.cooldownFill.fillAmount = ratio;
            var cg = s.cooldownFill.GetComponent<CanvasGroup>();
            if (cg && blockRaycastWhenCooling) cg.blocksRaycasts = remain > 0f;
        }

        if (s.cooldownText)
            s.cooldownText.text = (remain > 0.05f) ? Mathf.CeilToInt(remain).ToString() : "";

        if (s.button)
            s.button.interactable = remain <= 0f;
    }

    private IEnumerator Pulse(Transform t)
    {
        Vector3 baseScale = t.localScale;
        float end = Time.time + 0.15f;
        while (Time.time < end)
        {
            float p = Mathf.InverseLerp(end - 0.15f, end, Time.time);
            float s = 1f + 0.1f * Mathf.Sin(p * Mathf.PI);
            t.localScale = baseScale * s;
            yield return null;
        }
        t.localScale = baseScale;
    }
}

