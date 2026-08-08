public abstract class ZombieState
{
    protected ZombieStateMachine zombie;

    protected ZombieState(ZombieStateMachine zombie)
    {
        this.zombie = zombie;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}