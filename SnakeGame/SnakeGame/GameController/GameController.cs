namespace SnakeGame;
using System.Threading;

public class GameController
{
    private SharedData shared_data;
    private SnakeState snake_state;
    private NativeKeyboard buffer;
    private CollisionDetect colIndicator;

    public GameController(SharedData shared_data)
    {
        this.shared_data = shared_data;
        this.snake_state = new SnakeState(shared_data);
        this.buffer = new NativeKeyboard();
        this.colIndicator = new CollisionDetect(shared_data);
        
        //Game Controller初始化的时候，订阅OnGameStart以及OnGameExit
        GameEvents.OnGameStart += GameLoop;
    }

    public void GameLoop(SharedData shared_data)
    {
        while (!shared_data.deathIndicator)
        {
            GameEvents.TriggerMapClear();
            GameEvents.TriggerMapShow(shared_data);
            GameEvents.TriggerKeyboardInput();
            GameEvents.TriggerPause(); //检测是否输入了暂停
            shared_data.GlobalCopy();// 下载数据缓存
            GameEvents.TriggerSnakeMovement();
            shared_data.GlobalCopy();// 下载数据缓存
            GameEvents.TriggerGameOver(); //判断是否满足游戏结束的条件
            shared_data.GlobalCopy();
        }

        Console.WriteLine("Game Over!");
        Console.WriteLine($"Your Score:{GlobalData.GlobalWin * 1000}");
        
        Console.WriteLine("Press any key to continue...!");
        Thread.Sleep(5000);
        Console.ReadKey();

        GameEvents.ClearAllSubscriptions();
    }
}