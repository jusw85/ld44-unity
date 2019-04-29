using UnityEngine;
using UnityEngine.UI;

public class HpUpdate : MonoBehaviour
{
    public GameObject obj;

    private Text text;
    private IHasHP hasHp;

    private void Awake()
    {
        text = GetComponent<Text>();
        hasHp = obj.GetComponent<IHasHP>();
    }

    private void Update()
    {
        text.text = hasHp.GetHP().ToString();
    }
}