namespace SnakeGame;

public class IdleState : IState
{
    private SharedData shared_data;
    private FSM<StateEnemy> fsm;
    private double idleProb;
    private Random ran;

    public IdleState(SharedData shared_data, FSM<StateEnemy> fsm,double prob)
    {
        this.shared_data = shared_data;
        this.fsm = fsm;
        this.idleProb = prob;
        this.ran = new Random();
    }
    public void OnEnter()
    {

    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if (ran.NextDouble() > idleProb)
        {
            fsm.SwitchState(StateEnemy.Move); //如果不静止，那么就切换为移动状态
        }
    }
}

public class MoveState : IState
{
    private SharedData shared_data;
    private FSM<StateEnemy> fsm;
    private double moveProb;
    private Random ran;

    public MoveState(SharedData shared_data, FSM<StateEnemy> fsm,double prob)
    {
        this.shared_data = shared_data;
        this.fsm = fsm;
        this.moveProb = prob;
        this.ran = new Random();
    }
    public void OnEnter()
    {
        Vector snakeTemp = ReadSnakeBody();
        
        double delta_x = shared_data.enemyAtom.x - snakeTemp.x;
        double delta_y = shared_data.enemyAtom.y - snakeTemp.y;
        double prob_x = delta_x / (delta_x + delta_y);

        if (ran.NextDouble() < prob_x)
        {
            SharedData.SharedDataUpdate(("enemyAtom",new Vector(shared_data.enemyAtom.x - Math.Sign(delta_x) , shared_data.enemyAtom.y)));
        }
        else
        {
            SharedData.SharedDataUpdate(("enemyAtom",new Vector(shared_data.enemyAtom.x , shared_data.enemyAtom.y - Math.Sign(delta_y))));
        }
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        Vector snakeTemp = ReadSnakeBody();
        
        if (ran.NextDouble() > moveProb)
        {
            fsm.SwitchState(StateEnemy.Idle); //如果不静止，那么就切换为移动状态
        }
        else
        {
            double delta_x = shared_data.enemyAtom.x - snakeTemp.x;  //注意：为了保证能除出小数，变量类型必须为浮点数/double！
            double delta_y = shared_data.enemyAtom.y - snakeTemp.y;
            double prob_x = delta_x / (delta_x + delta_y);

            if (delta_x > 1.5 && delta_y > 1.5)
            {
                if (ran.NextDouble() < prob_x)
                {
                    SharedData.SharedDataUpdate(("enemyAtom",new Vector(shared_data.enemyAtom.x - Math.Sign(delta_x), shared_data.enemyAtom.y)));
                }
                else
                {
                    SharedData.SharedDataUpdate(("enemyAtom",new Vector(shared_data.enemyAtom.x , shared_data.enemyAtom.y - Math.Sign(delta_y) )));
                }
            }
            else
            {
                if (ran.NextDouble() < 0.5)
                {
                    SharedData.SharedDataUpdate(("enemyAtom",new Vector(shared_data.enemyAtom.x - Math.Sign(delta_x), shared_data.enemyAtom.y)));
                }
                else
                {
                    SharedData.SharedDataUpdate(("enemyAtom",new Vector(shared_data.enemyAtom.x , shared_data.enemyAtom.y - Math.Sign(delta_y) )));
                }
            }
            
        }
    }

    private Vector ReadSnakeBody()
    {
        Vector[] array = new Vector[shared_data.snake.Count];
        shared_data.snake.CopyTo(array, 0);
        return array[shared_data.snake.Count - 1 - (int)(shared_data.snake.Count * ran.NextDouble() / 2 )];
    }
}