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
    public static event Action OnGameStart;
    public static event Action OnSnakeMove; // 蛇移动
    public static event Action OnEnemyMove; // 敌人移动
    
    // 游戏渲染事件
    public static event Action<SharedData> OnMapShow; // 地图渲染更新时触发
    public static event Action OnMapClear; // 清除地图渲染时候触发
    public static event Action<SharedData> OnBloodShow; //敌人血条显示
    
    // 地图更新事件
    public static event Action<Map,Vector,Map.MapIndicator> OnMapUpdate; // 地图更新时触发
    
    // 输入监听事件
    public static event Action OnKeyboardInput; //触发输入事件
    
    //UI调用事件
    public static event Action OnPause; //游戏暂停
    
    // 胜利条件 / 碰撞检测
    public static event Action<SharedData> OnGameOver; // 游戏结束
    
    // 触发事件的方法
    public static void TriggerGameStart() => OnGameStart?.Invoke(); // Invoke：执行委托订阅的所有函数！
    public static void TriggerMapShow(SharedData data) => OnMapShow?.Invoke(data);
    public static void TriggerMapClear() => OnMapClear?.Invoke();
    public static void TriggerBloodShow() => OnBloodShow?.Invoke(SharedData.globalData);
    public static void TriggerKeyboardInput() => OnKeyboardInput?.Invoke();
    public static void TriggerSnakeMovement() => OnSnakeMove?.Invoke();
    public static void TriggerPause() => OnPause?.Invoke();
    public static void TriggerGameOver(SharedData data) => OnGameOver?.Invoke(data);
    public static void TriggerMapUpdate(Map map,Vector vector,Map.MapIndicator indicator) => OnMapUpdate?.Invoke(map,vector,indicator);
    public static void TriggerEnemyMovement() => OnEnemyMove?.Invoke();
    
    // 重要！用于回收释放函数指针！
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
/// 共享数据类
/// </summary>
public class SharedData : ITools
{
    // Initialization!
    public Queue<Vector>? snake;
    public StateSnakeStatus? snakeStatus;
    public bool snakeBonusCount;
    public StateSnakeAction? snakeCommandAction;
    public StateSnakeAttack? snakeCommandStatus;
    public bool snakeCommandLock;
    public int snakeRange;
    
    public Vector enemyAtom;
    public List<Vector> enemyCluster;
    public int enemyBlood;
    
    public StateSettle? settleIndicator;
    public int win;
    public double enemyWin;
    public double enemyWinMax;
    
    public Map map;
    public StateSystem? systemCommand;

    public static SharedData globalData = new SharedData(); //静态成员，无需实例化！

    public SharedData()
    {
        this.settleIndicator = null;
        this.win = 0;
        this.enemyWin = 0;
        this.enemyWinMax = 6.0;
        
        this.snake = new Queue<Vector>();
        this.snakeStatus = null;
        this.snakeBonusCount = false;
        this.snakeRange = 4;
        
        this.enemyAtom = new Vector();
        this.enemyCluster = new List<Vector>();
        this.enemyBlood = 0;
        
        this.snakeCommandAction = null;
        this.snakeCommandStatus = null;
        this.systemCommand = null;
        this.snakeCommandLock = true;
        
        this.map = new Map(10,10);
    }

    // 利用反射，进行数据的动态更新！(但是似乎没有必要？甚至降低了代码效率！)
    public static void SharedDataUpdate(params (string name, object value)[] updates)
    {
        foreach (var (name, value) in updates)
        {
            MemberInfo member = typeof(SharedData)
                .GetMember(name, BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(); //注意：此处需要查找的是实例！
            
            if (member is FieldInfo field)
                field.SetValue(globalData, value);  // 静态字段赋值（obj参数为null）
            else if (member is PropertyInfo prop)
                prop.SetValue(globalData, value);   // 静态属性赋值 (也即：由{get;set;}定义的东西！)
        }
    }

    // 队列转栈！
    public Stack<Vector> DeepStackCopy(Queue<Vector> obj)
    {
        Stack<Vector> temp = new Stack<Vector>();

        foreach (Vector vector in obj)
        {
            temp.Push(vector);
        }
        return temp;
    }
    
    // 队列转队列！
    public Queue<Vector> DeepQueueCopy(Queue<Vector> obj)
    {
        Queue<Vector> temp = new Queue<Vector>();

        foreach (Vector vector in obj)
        {
            temp.Enqueue(vector);
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