using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int atk;
    [SerializeField] int health;
    [SerializeField] Animator anim;

    public int Atk
    {
        get => atk;
        set
        {
            atk = value;
            atkText.text = $"ATK {atk}";
        }
    }
    public int Health
    {
        get => health;
        set
        {
            health = value;
            healthText.text = $"HP {health}";
        }
    }

    [Header("UI")]
    [SerializeField] TMP_Text atkText;
    [SerializeField] TMP_Text healthText;

    public async UniTask SpawnAsync()
    {
        this.gameObject.SetActive(true);
        anim.SetTrigger("Spawn");

        await UniTask.WaitForSeconds(0.3f);
    }

    public async UniTask AttackAnimAsync()
    {
        anim.SetTrigger("Attack1");

        // 타격까지 걸리는 시간
        await UniTask.WaitForSeconds(3.67f);
    }

    public async UniTask TakeDamageAnimAsync(int damage)
    {
        anim.SetTrigger("TakeDamage");
        await UniTask.WaitForSeconds(1.5f);
    }

    public async UniTask DieAnimAsync()
    {
        anim.SetTrigger("Die");
        await UniTask.WaitForSeconds(1f);
    }
}
