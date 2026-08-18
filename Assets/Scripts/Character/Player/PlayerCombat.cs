using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerCombat : MonoBehaviour
{
    private enum EquippedWeapon
    {
        Knife,
        Gun
    }

    [Header("Weapon")]
    [SerializeField] private EquippedWeapon equippedWeapon = EquippedWeapon.Knife;
    [SerializeField] private GameObject knifeObject;
    [SerializeField] private GameObject gunObject;
    [SerializeField] private CameraCollision cameraCollision;
    [SerializeField] private GameObject crosshair;

    [Header("Knife Combo")]
    [SerializeField] private float comboResetTime = 0.8f;
    [SerializeField] private float minimumAttackInterval = 0.15f;

    [Header("Knife Damage")]
    [SerializeField] private float knifeDamage = 35f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRadius = 0.75f;
    [SerializeField] private LayerMask zombieLayers = ~0;

    [Header("Gun")]
    [SerializeField] private float fireInterval = 0.15f;

    private PlayerAnimation playerAnimation;
    private bool nextAttackIsInward;
    private float lastAttackTime = float.NegativeInfinity;

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Start()
    {
        SetAiming(false);
        UpdateWeaponVisuals();
    }

    private void Update()
    {
        HandleWeaponSwitch();

        if (Time.time - lastAttackTime > comboResetTime)
            nextAttackIsInward = false;

        if (equippedWeapon == EquippedWeapon.Gun)
        {
            bool aiming = Input.GetMouseButton(0);
            SetAiming(aiming);

            if (aiming && Time.time - lastAttackTime >= fireInterval)
                FireGun();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            TryKnifeAttack();
        }
    }

    private void HandleWeaponSwitch()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f && equippedWeapon == EquippedWeapon.Knife)
        {
            equippedWeapon = EquippedWeapon.Gun;
            UpdateWeaponVisuals();
        }
        else if (scroll < 0f && equippedWeapon == EquippedWeapon.Gun)
        {
            equippedWeapon = EquippedWeapon.Knife;
            nextAttackIsInward = false;
            SetAiming(false);
            UpdateWeaponVisuals();
        }
    }

    private void UpdateWeaponVisuals()
    {
        if (knifeObject != null)
            knifeObject.SetActive(equippedWeapon == EquippedWeapon.Knife);

        if (gunObject != null)
            gunObject.SetActive(equippedWeapon == EquippedWeapon.Gun);
    }

    private void FireGun()
    {
        playerAnimation.TriggerShoot();
        lastAttackTime = Time.time;
    }

    private void TryKnifeAttack()
    {
        if (Time.time - lastAttackTime < minimumAttackInterval)
            return;

        if (nextAttackIsInward)
            playerAnimation.TriggerKnifeInward();
        else
            playerAnimation.TriggerKnifeOutward();

        DamageZombie();
        nextAttackIsInward = !nextAttackIsInward;
        lastAttackTime = Time.time;
    }

    private void SetAiming(bool aiming)
    {
        if (cameraCollision != null)
            cameraCollision.SetAiming(aiming);

        if (crosshair != null && crosshair.activeSelf != aiming)
            crosshair.SetActive(aiming);
    }

    private void DamageZombie()
    {
        Vector3 hitCenter = transform.position + transform.forward * attackRange;
        Collider[] hits = Physics.OverlapSphere(
            hitCenter,
            attackRadius,
            zombieLayers,
            QueryTriggerInteraction.Ignore);

        ZombieHealth closestZombie = null;
        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            ZombieHealth zombieHealth = hit.GetComponentInParent<ZombieHealth>();
            if (zombieHealth == null || zombieHealth.IsDead)
                continue;

            float distance = (zombieHealth.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestZombie = zombieHealth;
            }
        }

        if (closestZombie != null)
            closestZombie.TakeDamage(knifeDamage);
    }
}
