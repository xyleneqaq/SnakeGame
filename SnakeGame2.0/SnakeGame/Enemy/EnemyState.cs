namespace SnakeGame;
using System;
public enum StateEnemy
{
    Idle,
    Move,
}

public class EnemyState
{
    private FSM<StateEnemy> fsm;

    public EnemyState(SharedData shared_data)
    {
        this.fsm = new FSM<StateEnemy>();
        fsm.AddState(StateEnemy.Idle,new IdleState(shared_data,fsm,0.2));
        fsm.AddState(StateEnemy.Move,new MoveState(shared_data,fsm,0.8));
        fsm.SwitchState(StateEnemy.Move); //初始假设静止！
        //挂事件状态
        GameEvents.OnEnemyMove += Update;
    }

    public void Update()
    {
        fsm.OnUpdate();
        SharedData.globalData.enemyCluster.Clear();
        SharedData.globalData.enemyCluster.Add(SharedData.globalData.enemyAtom);
    }
}

public class EnemyCluster
{
    
}
