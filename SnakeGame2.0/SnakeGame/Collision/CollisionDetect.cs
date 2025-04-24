using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace SnakeGame;
using System.Linq; //保证我可以进行队尾Peek操作！

public class CollisionDetect
{
    public CollisionDetect()
    {
        //挂事件
        GameEvents.OnGameOver += EnemyCollision;
        GameEvents.OnGameOver += SnakeSnake;
        GameEvents.OnGameOver += SnakeWall;
        GameEvents.OnGameOver += SnakeEnemy;
        GameEvents.OnGameOver += EnemyFull;
        GameEvents.OnGameOver += WinDetection;
        GameEvents.OnGameOver += LoopDetection;
    }

    private void SnakeSnake(SharedData data)
    {
        if (data.snake.Count != 1)
        {
            // 队列的深拷贝,并队列倒置（设置为栈）
            Stack<Vector> snake_temp = data.DeepStackCopy(data.snake);
                    
            // 提取队尾，并删除
            Vector observed = snake_temp.Pop();
            
            if (snake_temp.Count() != 1)
            {
                foreach (Vector item in snake_temp)
                {
                    if (item == observed)
                    {
                        SharedData.SharedDataUpdate(("settleIndicator",StateSettle.Dead));
                        break;
                    }
                }
            }
        }
    }

    private void SnakeWall(SharedData data)
    {
        foreach (Vector item in data.snake)
        {
            if (data.map.MapCursor[item] == Map.MapIndicator.Obstacle)
            {
                SharedData.SharedDataUpdate(("settleIndicator",StateSettle.Dead));
                break;
            }
        }
    }

    private void SnakeEnemy(SharedData data)
    {
        if (data.enemyAtom == data.snake.Last())
        {
            SharedData.SharedDataUpdate(("settleIndicator",StateSettle.Dead));
        }
    }

    private void EnemyFull(SharedData data)
    {
        if (data.enemyWin >= data.enemyWinMax) //被吃比例为enemyWinMax，则游戏结束！
        {
            SharedData.SharedDataUpdate(("settleIndicator",StateSettle.Full));
        }
    }

    private void WinDetection(SharedData data)
    {
        if (data.enemyBlood == 0)
        {
            SharedData.SharedDataUpdate(("settleIndicator",StateSettle.Win));
        }
    }

    public void LoopDetection(SharedData data)
    {
        bool flag = true;
        foreach(var item in data.map.MapCursor)
        {
            if (item.Value == Map.MapIndicator.Bonus)
            {
                flag = false;
            }
        }
        //如果全部Bonus都被吃完，那么重新更新一波！
        if (flag)
        {
            data.map.InitializeBonusMap(0.015);
            SharedData.SharedDataUpdate(("win",SharedData.globalData.win + 1));
        }
    }

    public void EnemyCollision(SharedData data)
    {
        Vector enemy_copy = new Vector();
        enemy_copy = data.enemyAtom;

        Queue<Vector> snake_temp = data.DeepQueueCopy(data.snake);
        
        bool flag = false;
        int sum = 0;

        // 注意：此处犯了一个极其容易犯的错误！
        // 此处，循环判断的上限是会随着 “队列” 的更新而变化的！
        // 显然，我们是要遍历原先的队列，也就是说：我们需要遍历“原先队列”的所有值！
        // 并且，此时如果尝试用foreach进行修改，会报错。这是因为 foreach本质上是用迭代器进行修改，迭代器不允许在遍历时修改集合！
        // 由此，解决方式：1.将 “原先队列”中要删除的Entity进行临时存储 2.倒序for循环 √（最好用！）
        // 此处：由于不涉及到Snake队列中Entity的Index-Dependent Altering，直接保存循环上限 亦可！
        int snake_length = snake_temp.Count; 
        for(int i = 0; i < snake_length; ++i)
        {
            sum = sum + 1;
            if (snake_temp.Count > 1)
            {
                if (enemy_copy == snake_temp.Dequeue())
                {
                    flag = true;
                    break;
                }
            }
        }

        if (flag)
        {
            double portionEat = (double)sum / (double)snake_length + (double)sum / (double)15.0; //综合被吃的比例以及被吃数目计算！
            SharedData.SharedDataUpdate(("enemyWin",data.enemyWin + portionEat));
            SharedData.SharedDataUpdate(("snake",snake_temp));
        }
    }
}