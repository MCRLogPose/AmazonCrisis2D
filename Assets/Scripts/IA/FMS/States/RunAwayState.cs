using UnityEngine;

public class RunAwayState : AnimalState
{
    public RunAwayState(
        AnimalBrain brain
    ) : base(brain)
    {
    }

    public override void Enter()
    {
        brain.animator.SetBool(
            "isWalking",
            true
        );
    }

    public override void Update()
    {
        Transform player =
            brain.sensor.GetPlayer();

        int direction =
            (
                brain.transform.position.x
                <
                player.position.x
            )
            ? -1
            : 1;

        brain.movement.SetDirection(direction);

        brain.movement.Move(brain.movement.runSpeed);

        if (
            !brain.sensor.IsPlayerNear()
        )
        {
            brain.ChangeState(
                AnimalStateType.Walk
            );
        }
    }

    public override void Exit()
    {
    }
}