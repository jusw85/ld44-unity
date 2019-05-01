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

//    public float globalCooldown;
    public SkillButton[] skills;
    public PlayerController player;
    public Slider chargeBar;

    [NonSerialized]
    public bool isInputFrozen;

    private SkillButton currentButtonPressed;

//    private void OnValidate()
//    {
//        foreach (SkillButton skill in skills)
//        {
//            skill.cooldown = Math.Max(globalCooldown, skill.cooldown);
//        }
//    }

    private void Awake()
    {
        currentButtonPressed = null;
    }

    private void Start()
    {
        foreach (SkillButton skill in skills)
        {
            skill.keyCode = (KeyCode) Enum.Parse(typeof(KeyCode), skill.key);
        }
    }

    public void FreezeInput(bool isFrozen)
    {
        isInputFrozen = isFrozen;
        if (isFrozen)
        {
            chargeBar.value = 0f;
            currentButtonPressed = null;
        }
    }

    private void Update()
    {
        if (isInputFrozen)
        {
            return;
        }

        SkillButton buttonPress = null;
        foreach (SkillButton s in skills)
        {
            if (Input.GetKey(s.keyCode))
            {
                buttonPress = s;
                break;
            }
        }

        if (currentButtonPressed == null && buttonPress != null && player.isCasting() == false)
        {
            player.SetCharging(true);
            currentButtonPressed = buttonPress;
        }

        if (currentButtonPressed != null)
        {
            if (currentButtonPressed == buttonPress)
            {
                chargeBar.value += Time.deltaTime / currentButtonPressed.cooldown;
                chargeBar.value = Mathf.Clamp(chargeBar.value, 0f, 1f);
                if (chargeBar.value >= 1)
                {
                    player.HandleSkill(currentButtonPressed.key, currentButtonPressed.skillCost);
                    chargeBar.value = 0f;
                    currentButtonPressed = null;
                }
            }
            else
            {
                player.SetCharging(false);
                chargeBar.value = 0f;
                currentButtonPressed = null;
            }
        }
    }

//    private void Update()
//    {
//        bool isButtonPressed = false;
//        if (!isInputFrozen)
//        {
//            foreach (SkillButton s in skills)
//            {
//                if (Input.GetKeyDown(s.keyCode) && s.cooldownImage.fillAmount <= 0 &&
//                    player.GetNumSlaves() >= s.skillCost)
//                {
//                    s.cooldownImage.fillAmount = 1.0f;
//                    s.currentCooldown = s.cooldown;
//                    isButtonPressed = true;
//                    player.HandleSkill(s.key, s.skillCost);
//                }
//            }
//        }
//
//        if (isButtonPressed)
//        {
//            foreach (SkillButton s in skills)
//            {
//                if (player.GetNumSlaves() < s.skillCost)
//                {
//                    s.cooldownImage.fillAmount = 1.0f;
//                    continue;
//                }
//
//                if (s.cooldownImage.fillAmount <= 0)
//                {
//                    s.cooldownImage.fillAmount = 1.0f;
//                    s.currentCooldown = globalCooldown;
//                }
//                else
//                {
//                    var realTimeCooldown = s.cooldownImage.fillAmount * s.currentCooldown;
//                    realTimeCooldown = Math.Max(realTimeCooldown, globalCooldown);
//                    s.cooldownImage.fillAmount = Math.Min(realTimeCooldown / s.currentCooldown, 1);
//                }
//            }
//        }
//
//        foreach (SkillButton s in skills)
//        {
//            if (s.cooldownImage.fillAmount > 0 && player.GetNumSlaves() >= s.skillCost)
//            {
//                s.cooldownImage.fillAmount -= Time.deltaTime / s.currentCooldown;
//                s.cooldownImage.fillAmount = Mathf.Clamp(s.cooldownImage.fillAmount, 0f, 1f);
//            }
//        }
//    }
}