using UnityEngine;

public class MissionPoint : MonoBehaviour
{
    [Header("Mission")]
    [SerializeField] private string missionTitle = "Investigate the Area";

    [TextArea]
    [SerializeField]
    private string objective =
        "Go to the marked location and investigate.";

    private bool missionCreated;

    public string MissionTitle => missionTitle;
    public string Objective => objective;
    public bool MissionCreated => missionCreated;

    public void CreateMission()
    {
        if (missionCreated)
            return;

        missionCreated = true;

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CreateMission(this);
        }
    }
}