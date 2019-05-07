using UnityEngine;
using UnityEngine.UI;

public class SkillBarController : MonoBehaviour
{
    public GameObject skillPrefab;
    private int skillIdx;
    public int skillLeftOffset;
    public int skillSpacing;

    public void CreateSkill(string name, string key, Sprite sprite)
    {
        var skill = Instantiate(skillPrefab, transform);
        skill.name = "Skill" + skillIdx;
        skill.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(skillLeftOffset + (skillIdx * skillSpacing), 0f);
        skill.GetComponent<Image>().sprite = sprite;
        skill.transform.GetChild(1).GetComponent<Text>().text = name + "(" + key + ")";
        skillIdx++;
    }

    public void Highlight()
    {
    }
}