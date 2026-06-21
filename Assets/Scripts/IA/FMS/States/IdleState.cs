public class IdleState : AnimalState
{
    public IdleState(AnimalBrain brain)
        : base(brain)
    {
    }

    public override void Enter()
    {
        brain.movement.Stop();

        brain.animator.SetBool(
            "isWalking",
            false
        );
    }
}