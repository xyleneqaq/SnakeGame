namespace SnakeGame;
using System;

public enum StateSnake
{
    Left,
    Up,
    Down,
    Right,
}

public class SnakeState
{
    private FSM<StateSnake> fsm;
    public SharedData shared_data;

    public SnakeState(SharedData shared_data)
    {
        this.shared_data = shared_data;  //从数据库中调用出SnakeGame的当前状态！
        this.fsm = new FSM<StateSnake>(shared_data);
        fsm.AddState(StateSnake.Left,new LeftState(shared_data,fsm));
        fsm.AddState(StateSnake.Up,new UpState(shared_data,fsm));
        fsm.AddState(StateSnake.Right,new RightState(shared_data,fsm));
        fsm.AddState(StateSnake.Down,new DownState(shared_data,fsm));
        fsm.SwitchState(StateSnake.Left); //初始假设往左走！
        //挂事件状态
        GameEvents.OnSnakeMove += Update;
    }

    public void Update()
    {
        shared_data.GlobalCopy();
        fsm.OnUpdate();
    }
}