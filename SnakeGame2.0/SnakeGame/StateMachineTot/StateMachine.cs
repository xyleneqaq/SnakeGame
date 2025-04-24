namespace SnakeGame;
using System;
using System.Collections.Generic;

// 状态转变的一些函数
public interface IState
{
    void OnEnter();
    void OnExit();
    void OnUpdate();
}

public class FSM<T>
{
    public IState curState;
    public Dictionary<T, IState> states; //新建一个字典，用于储存 状态 - 操作 之间的映射！

    public FSM() // FSM无需调用共享数据，只需要专注切换状态即可！
    {
        states = new Dictionary<T, IState>(); 
    }

    // 添加状态
    public void AddState(T stateType,IState state)
    {
        if (states.ContainsKey(stateType))
        {
            Console.WriteLine("State already exists");
            return;
        }
        states.Add(stateType, state);
    }

    // 状态转换
    public void SwitchState(T stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            return;
        }

        //逻辑：如果当前状态不为空，那么退出当前状态
        if (curState != null)
        {
            curState.OnExit();
        }
        curState = states[stateType];  //切换到当前状态，也即: ...key的...
        curState.OnEnter();
    }

    public void StartState(T stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            return;
        }
        
        curState = states[stateType];  //切换到当前状态，也即: ...key的...。注意此处我们不调用Enter!
    }

    // 状态更新
    public void OnUpdate()
    {
        curState.OnUpdate();
    }
}
