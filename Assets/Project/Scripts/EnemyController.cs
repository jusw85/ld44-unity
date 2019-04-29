using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, IHasHP
{
    public int hp;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
    }

    public int GetHP()
    {
        return hp;
    }
}