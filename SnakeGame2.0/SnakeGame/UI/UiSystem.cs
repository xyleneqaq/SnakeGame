using System.Collections;

namespace SnakeGame;
using System;
using System.Text;

public enum StateSystem
{
    Pause,
    Return,
    Continue,
}

public enum StateSettle
{
    Full,
    Dead,
    Win,
}

/// <summary>
/// UI系统
/// </summary>
public class UiSystem //注意：类声明后面不能加分号！
{
    private readonly StringBuilder _displayCache = new();
    public UiSystem()
    {
        Console.WriteLine("Welcome to the Snake Game!");
        
        // ui初始化时，便订阅了DisplayMap事件！
        GameEvents.OnMapShow += DisplayMap;
        GameEvents.OnMapClear += Clear;
        GameEvents.OnBloodShow += BloodShow;
        GameEvents.OnPause += Pause;
        GameEvents.OnPause += Exit;
    }

    public void Menu()
    {
        Console.WriteLine("Please select a option:!");
        Console.WriteLine("1. Play");
        Console.WriteLine("2. Exit");
        Console.WriteLine("3. About");
        
        while (true)
        {
            string? flag = Console.ReadLine();
            switch (flag)
            {
                case "1":
                    Console.WriteLine("Game Start!");
                    
                    Load();
                    
                    GameController game = new GameController(); //实例化一个Game，此时事件就订阅上了！
                    
                    GameEvents.TriggerGameStart(); //事件触发：
                    
                    Settlement(); //结算事件
                    
                    return;
                case "2":
                    Console.WriteLine("Game Exit!");
                    return;
                case "3":
                    About();
                    break;
                default:
                    Console.WriteLine("Please input a right key!");
                    break;
            }
        }
    }
    
    /// <summary>
    /// 显示地图，利用图层的方法，在baseMap上进行替换！
    /// </summary>
    private void DisplayMap(SharedData data)
    {
        _displayCache.Clear();
        
        StringBuilder sb = new StringBuilder(
            data.map.map_length * data.map.map_width * 3); 
        // 在地图基底上进行map的修改:1
        //注意：不能直接 Map baseMap = map!这种情况相当于直接复制map的引用！！！
        Map baseMap = data.DeepCopy(data.map); 
        
        // 在地图基底上进行map的修改:2,snake
        foreach (Vector item in data.snake)
        {
            baseMap.MapCursor[item] = Map.MapIndicator.Snake;
        }
        
        // 在地图基底上进行map的修改:3,enemy
        foreach (var item in data.enemyCluster)
        {
            baseMap.MapCursor[item] = Map.MapIndicator.Enemy;
        }
     
        // 显示修改完后的Map
        for (int i = 0; i < data.map.map_length; i++)
        {
            for (int j = 0; j < data.map.map_width; j++)
            {
                DisplayElements(baseMap,new Vector(i, j), out string cursor);
                sb.Append(cursor).Append(' '); //为了提高刷新率，减少I/O操作次数！(I/O操作极其消耗性能！)
            }
            sb.AppendLine();
        }
        Console.Write(sb.ToString());
    }
    
    /// <summary>
    /// 工具函数：将MapCursor的内容可视化
    /// </summary>
    private void DisplayElements(Map map,Vector vector,out string cursor)
    {
        cursor = "5";
        switch (map.MapCursor[vector])
        {
            case Map.MapIndicator.Free:
                cursor = " ";
                break;
            case Map.MapIndicator.Bonus:
                cursor = "1";
                break;
            case Map.MapIndicator.Obstacle:
                cursor = "b";
                break;
            case Map.MapIndicator.Snake:
                switch (SharedData.globalData.snakeStatus)
                {
                    case StateSnakeStatus.Calm:
                        cursor = "★";
                        break;
                    case StateSnakeStatus.Furious:
                        cursor = "☆";
                        break;
                }
                break;
            case Map.MapIndicator.Enemy:
                cursor = "δ";
                break;
        }
    }
    
    /// <summary>
    /// 生成含有随机bonus的地图
    /// </summary>
    public void Load()
    {
        Console.WriteLine("Loading Map...");
        Thread.Sleep(2000);
        
        Map baseMap = new Map(20, 30);
        baseMap.InitializeSnakeMap(new Vector(10, 15));
        baseMap.InitializeObstacleMap();
        baseMap.InitializeBonusMap(0.01);
        // 上传地图数据
        SharedData.SharedDataUpdate(("map",baseMap)); 
        
        Console.WriteLine("Map Loaded!");
        Console.WriteLine("\n");
        DisplayMap(SharedData.globalData);
        Console.WriteLine("\n");
        
        Console.WriteLine("Loading Data...");
        Thread.Sleep(2000);
        
        Queue<Vector> snakeInit = new Queue<Vector>();
        snakeInit.Enqueue(new Vector(6, 15));
        SharedData.SharedDataUpdate(("snake",snakeInit)); // 上传蛇头数据
        Vector enemyInit = new Vector(3,3);
        List<Vector> enemyClusterInit = new List<Vector>();
        SharedData.SharedDataUpdate(("enemyAtom",enemyInit)); // 上传敌怪数据
        enemyClusterInit.Add(enemyInit);
        SharedData.SharedDataUpdate(("enemyCluster",enemyClusterInit)); // 上传敌怪数据
        int enemyBlood = 5;
        SharedData.SharedDataUpdate(("enemyBlood",enemyBlood)); // 上传敌怪数据
        
        
        Console.WriteLine("Global Data Loaded!");
        Console.WriteLine("\n");
        Console.WriteLine("Press Enter key to continue...");
        Console.ReadKey();
    }

    public void Clear()
    {
        Console.Clear();
    }

    public void Pause()
    {
        if (SharedData.globalData.systemCommand == StateSystem.Pause) //暂停！
        {
            Console.WriteLine("Pause!");
            Console.WriteLine("Press Any Key To Continue...");
            Console.ReadKey();
            SharedData.globalData.systemCommand = StateSystem.Continue; //暂停结束后应当恢复原状态！即：游戏进行状态！
        }
    }

    public void Exit()
    {
        if (SharedData.globalData.systemCommand == StateSystem.Return) //暂停！
        {
            while (true)
            {
                Console.WriteLine("Are You Sure to Exit? Yes:Y No:N");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.N:
                        SharedData.globalData.systemCommand = StateSystem.Continue;
                        return;
                    default:
                        Console.WriteLine("\n");
                        Console.WriteLine("Please enter a valid character!");
                        break;
                }
            }
        }
    }

    private void Settlement()
    {
        Console.WriteLine("Game Over!");
        Console.WriteLine($"Your Score:{SharedData.globalData.win * 1000}");
        switch (SharedData.globalData.settleIndicator)
        {
            case StateSettle.Full:
                Console.WriteLine($"Enemy is quite full and has left quite contentedly!");
                break;
            case StateSettle.Dead:
                Console.WriteLine("Snake has ended its life in agony!");
                break;
            case StateSettle.Win:
                Console.WriteLine("Enemy has been defeated!");
                break;
        }
        Thread.Sleep(5000);
        while (Console.KeyAvailable)
        {
            Console.ReadKey(true); // true = 不显示按键，非阻塞读取
        }
        Console.WriteLine("Press Any Key To Continue...");
        Console.ReadKey();
    }

    public void BloodShow(SharedData data)
    {
        int contentedMax = (int)(5 * data.enemyWinMax);
        StringBuilder sb1 = new StringBuilder(5 * data.enemyBlood);
        StringBuilder sb2 = new StringBuilder(contentedMax);
        
        for (int i = 0; i < data.enemyBlood; ++i)
        {
            sb1.Append("-").Append("-").Append("-").Append("-").Append("-");
        }
        
        for (int i = 0; i < contentedMax; ++i)
        {
            if (i < data.enemyWin * 5)
            {
                sb2.Append("-");
            }
            else
            {
                sb2.Append(" ");
            }
        }
        sb2.Append("|").Append("|");
        
        Console.Write("Enemy Blood: " + sb1.ToString() + "\n");
        Console.Write("Enemy Satiety: " + sb2.ToString() + "\n");
    }

    public void About()
    {
        Console.WriteLine("W A S D:Moving");
        Console.WriteLine("\n");
        Console.WriteLine("Q: Pause, ESC: Exit");
        Console.WriteLine("\n");
        Console.WriteLine("Space: Attack (Only in Rage Mode(\'5\')");
        Console.WriteLine("\n");
        Console.WriteLine("There's Nothing To Talk About More...");
        Console.WriteLine("\n");
    }
}