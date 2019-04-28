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
        public int skillCost;

        [NonSerialized]
        public float currentCooldown;

        [NonSerialized]
        public KeyCode keyCode;
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
            if (Input.GetKeyDown(s.keyCode) && s.cooldownImage.fillAmount <= 0 && player.GetNumSlaves() >= s.skillCost)
            {
                s.cooldownImage.fillAmount = 1.0f;
                s.currentCooldown = s.cooldown;
                isButtonPressed = true;
                player.HandleSkill(s.key, s.skillCost);
            }
        }

        if (isButtonPressed)
        {
            foreach (SkillButton s in skills)
            {
                if (player.GetNumSlaves() < s.skillCost)
                {
                    s.cooldownImage.fillAmount = 1.0f;
                    continue;
                }

                if (s.cooldownImage.fillAmount <= 0)
                {
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

        foreach (SkillButton s in skills)
        {
            if (s.cooldownImage.fillAmount > 0 && player.GetNumSlaves() >= s.skillCost)
            {
                s.cooldownImage.fillAmount -= Time.deltaTime / s.currentCooldown;
                s.cooldownImage.fillAmount = Mathf.Clamp(s.cooldownImage.fillAmount, 0f, 1f);
            }
        }
    }
}