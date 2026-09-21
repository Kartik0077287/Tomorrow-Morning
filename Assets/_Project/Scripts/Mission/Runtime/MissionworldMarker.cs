using UnityEngine;

public class MissionWorldMarker : MonoBehaviour
{
    [Header("Beam")]
    [SerializeField] private float beamHeight = 25f;
    [SerializeField] private float beamWidth = 0.35f;

    [Header("Color")]
    [SerializeField]
    private Color beamColor =
        new Color(1f, 0.65f, 0.05f, 0.8f);

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;

        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth * 0.15f;

        lineRenderer.startColor = beamColor;
        lineRenderer.endColor = beamColor;

        lineRenderer.enabled = false;
    }

    public void ShowMarker(Vector3 position)
    {
        transform.position = position;

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.up * beamHeight);

        lineRenderer.enabled = true;
    }
}