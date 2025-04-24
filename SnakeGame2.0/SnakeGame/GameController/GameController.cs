namespace SnakeGame;
using System.Threading;

public class GameController
{
    private SnakeState snake_state;
    private EnemyState enemy_state;
    private NativeKeyboard buffer;
    private CollisionDetect colIndicator;

    public GameController()
    {
        this.snake_state = new SnakeState(SharedData.globalData);
        this.enemy_state = new EnemyState(SharedData.globalData);
        this.buffer = new NativeKeyboard();
        this.colIndicator = new CollisionDetect();
        
        //Game Controller初始化的时候，订阅OnGameStart以及OnGameExit
        GameEvents.OnGameStart += GameLoop;
    }

    public void GameLoop()
    {
        while (SharedData.globalData.settleIndicator == null)
        {
            GameEvents.TriggerMapClear();
            GameEvents.TriggerMapShow(SharedData.globalData);
            GameEvents.TriggerBloodShow();
            GameEvents.TriggerKeyboardInput();
            GameEvents.TriggerPause(); //检测是否输入了暂停
            GameEvents.TriggerSnakeMovement(); //蛇运动
            GameEvents.TriggerEnemyMovement(); //敌人运动
            GameEvents.TriggerGameOver(SharedData.globalData); //判断是否满足游戏结束的条件
        }
    }
}