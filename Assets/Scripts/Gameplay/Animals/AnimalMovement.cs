using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;

    private int direction = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float speed)
    {
        rb.velocity =
            new Vector2(
                direction * speed,
                rb.velocity.y
            );
    }

    public void Stop()
    {
        rb.velocity =
            new Vector2(
                0,
                rb.velocity.y
            );
    }

    public void Jump()
    {
        rb.velocity =
            new Vector2(
                rb.velocity.x,
                jumpForce
            );
    }

    public void SetDirection(int newDirection)
    {
        direction = newDirection;

        if (direction == 1)
        {
            transform.localScale =
                new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale =
                new Vector3(-1, 1, 1);
        }
    }

    public int GetDirection()
    {
        return direction;
    }
}