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
        playerStamina.OnStaminaChanged += UpdateBar;
    }

    private void OnDisable()
    {
        playerStamina.OnStaminaChanged -= UpdateBar;
    }

    private void Start()
    {
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