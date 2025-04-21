using System.Runtime.CompilerServices;

namespace SnakeGame;
using System.Linq; //保证我可以进行队尾Peek操作！

public class CollisionDetect
{
    public SharedData shared_data;

    public CollisionDetect(SharedData shared_data)
    {
        this.shared_data = shared_data;
        
        //挂事件
        GameEvents.OnGameOver += Detection;
        GameEvents.OnGameOver += WinDetection;
    }

    private bool SnakeSnake()
    {
        bool flag = false;
        
        // 队列的深拷贝,并队列倒置（设置为栈）
        Stack<Vector> snake_temp = shared_data.DeepCopy(shared_data.snake);
        
        // 提取队尾，并删除
        Vector observed = snake_temp.Pop();

        if (snake_temp.Count() != 1)
        {
            foreach (Vector item in snake_temp)
            {
                if (item == observed)
                {
                    flag = true;
                    break;
                }
            }
        }
        return flag;
    }

    private bool SnakeWall()
    {
        bool flag = false;

        foreach (Vector item in shared_data.snake)
        {
            if (shared_data.map.MapCursor[item] == Map.MapIndicator.Obstacle) //0代表墙(Obstacle)的Enumerate代号！
            {
                flag = true;
                break;
            }
        }
        return flag;
    }

    public void Detection()
    {
        bool flag = SnakeSnake() || SnakeWall(); //只要满足其中一个条件就寄了！
        GlobalData.GlobalUpdate(("GlobalDeathIndicator",flag));
    }

    public void WinDetection()
    {
        bool flag = true;
        foreach(var item in shared_data.map.MapCursor)
        {
            if (item.Value == Map.MapIndicator.Bonus)
            {
                flag = false;
            }
        }
        //如果全部Bonus都被吃完，那么重新更新一波！
        if (flag)
        {
            shared_data.map.InitializeBonusMap(0.015);
            GlobalData.GlobalUpdate(("GlobalWin",GlobalData.GlobalWin + 1));
        }
    }
}