namespace SnakeGame;
using System.Threading;
using System;
using System.Collections.Generic;

//为什么要OnEnter以及OnUpgrade各写一份相同的蛇更新逻辑呢？
//这是因为在检测输入Switch状态时候，我们要保证这一“帧”是已经响应我们的输入了的。此时必须让Switch函数调用的"OnStart"能够起到更新蛇的效果！
public class LeftState : IState
{
    private SharedData shared_data;
    private FSM<StateSnake> fsm;

    public LeftState(SharedData shared_data, FSM<StateSnake> fsm)
    {
        this.shared_data = shared_data;
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        // 创建一个新的往左的头！
        Vector newHead = new Vector(shared_data.snake.Last().x, shared_data.snake.Last().y - 1);

        if (shared_data.map.MapCursor[shared_data.snake.Last()] == Map.MapIndicator.Bonus)
        {
            shared_data.GlobalCopy(); //更新数据
            GameEvents.TriggerMapUpdate(shared_data,shared_data.snake.Last(),Map.MapIndicator.Free);
            shared_data.snake.Enqueue(newHead);
            GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
        }
        else
        {
            shared_data.snake.Enqueue(newHead);
            shared_data.snake.Dequeue();
            GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
        }
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        // 创建一个新的往左的头！
        Vector newHead = new Vector(shared_data.snake.Last().x, shared_data.snake.Last().y - 1); //按照队尾更新
        
        // 状态改变,并将蛇的数据上传到"数据库"里。
        // 注意判断是否踩到了bonus
        // 输入检测，发现有输入那么切换状态
        switch (shared_data.key)
        {
            case 0x57:
                fsm.SwitchState(StateSnake.Up);
                break;
            case 0x53:
                fsm.SwitchState(StateSnake.Down);
                break;
            default:
                if (shared_data.map.MapCursor[shared_data.snake.Last()] == Map.MapIndicator.Bonus)
                {
                    shared_data.GlobalCopy(); //更新数据
                    GameEvents.TriggerMapUpdate(shared_data,shared_data.snake.Last(),Map.MapIndicator.Free);
                    shared_data.snake.Enqueue(newHead);
                    GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
                }
                else
                {
                    shared_data.snake.Enqueue(newHead);
                    shared_data.snake.Dequeue();
                    GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
                }
                break;
        }
    }
}
public class UpState : IState
{
    private SharedData shared_data; 
    private FSM<StateSnake> fsm;
    
    public UpState(SharedData shared_data, FSM<StateSnake> fsm)
    {
        this.shared_data = shared_data;
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        // 创建一个新的往上的头！
        Vector newHead = new Vector(shared_data.snake.Last().x - 1, shared_data.snake.Last().y);
        
        // 状态改变，上传数据到数据库！
        if (shared_data.map.MapCursor[shared_data.snake.Last()] == Map.MapIndicator.Bonus)
        {
            shared_data.GlobalCopy(); //更新数据
            GameEvents.TriggerMapUpdate(shared_data,shared_data.snake.Last(),Map.MapIndicator.Free);
            shared_data.snake.Enqueue(newHead);
            GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
        }
        else
        {
            shared_data.snake.Enqueue(newHead);
            shared_data.snake.Dequeue();
            GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
        }
    }

    public void OnExit()
    {

    }
    
    public void OnUpdate()
    {
        // 创建一个新的往左的头！
        Vector newHead = new Vector(shared_data.snake.Last().x - 1, shared_data.snake.Last().y);
        
        // 状态改变,并将蛇的数据上传到"数据库"里。
        // 输入检测，发现有输入那么切换状态
        switch (shared_data.key)
        {
            case 0x41:
                fsm.SwitchState(StateSnake.Left);
                break;
            case 0x44:
                fsm.SwitchState(StateSnake.Right);
                break;
            default:
                if (shared_data.map.MapCursor[shared_data.snake.Last()] == Map.MapIndicator.Bonus)
                {
                    shared_data.GlobalCopy(); //更新数据
                    GameEvents.TriggerMapUpdate(shared_data,shared_data.snake.Last(),Map.MapIndicator.Free);
                    shared_data.snake.Enqueue(newHead);
                    GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
                }
                else
                {
                    shared_data.snake.Enqueue(newHead);
                    shared_data.snake.Dequeue();
                    GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
                }
                break;
        }
    }
}

public class RightState : IState
{
    private SharedData shared_data; 
    private FSM<StateSnake> fsm;
    
    public RightState(SharedData shared_data, FSM<StateSnake> fsm)
    {
        this.shared_data = shared_data;
        this.fsm = fsm;
    }
    public void OnEnter()
    {
        // 创建一个新的往右的头！
        Vector newHead = new Vector(shared_data.snake.Last().x, shared_data.snake.Last().y + 1);
             
        // 更新状态，并上传数据库！
        if (shared_data.map.MapCursor[shared_data.snake.Last()] == Map.MapIndicator.Bonus)
        {
            shared_data.GlobalCopy(); //更新数据
            GameEvents.TriggerMapUpdate(shared_data,shared_data.snake.Last(),Map.MapIndicator.Free);
            shared_data.snake.Enqueue(newHead);
            GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
        }
        else
        {
            shared_data.snake.Enqueue(newHead);
            shared_data.snake.Dequeue();
            GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
        }
    }

    public void OnExit()
    {

    }
    public void OnUpdate()
    {
        // 创建一个新的往左的头！
        Vector newHead = new Vector(shared_data.snake.Last().x, shared_data.snake.Last().y + 1);
        
        // 状态改变,并将蛇的数据上传到"数据库"里。
        // 注意判断是否踩到了bonus
        // 输入检测，发现有输入那么切换状态
        switch (shared_data.key)
        {
            case 0x57:
                fsm.SwitchState(StateSnake.Up);
                break;
            case 0x53:
                fsm.SwitchState(StateSnake.Down);
                break;
            default:
                if (shared_data.map.MapCursor[shared_data.snake.Last()] == Map.MapIndicator.Bonus)
                {
                    shared_data.GlobalCopy(); //更新数据
                    GameEvents.TriggerMapUpdate(shared_data,shared_data.snake.Last(),Map.MapIndicator.Free);
                    shared_data.snake.Enqueue(newHead);
                    GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
                }
                else
                {
                    shared_data.snake.Enqueue(newHead);
                    shared_data.snake.Dequeue();
                    GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
                }
                break;
        }
    }
}

public class DownState : IState
{
    private SharedData shared_data; 
    private FSM<StateSnake> fsm;
    
    public DownState(SharedData shared_data, FSM<StateSnake> fsm)
    {
        this.shared_data = shared_data;
        this.fsm = fsm;
    }
    public void OnEnter()
    {
        // 创建一个新的往下的头！
        Vector newHead = new Vector(shared_data.snake.Last().x + 1, shared_data.snake.Last().y);
        
        // 状态改变，上传数据到数据库！
        if (shared_data.map.MapCursor[shared_data.snake.Last()] == Map.MapIndicator.Bonus)
        {
            shared_data.GlobalCopy(); //更新数据
            GameEvents.TriggerMapUpdate(shared_data,shared_data.snake.Last(),Map.MapIndicator.Free);
            shared_data.snake.Enqueue(newHead);
            GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
        }
        else
        {
            shared_data.snake.Enqueue(newHead);
            shared_data.snake.Dequeue();
            GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
        }
    }

    public void OnExit()
    {

    }
    public void OnUpdate()
    {
        // 创建一个新的往左的头！
        Vector newHead = new Vector(shared_data.snake.Last().x + 1, shared_data.snake.Last().y);
        
        // 状态改变,并将蛇的数据上传到"数据库"里。
        // 输入检测，发现有输入那么切换状态
        switch (shared_data.key)
        {
            case 0x41:
                fsm.SwitchState(StateSnake.Left);
                break;
            case 0x44:
                fsm.SwitchState(StateSnake.Right);
                break;
            default:
                if (shared_data.map.MapCursor[shared_data.snake.Last()] == Map.MapIndicator.Bonus)
                {
                    shared_data.GlobalCopy(); //更新数据
                    GameEvents.TriggerMapUpdate(shared_data,shared_data.snake.Last(),Map.MapIndicator.Free);
                    shared_data.snake.Enqueue(newHead);
                    GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
                }
                else
                {
                    shared_data.snake.Enqueue(newHead);
                    shared_data.snake.Dequeue();
                    GlobalData.GlobalUpdate(("GlobalSnake", shared_data.snake)); //上传到公用数据库
                }
                break;
        }
    }
}