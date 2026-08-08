using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private WeaponBase[] weapons;

    private WeaponBase currentWeapon;
    private int currentWeaponIndex;

    private void Start()
    {
        if (weapons.Length == 0)
            return;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Unequip();
        }

        EquipWeapon(0);
    }

    private void Update()
    {
        HandleWeaponSwitching();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (currentWeapon == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.TryAttack();
        }
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeapon(2);
        }
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
            return;

        if (currentWeapon != null)
        {
            currentWeapon.Unequip();
        }

        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];

        currentWeapon.Equip();
    }
}