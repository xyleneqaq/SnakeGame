using System;

namespace SnakeGame
{
    class Program
    {
        public static void Main()
        {
            // UI启动
            UiSystem ui = new UiSystem();
            ui.Menu();
            
            // 清除所有订阅！
            GameEvents.ClearAllSubscriptions();
        }
    }
    
}