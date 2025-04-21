namespace SnakeGame;

/// <summary>
/// 地图类
/// </summary>
public class Map
{
    public enum MapIndicator
    {
        Obstacle,
        Free,
        Bonus,
        Snake,
    };

    public enum MapFlag
    {
        Valid,
        Occupied,
    }

    public int map_length;
    public int map_width;
    public Dictionary<Vector,MapIndicator> MapCursor;
    public Dictionary<Vector,MapFlag> MapFlagCursor;

    public Map(int mapLength, int mapWidth)
    {
        this.map_length = mapLength;
        this.map_width = mapWidth;
        MapCursor = new Dictionary<Vector,MapIndicator>();
        MapFlagCursor = new Dictionary<Vector,MapFlag>();

        // 地图占有情况初始化
        for (int i = 0; i < map_length; i++)
        {
            for (int j = 0; j < map_width; j++)
            {
                MapFlagCursor.Add(new Vector(i, j),MapFlag.Valid);
            }
        }
        
        // 地图初始化
        for (int i = 0; i < map_length; i++)
        {
            for (int j = 0; j < map_width; j++)
            {
                MapCursor.Add(new Vector(i, j),MapIndicator.Free);
            }
        }
        
        //挂事件
        GameEvents.OnMapUpdate += MapElementSwitch;
    }

    public void InitializeObstacleMap()
    {
        for (int i = 0; i < map_length; i++)
        {
            for (int j = 0; j < map_width; j++)
            {
                if (i == 0 | i == map_length - 1 | j == 0 | j == map_width - 1 & MapFlagCursor[new Vector(i,j)] != MapFlag.Occupied)
                {
                    MapCursor[new Vector(i,j)] = MapIndicator.Obstacle;
                    MapFlagCursor[new Vector(i,j)] = MapFlag.Occupied;
                }
            }
        }
    }
    
    public void InitializeBonusMap(double possibility)
    {
        // 地图：Obstacle更新
        for (int i = 0; i < map_length; i++)
        {
            for (int j = 0; j < map_width; j++)
            {
                Random ran = new Random();
                double flag = ran.NextDouble();
                if (flag < possibility & MapFlagCursor[new Vector(i,j)] != MapFlag.Occupied)
                {
                    MapCursor[new Vector(i,j)] = MapIndicator.Bonus;
                    MapFlagCursor[new Vector(i,j)] = MapFlag.Occupied;
                }
            }
        }
    }
    
    public void InitializeSnakeMap(Vector vec)
    {
        // 地图：Obstacle更新
        for (int i = 0; i < map_length; i++)
        {
            for (int j = 0; j < map_width; j++)
            {
                if (new Vector(i,j) == vec & MapFlagCursor[new Vector(i,j)] != MapFlag.Occupied)
                {
                    MapCursor[new Vector(i,j)] = MapIndicator.Free; //在图中不显示，方便更新！
                    MapFlagCursor[new Vector(i,j)] = MapFlag.Occupied;
                }
            }
        }
    }

    public void MapElementSwitch(SharedData sharedData,Vector vector,MapIndicator indicator)
    {
        sharedData.map.MapCursor[vector] = indicator;
        if (indicator == MapIndicator.Free)
        {
            sharedData.map.MapFlagCursor[vector] = MapFlag.Valid;
        }
        else
        {
            sharedData.map.MapFlagCursor[vector] = MapFlag.Occupied;
        }
        GlobalData.GlobalUpdate(("GlobalMap",sharedData.map));
    }
}





