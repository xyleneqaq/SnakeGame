namespace SnakeGame;
using System.Reflection;
using System.Linq;

/// <summary>
/// 矢量结构体。实际上可以利用现成的Vector2类代替！
/// </summary>
public struct Vector
{
    public int x;
    public int y;
        
    // 添加构造函数方便初始化
    public Vector(int x, int y) 
    {
        this.x = x;
        this.y = y;
    }

    // 运算符重载！
    public static bool operator ==(Vector left, Vector right)
    {
        return left.x == right.x && left.y == right.y;
    }
    
    public static bool operator !=(Vector left, Vector right)
    {
        return left.x == right.x && left.y == right.y;
    }
}

/// <summary>
/// 事件管理器类，专门用于储存分派事件以及事件通信。性质：静态的函数指针类？
/// </summary>
public static class GameEvents
{
    // 游戏状态事件
    public static event Action<SharedData> OnGameStart;
    public static event Action OnSnakeMove;
    
    // 游戏渲染事件
    public static event Action<SharedData> OnMapShow; // 地图渲染更新时触发
    public static event Action OnMapClear; // 清除地图渲染时候触发
    
    // 地图更新事件
    public static event Action<SharedData,Vector,Map.MapIndicator> OnMapUpdate; // 地图更新时触发
    public static event Action<Vector,Map.MapIndicator> OnMapSnakeOverwrite; // 用于更新地图可用区域的覆写
    
    // 输入监听事件
    public static event Action OnKeyboardInput; //触发输入事件
    
    //UI调用事件
    public static event Action OnPause; //游戏暂停
    
    // 胜利条件 / 碰撞检测
    public static event Action OnGameOver; // 游戏结束
    
    // 触发事件的方法
    public static void TriggerGameStart(SharedData data) => OnGameStart?.Invoke(data);
    public static void TriggerMapShow(SharedData data) => OnMapShow?.Invoke(data);
    public static void TriggerMapClear() => OnMapClear?.Invoke();
    public static void TriggerKeyboardInput() => OnKeyboardInput?.Invoke();
    public static void TriggerSnakeMovement() => OnSnakeMove?.Invoke();
    public static void TriggerPause() => OnPause?.Invoke();
    public static void TriggerGameOver() => OnGameOver?.Invoke();
    public static void TriggerMapUpdate(SharedData data,Vector vector,Map.MapIndicator indicator) => OnMapUpdate?.Invoke(data,vector,indicator);
    
    // 重要！用于释放函数指针！
    public static void ClearAllSubscriptions()
    {
        OnGameStart = null;
        OnSnakeMove = null;
        OnMapShow = null;
        OnMapClear = null;
        OnMapUpdate = null;
        OnPause = null;
        OnGameOver = null;
        OnKeyboardInput = null;
        OnSnakeMove = null;
    }
}

/// <summary>
/// 数据类，专门用于储存数据和数据通信！
/// </summary>
public static class GlobalData
{
    // Initialization!Get/Set:属性
    public static Queue<Vector> GlobalSnake { get; set; } = new Queue<Vector>();
    public static List<Vector> GlobalEnemy { get; set; } = new List<Vector>();
    public static bool GlobalDeathIndicator { get; set; } = false;
    public static int GlobalWin { get; set; } = 0;
    public static Map GlobalMap { get; set; } = new Map(10,10);
    public static int? GlobalKey { get; set; } = null; // I/O数据上传
    public static StateSnake GlobalSnakeDir { get; set; } = StateSnake.Left;

    // 利用反射实现数据的更新！
    public static void GlobalUpdate(params (string name, object value)[] updates)
    {
        foreach (var (name, value) in updates)
        {
            MemberInfo member = typeof(GlobalData)
                .GetMember(name, BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault();

            if (member is FieldInfo field)
                field.SetValue(null, value);  // 静态字段赋值（obj参数为null）
            else if (member is PropertyInfo prop)
                prop.SetValue(null, value);   // 静态属性赋值
        }
    }
}

/// <summary>
/// 轻量级的共享数据
/// </summary>
public class SharedData : ITools
{
    // Initialization!
    public Queue<Vector> snake;
    public List<Vector> enemy;
    public bool deathIndicator;
    public int win;
    public Map map;
    public int? key;
    public StateSnake snakeDir;

    public SharedData()
    {
        GlobalCopy();
    }

    public void GlobalCopy()
    {
        this.snake = GlobalData.GlobalSnake;
        this.enemy = GlobalData.GlobalEnemy;
        this.map = GlobalData.GlobalMap;
        this.deathIndicator = GlobalData.GlobalDeathIndicator;
        this.win = GlobalData.GlobalWin;
        this.key = GlobalData.GlobalKey;
        this.snakeDir = GlobalData.GlobalSnakeDir;
    }

    // 队列转栈！
    public Stack<Vector> DeepCopy(Queue<Vector> obj)
    {
        Stack<Vector> temp = new Stack<Vector>();

        foreach (Vector vector in obj)
        {
            temp.Push(vector);
        }
        return temp;
    }
    
    //深拷贝函数,保证Copy出来的地图之间是无关的！
    public Map DeepCopy(Map obj)
    {
        Map map_temp = new Map(obj.map_length,obj.map_width);
        map_temp.MapCursor = new Dictionary<Vector,Map.MapIndicator>(); // 结构体和enum类型类似静态的！

        for (int i = 0; i < map_temp.map_length; i++)
        {
            for (int j = 0; j < map_temp.map_width; j++)
            {
                map_temp.MapCursor[new Vector(i, j)] = obj.MapCursor[new Vector(i, j)];
            }
        }
        return map_temp;
    }
}
