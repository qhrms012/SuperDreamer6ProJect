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


    [Header("HpUI")]
    [SerializeField] private Image playerHp;
    [SerializeField] private Image enemyHp;

    [Header("FinishUI")]
    [SerializeField] private TextMeshProUGUI finishText;

    [Header("PlayerBtn")]
    [SerializeField] private HoldButton aButton;   // 왼쪽 버튼
    [SerializeField] private HoldButton dButton;   // 오른쪽 버튼
    [SerializeField] private bool useUIInput = true; // UI로만 조작할 때 켜기
    [SerializeField] private Image playerCheckImage;


    void Awake()
    {
        foreach (var s in slots)
        {

            s.skill = s.skillBehaviour as ISkill;
            s.button.onClick.RemoveAllListeners();
            s.button.onClick.AddListener(() => OnClickSkill(s));


            UpdateSlotUI(s);
            UpdatePlayer();
        }
    }

    private void OnEnable()
    {
        Player.onHpChanged += UpdatePlayerHpUI;
        Player.isDead += UpdateEnemyFinishUI;
        Enemy.onHpChanged += UpdateEnemyHpUI;
        Enemy.isDead += UpdatePlayerFinishUI;
    }

    private void OnDisable()
    {
        Player.onHpChanged -= UpdatePlayerHpUI;
        Player.isDead -= UpdateEnemyFinishUI;
        Enemy.onHpChanged -= UpdateEnemyHpUI;
        Enemy.isDead -= UpdatePlayerFinishUI;
    }
    void Update()
    {            
        foreach (var s in slots)
        {
            if (s.skill == null) continue;
            UpdateSlotUI(s);
        }

        if (!useUIInput) return;

        float x = 0f;
        if (aButton && aButton.IsHeld) x -= 1f;
        if (dButton && dButton.IsHeld) x += 1f;

        GameManager.Instance.player.SetUiMove(new Vector2(x, 0f));
    }

    private void OnClickSkill(SkillSlot s)
    {
        if (s.skill == null) return;
        if (s.skill.GetRemainingCooldown() > 0f) return;

        bool casted = s.skill.TryCast();
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        if (!casted && s.button)    StartCoroutine(Pulse(s.button.transform)); 
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

    public void UpdatePlayer()
    {
        if (useUIInput)
        {
            useUIInput = false;
            playerCheckImage.gameObject.SetActive(false);
        }
        else
        {
            useUIInput = true;
            playerCheckImage.gameObject.SetActive(true);
        }

    }
    private void UpdatePlayerHpUI(float curHp)
    {
        playerHp.fillAmount = curHp / GameManager.Instance.player.maxHp;
    }

    private void UpdateEnemyHpUI(float curHp)
    {
        enemyHp.fillAmount = curHp / GameManager.Instance.enemy.maxHp;
    }

    private void UpdateEnemyFinishUI(bool finish) => finishText.text = "Lose";
    private void UpdatePlayerFinishUI(bool finish) => finishText.text = "Victory";
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

