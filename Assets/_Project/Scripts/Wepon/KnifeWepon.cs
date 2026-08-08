using UnityEngine;

public class KnifeWeapon : WeaponBase
{
    [Header("Knife")]

    [SerializeField] private Transform attackPoint;

    [SerializeField] private float attackRadius = 1.5f;

    [SerializeField] private LayerMask damageLayer;

    protected override void Attack()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                attackPoint.position,
                attackRadius,
                damageLayer);

        foreach (Collider hit in hits)
        {
            IDamageable damageable =
                hit.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        Debug.Log("Knife Attack");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }
}