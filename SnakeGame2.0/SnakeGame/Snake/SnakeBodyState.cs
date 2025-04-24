namespace SnakeGame;

public class CalmState:IState
{
    private int snakeEatCount;
    private int snakeTimeCount;
    private SharedData data;
    private FSM<StateSnakeStatus> fsm;

    public CalmState(SharedData data,FSM<StateSnakeStatus> fsm)
    {
        snakeEatCount = 0;
        snakeTimeCount = 0;
        this.data = data;
        this.fsm = fsm;
    }
    
    public void OnEnter()
    {
        SharedData.SharedDataUpdate(("snakeStatus", StateSnakeStatus.Calm));
        snakeEatCount = 0;
        snakeTimeCount = 0;
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        SharedData.SharedDataUpdate(("snakeStatus", StateSnakeStatus.Calm));
        if (data.snakeBonusCount)
        {
            ++snakeEatCount;
        }
        if (snakeEatCount > 7 && snakeTimeCount > 15)
        {
            fsm.SwitchState(StateSnakeStatus.Furious);
        }
        ++snakeTimeCount;
        SharedData.SharedDataUpdate(("snakeBonusCount", false)); //上传到公用数据库
    }
}

public class FuriousState:IState
{
    private int snakeEatCount;
    private SharedData data;
    private FSM<StateSnakeStatus> fsm;

    public FuriousState(SharedData data,FSM<StateSnakeStatus> fsm)
    {
        snakeEatCount = 0;
        this.data = data;
        this.fsm = fsm;
    }
    public void OnEnter()
    {
        SharedData.SharedDataUpdate(("snakeStatus", StateSnakeStatus.Furious));
        SharedData.SharedDataUpdate(("snakeBonusCount", false)); //上传到公用数据库
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        double delta_x = Math.Abs(data.enemyAtom.x - data.snake.Last().x);
        double delta_y = Math.Abs(data.enemyAtom.y - data.snake.Last().y);
        
        SharedData.SharedDataUpdate(("snakeStatus", StateSnakeStatus.Furious));
        if(delta_x <= data.snakeRange && delta_y <= data.snakeRange)
        {
            fsm.SwitchState(StateSnakeStatus.ActivelyFurious);
        }
    }
}

public class ActivelyFuriousState:IState
{
    private int snakeEatCount;
    private SharedData data;
    private FSM<StateSnakeStatus> fsm;

    public ActivelyFuriousState(SharedData data,FSM<StateSnakeStatus> fsm)
    {
        snakeEatCount = 0;
        this.data = data;
        this.fsm = fsm;
    }
    public void OnEnter()
    {
        SharedData.SharedDataUpdate(("snakeStatus", StateSnakeStatus.ActivelyFurious));
        SharedData.SharedDataUpdate(("snakeBonusCount", false)); //上传到公用数据库
        data.snakeCommandLock = false;
        SharedData.SharedDataUpdate(("snakeCommandLock", false)); //解除命令锁
    }

    public void OnExit()
    {
        data.snakeCommandLock = true;
        SharedData.SharedDataUpdate(("snakeCommandLock", true)); //重新施加命令锁
    }

    public void OnUpdate()
    {
        double delta_x = Math.Abs(data.enemyAtom.x - data.snake.Last().x);
        double delta_y = Math.Abs(data.enemyAtom.y - data.snake.Last().y);
        
        SharedData.SharedDataUpdate(("snakeStatus", StateSnakeStatus.ActivelyFurious));
        if(!(delta_x <= data.snakeRange && delta_y <= data.snakeRange))
        {
            fsm.SwitchState(StateSnakeStatus.Furious);
        }
        if (data.snakeCommandStatus == StateSnakeAttack.Attack)
        {
            SharedData.SharedDataUpdate(("enemyBlood", data.enemyBlood - 1));
            fsm.SwitchState(StateSnakeStatus.Calm);
            SharedData.SharedDataUpdate(("snakeCommandStatus", StateSnakeAttack.Idle));
        }
    }
}