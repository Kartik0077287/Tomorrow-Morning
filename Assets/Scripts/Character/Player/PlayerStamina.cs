using UnityEngine;
using System;

public class PlayerStamina : MonoBehaviour
{
    public event Action<float, float> OnStaminaChanged;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenRate = 20f;
    [SerializeField] private float regenDelay = 1.5f;

    [Header("Drain")]
    [SerializeField] private float sprintDrain = 15f;

    private float currentStamina;
    private float regenTimer;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    [Header("Limits")]
    [SerializeField] private float minimumSprintStamina = 5f;

    public bool HasStamina => currentStamina >= minimumSprintStamina;

    private void Awake()
    {
        currentStamina = maxStamina;
        NotifyStaminaChanged();
    }

    private void Update()
    {
        if (regenTimer > 0)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        if (currentStamina >= maxStamina)
            return;

        Regenerate();
    }

    private void Regenerate()
    {
        float previous = currentStamina;
        currentStamina = Mathf.Clamp(currentStamina + regenRate * Time.deltaTime, 0f, maxStamina);

        if (!Mathf.Approximately(previous, currentStamina))
            NotifyStaminaChanged();
    }

    public void DrainSprint()
    {
        Drain(sprintDrain * Time.deltaTime);
    }

    private void Drain(float amount)
    {
        float previous = currentStamina;
        currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
        regenTimer = regenDelay;

        if (!Mathf.Approximately(previous, currentStamina))
            NotifyStaminaChanged();
    }

    private void NotifyStaminaChanged()
    {
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public void RestoreFull()
    {
        currentStamina = maxStamina;
        NotifyStaminaChanged();
    }

    public void Restore(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);
        NotifyStaminaChanged();
    }
}
