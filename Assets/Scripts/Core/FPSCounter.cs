using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private int fontSize = 24;
    [SerializeField] private Vector2 position = new Vector2(15f, 15f);

    [Header("Smoothing")]
    [SerializeField] private float updateInterval = 0.25f;

    private float fps;
    private float timer;

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= updateInterval)
        {
            fps = 1f / Time.unscaledDeltaTime;
            timer = 0f;
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;

        Rect rect = new Rect(
            position.x,
            position.y,
            300f,
            100f
        );

        GUI.Label(
            rect,
            $"FPS: {fps:F0}\nFrame: {1000f / Mathf.Max(fps, 0.01f):F1} ms",
            style
        );
    }
}