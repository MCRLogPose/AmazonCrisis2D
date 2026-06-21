public abstract class AnimalState
{
    protected AnimalBrain brain;

    public AnimalState(AnimalBrain brain)
    {
        this.brain = brain;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }
}