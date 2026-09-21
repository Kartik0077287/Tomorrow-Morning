using UnityEngine;

// Climbing is temporarily disabled. Keep this component so existing scene references remain valid.
[RequireComponent(typeof(Rigidbody))]
public class PlayerClimbing : MonoBehaviour
{
    public bool IsClimbing => false;
    public bool IsMantling => false;
}
