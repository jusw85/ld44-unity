using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillBarController : MonoBehaviour
{
    [Serializable]
    public class SkillButton
    {
        public String key;
        public Image cooldownImage;
        public float cooldown;

        [NonSerialized]
        public float currentCooldown;

        [NonSerialized]
        public KeyCode keyCode;

        [NonSerialized]
        public bool isCoolingDown;
    }

    public float globalCooldown;
    public SkillButton[] skills;
    public PlayerController player;

    private void OnValidate()
    {
        foreach (SkillButton skill in skills)
        {
            skill.cooldown = Math.Max(globalCooldown, skill.cooldown);
        }
    }

    private void Start()
    {
        foreach (SkillButton skill in skills)
        {
            skill.keyCode = (KeyCode) Enum.Parse(typeof(KeyCode), skill.key);
        }
    }

    private void Update()
    {
        bool isButtonPressed = false;
        foreach (SkillButton s in skills)
        {
            HandleSkill(s, ref isButtonPressed);
        }

        if (isButtonPressed)
        {
            foreach (SkillButton s in skills)
            {
                if (!s.isCoolingDown)
                {
                    s.isCoolingDown = true;
                    s.cooldownImage.fillAmount = 1.0f;
                    s.currentCooldown = globalCooldown;
                }
                else
                {
                    var realTimeCooldown = s.cooldownImage.fillAmount * s.currentCooldown;
                    realTimeCooldown = Math.Max(realTimeCooldown, globalCooldown);
                    s.cooldownImage.fillAmount = Math.Min(realTimeCooldown / s.currentCooldown, 1);
                }
            }
        }
    }

    private void HandleSkill(SkillButton s, ref bool isButtonPressed)
    {
        if (Input.GetKeyDown(s.keyCode) && !s.isCoolingDown)
        {
            s.isCoolingDown = true;
            s.cooldownImage.fillAmount = 1.0f;
            s.currentCooldown = s.cooldown;
            isButtonPressed = true;
            player.HandleSkill(s.key);
        }

        if (s.isCoolingDown)
        {
            s.cooldownImage.fillAmount -= Time.deltaTime / s.currentCooldown;
        }

        if (s.cooldownImage.fillAmount <= 0)
        {
            s.cooldownImage.fillAmount = 0;
            s.isCoolingDown = false;
        }
    }
}