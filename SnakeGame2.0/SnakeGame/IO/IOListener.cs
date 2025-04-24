// 采用Window自带的API实现键盘输入监听！
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using SnakeGame;

public class NativeKeyboard
{
    private readonly Stack<int> _keyStack = new Stack<int>();
    private readonly System.Timers.Timer _inputTimer;
    private readonly ManualResetEventSlim _signal = new ManualResetEventSlim(false);
    private const int BUFFER_DURATION = 200;
    private volatile int _finalKey = -1;

    public NativeKeyboard()
    {
        _inputTimer = new System.Timers.Timer(16); // 60Hz检测
        _inputTimer.Elapsed += DetectKeys;
        _inputTimer.AutoReset = true;

        // 挂事件：I/O数据流
        GameEvents.OnKeyboardInput += WaitForInput;
        GameEvents.OnKeyboardInput += KeyCommand;
    }

    // 主线程调用这个方法会阻塞直到输入完成，注意global数据要用引用传递！
    public void WaitForInput()
    {
        _keyStack.Clear();
        _finalKey = -1;
        _signal.Reset();
        
        _inputTimer.Start();
        
        // 关键点：设置单次触发的结束Timer
        var endTimer = new System.Timers.Timer(BUFFER_DURATION) { AutoReset = false };
        endTimer.Elapsed += ProcessInput;
        endTimer.Start();

        // 主线程在此阻塞，直到信号触发
        _signal.Wait();
        
        // 清空 Console 输入缓冲区，防止残留按键触发后续 ReadKey
        // 逻辑：Console.KeyAvailable表示有按键残留
        // Console.ReadKey(true)：不断吐出键(ReadKey)，直到其没有键了(False)，结束循环！
        while (Console.KeyAvailable)
        {
            Console.ReadKey(true); // true = 不显示按键，非阻塞读取
        }
    }

    // 按键 - 指令 映射，输入 - 逻辑 解耦分离！
    // 实际上，如果采用反射，将会非常舒适。但是，我们贪吃蛇不提供键位修改功能，由此...！
    public void KeyCommand()
    {
        switch (_finalKey)
        {
            case 0x57: // W
                SharedData.SharedDataUpdate(("snakeCommandAction",StateSnakeAction.Up)); //数据上传
                break;
            case 0x41: // A
                SharedData.SharedDataUpdate(("snakeCommandAction",StateSnakeAction.Left)); //数据上传
                break;
            case 0x53: // S
                SharedData.SharedDataUpdate(("snakeCommandAction",StateSnakeAction.Down)); //数据上传
                break;
            case 0x44: // D
                SharedData.SharedDataUpdate(("snakeCommandAction",StateSnakeAction.Right)); //数据上传
                break;
            case 0x51: // Q
                SharedData.SharedDataUpdate(("systemCommand",StateSystem.Pause)); //数据上传
                break;
            case 0x1B: // ESC
                SharedData.SharedDataUpdate(("systemCommand",StateSystem.Return)); //数据上传
                break;
            case 0x20: // Space
                if (SharedData.globalData.snakeCommandLock) // 加上命令锁，保证只有在特定条件下触发
                {
                    break;
                }
                SharedData.SharedDataUpdate(("snakeCommandStatus",StateSnakeAttack.Attack)); //数据上传
                break;
        }
    }

    private void DetectKeys(object sender, ElapsedEventArgs e)
    {
        // 按键检测逻辑（示例检测WASD,暂停Q，退出X）
        DetectKey(0x57); // W
        DetectKey(0x41); // A
        DetectKey(0x53); // S
        DetectKey(0x44); // D
        DetectKey(0x51); // Q
        DetectKey(0x1B); // ESC
        DetectKey(0x20); // Space
    }

    private void DetectKey(int vKey)
    {
        if ((GetAsyncKeyState(vKey) & 0x8000) != 0)
        {
            if (_keyStack.Count == 0 || _keyStack.Peek() != vKey)
                _keyStack.Push(vKey);
        }
    }

    private void ProcessInput(object sender, ElapsedEventArgs e)
    {
        _inputTimer.Stop();
        _finalKey = _keyStack.Count > 0 ? _keyStack.Peek() : -1; //-1:常见的无按键空值！
        
        // 关键点：释放阻塞的主线程
        _signal.Set(); 
        
        ((System.Timers.Timer)sender).Dispose();
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
}