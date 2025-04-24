namespace SnakeGame;
using System;

/// <summary>
/// 蛇的运动状态Enum
/// </summary>
public enum StateSnakeAction
{
    Left,
    Up,
    Down,
    Right,
}

public enum StateSnakeStatus
{
    Calm,
    Furious,
    ActivelyFurious,
}

public enum StateSnakeAttack
{
    Attack,
    Idle,
}
public class SnakeState
{
    private FSM<StateSnakeAction> fsm_action;
    private FSM<StateSnakeStatus> fsm_status;

    public SnakeState(SharedData shared_data)
    {
        this.fsm_action = new FSM<StateSnakeAction>();
        this.fsm_status = new FSM<StateSnakeStatus>();
        fsm_action.AddState(StateSnakeAction.Left,new LeftState(shared_data,fsm_action));
        fsm_action.AddState(StateSnakeAction.Up,new UpState(shared_data,fsm_action));
        fsm_action.AddState(StateSnakeAction.Right,new RightState(shared_data,fsm_action));
        fsm_action.AddState(StateSnakeAction.Down,new DownState(shared_data,fsm_action));
        fsm_status.AddState(StateSnakeStatus.Calm,new CalmState(shared_data,fsm_status));
        fsm_status.AddState(StateSnakeStatus.Furious,new FuriousState(shared_data,fsm_status));
        fsm_status.AddState(StateSnakeStatus.ActivelyFurious,new ActivelyFuriousState(shared_data,fsm_status));
        fsm_action.SwitchState(StateSnakeAction.Left); //初始假设往左走！
        fsm_status.SwitchState(StateSnakeStatus.Calm); //初始为安静状态！
        //挂事件状态
        GameEvents.OnSnakeMove += Update;
    }

    public void Update()
    {
        fsm_action.OnUpdate();
        fsm_status.OnUpdate();
    }
}