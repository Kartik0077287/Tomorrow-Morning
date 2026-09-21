using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Image fillImage;
    [SerializeField] private float smoothSpeed = 8f;

    private float targetFill;

    private void OnEnable()
    {
        if (playerStamina != null)
            playerStamina.OnStaminaChanged += UpdateBar;
    }

    private void OnDisable()
    {
        if (playerStamina != null)
            playerStamina.OnStaminaChanged -= UpdateBar;
    }

    private void Start()
    {
        if (playerStamina == null)
        {
            Debug.LogWarning("PlayerStamina reference is not assigned on PlayerStaminaUI.");
            enabled = false;
            return;
        }

        if (fillImage == null)
        {
            Debug.LogWarning("Fill Image reference is not assigned on PlayerStaminaUI.");
            enabled = false;
            return;
        }

        // Ensure the Image is set to Filled so fillAmount has effect
        fillImage.type = Image.Type.Filled;

        targetFill = playerStamina.CurrentStamina / playerStamina.MaxStamina;
        fillImage.fillAmount = targetFill;
    }

    private void Update()
    {
        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            targetFill,
            smoothSpeed * Time.deltaTime);
    }

    private void UpdateBar(float current, float max)
    {
        targetFill = current / max;
    }
}