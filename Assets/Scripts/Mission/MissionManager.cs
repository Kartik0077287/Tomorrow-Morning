using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("World Marker")]
    [SerializeField] private MissionWorldMarker worldMarker;

    private MissionPoint currentMission;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CreateMission(MissionPoint missionPoint)
    {
        if (missionPoint == null)
            return;

        if (currentMission != null)
            return;

        currentMission = missionPoint;

        Debug.Log("=================================");
        Debug.Log("MISSION CREATED");
        Debug.Log("Mission: " + missionPoint.MissionTitle);
        Debug.Log("Objective: " + missionPoint.Objective);
        Debug.Log("Location: " + missionPoint.transform.position);
        Debug.Log("=================================");

        if (worldMarker != null)
        {
            worldMarker.ShowMarker(missionPoint.transform.position);
        }
    }
}