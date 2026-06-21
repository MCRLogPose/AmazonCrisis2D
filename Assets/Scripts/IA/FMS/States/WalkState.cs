using UnityEngine;

public class WalkState : AnimalState
{
    private float timer;

    public WalkState(AnimalBrain brain)
        : base(brain)
    {
    }

    public override void Enter()
    {
        timer = 0;

        brain.animator.SetBool(
            "isWalking",
            true
        );
    }

    public override void Update()
    {
        brain.movement.Move(brain.movement.walkSpeed);

        timer += Time.deltaTime;

        if (timer > 3f)
        {
            int direction =
                Random.Range(0, 2) == 0
                ? -1
                : 1;

            brain.movement
                .SetDirection(direction);

            timer = 0;
        }

        if (
            brain.sensor.IsWallAhead(
                brain.movement.GetDirection()
            )
            &&
            brain.sensor.IsGrounded()
        )
        {
            brain.movement.Jump();
        }

        if (brain.sensor.IsPlayerNear())
        {
            brain.ChangeState(
                AnimalStateType.RunAway
            );
        }
    }

    public override void Exit()
    {
        brain.animator.SetBool(
            "isWalking",
            false
        );
    }
}