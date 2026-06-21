using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimals : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpForce = 5f;
    public float detectionDistance = 3f;

    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private int direction = 1; // 1 derecha, -1 izquierda
    private float changeTime;

    public Transform player;

    [Header("Animal Data")]
    public string animalType = "Monkey";
    public DiseaseData currentDisease;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Doctor_Mori").transform;

        ChangeDirection();
    }

    void Update()
    {
        CheckGround();
        DetectPlayer();
        Move();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundLayer);
    }

    void DetectPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectionDistance)
        {
            // huir en dirección opuesta al jugador
            direction = (transform.position.x < player.position.x) ? -1 : 1;
        }
    }

    void Move()
    {
        float speed = (Vector2.Distance(transform.position, player.position) < detectionDistance) ? runSpeed : walkSpeed;

        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        // Girar sprite
        if (direction == 1)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        animator.SetBool("isWalking", isGrounded && Mathf.Abs(rb.velocity.x) > 0.1f);
        animator.SetBool("isJumping", !isGrounded);

        // Detectar pared para saltar
        RaycastHit2D wall = Physics2D.Raycast(wallCheck.position, Vector2.right * direction, 0.2f, groundLayer);

        if (wall && isGrounded)
        {
            Jump();
        }

        // Cambiar dirección aleatoria cada cierto tiempo
        changeTime += Time.deltaTime;
        if (changeTime > 3f)
        {
            ChangeDirection();
            changeTime = 0;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void ChangeDirection()
    {
        direction = Random.Range(0, 2) == 0 ? -1 : 1;
    }
}
