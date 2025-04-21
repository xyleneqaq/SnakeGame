namespace SnakeGame;
using System;
using System.Text;

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
        GameEvents.OnPause += Pause;
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
                    SharedData data1 = Load(); //创建新数据！
                    
                    GameController game = new GameController(data1); //实例化一个Game，此时事件就订阅上了！
                    
                    GameEvents.TriggerGameStart(data1); //事件触发：
                    return;
                case "2":
                    Console.WriteLine("Game Exit!");
                    return;
                case "3":
                    return;
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
        
        // 在地图基底上进行map的修改:2
        foreach (Vector item in data.snake)
        {
            baseMap.MapCursor[item] = Map.MapIndicator.Snake;
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
        cursor = "0";
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
                cursor = "★";
                break;
        }
    }
    
    /// <summary>
    /// 生成含有随机bonus的地图
    /// </summary>
    public SharedData Load()
    {
        SharedData data = new SharedData(); // 创建新的缓存文件
        
        Console.WriteLine("Loading Map...");
        Thread.Sleep(2000);
        
        Map baseMap = new Map(20, 30);
        baseMap.InitializeSnakeMap(new Vector(10, 15));
        baseMap.InitializeObstacleMap();
        baseMap.InitializeBonusMap(0.01);
        // 上传地图数据
        GlobalData.GlobalUpdate(("GlobalMap",baseMap)); 
        data.GlobalCopy(); //下载缓存，打包
        
        Console.WriteLine("Map Loaded!");
        Console.WriteLine("\n");
        DisplayMap(data);
        Console.WriteLine("\n");
        
        Console.WriteLine("Loading Data...");
        Thread.Sleep(2000);
        Queue<Vector> snakeInit = new Queue<Vector>();
        snakeInit.Enqueue(new Vector(6, 15));
        GlobalData.GlobalUpdate(("GlobalSnake",snakeInit)); // 上传蛇头数据
        data.GlobalCopy(); //下载缓存，打包
        
        Console.WriteLine("Global Data Loaded!");
        Console.WriteLine("\n");
        Console.WriteLine("Press Enter key to continue...");
        Console.ReadKey();

        return data;
    }

    public void Clear()
    {
        Console.Clear();
    }

    public void Pause()
    {
        if (GlobalData.GlobalKey == 0x51) //Q键暂停！
        {
            Console.WriteLine("Pause!");
            Thread.Sleep(2000);
        }
    }
}