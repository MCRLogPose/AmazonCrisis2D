using UnityEngine;

public class AnimalSensor : MonoBehaviour
{
    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask groundLayer;

    public float detectionDistance = 3f;

    private Transform player;

    private void Start()
    {
        player =
            GameObject
            .FindGameObjectWithTag("Doctor_Mori")
            .transform;
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            0.2f,
            groundLayer
        );
    }

    public bool IsWallAhead(int direction)
    {
        return Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * direction,
            0.2f,
            groundLayer
        );
    }

    public bool IsPlayerNear()
    {
        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

        return distance < detectionDistance;
    }

    public Transform GetPlayer()
    {
        return player;
    }
}