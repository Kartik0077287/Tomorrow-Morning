using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] protected string weaponName;

    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float attackCooldown = 0.5f;

    protected float nextAttackTime;

    public string WeaponName => weaponName;

    public virtual void Equip()
    {
        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        gameObject.SetActive(false);
    }

    public bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    public void TryAttack()
    {
        if (!CanAttack())
            return;

        Attack();

        nextAttackTime = Time.time + attackCooldown;
    }

    protected abstract void Attack();
}