using UnityEngine;

public class PlayerMissionVision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Vision")]
    [SerializeField] private float viewDistance = 100f;
    [SerializeField] private LayerMask missionPointLayer;

    [Header("Detection")]
    [SerializeField] private float lookTimeRequired = 0.5f;

    private MissionPoint currentTarget;
    private float lookTimer;

    private void Update()
    {
        CheckVision();
    }

    private void CheckVision()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            viewDistance,
            missionPointLayer,
            QueryTriggerInteraction.Ignore))
        {
            MissionPoint missionPoint =
                hit.collider.GetComponentInParent<MissionPoint>();

            if (missionPoint != null &&
                !missionPoint.MissionCreated)
            {
                if (currentTarget != missionPoint)
                {
                    currentTarget = missionPoint;
                    lookTimer = 0f;
                }

                lookTimer += Time.deltaTime;

                if (lookTimer >= lookTimeRequired)
                {
                    missionPoint.CreateMission();

                    currentTarget = null;
                    lookTimer = 0f;
                }

                return;
            }
        }

        currentTarget = null;
        lookTimer = 0f;
    }
}