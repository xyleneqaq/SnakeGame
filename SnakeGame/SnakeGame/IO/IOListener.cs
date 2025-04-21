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
    private const int BUFFER_DURATION = 500;
    private volatile int _finalKey = -1;

    public NativeKeyboard()
    {
        _inputTimer = new System.Timers.Timer(16); // 60Hz检测
        _inputTimer.Elapsed += DetectKeys;
        _inputTimer.AutoReset = true;

        // 挂事件：I/O数据流
        GameEvents.OnKeyboardInput += WaitForInput;
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
        
        GlobalData.GlobalUpdate(("GlobalKey",_finalKey)); //数据上传
    }

    private void DetectKeys(object sender, ElapsedEventArgs e)
    {
        // 你的按键检测逻辑（示例检测WASD,以及ESC）
        DetectKey(0x57); // W
        DetectKey(0x41); // A
        DetectKey(0x53); // S
        DetectKey(0x44); // D
        DetectKey(0x51); // Q
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
        _finalKey = _keyStack.Count > 0 ? _keyStack.Peek() : -1;
        
        // 关键点：释放阻塞的主线程
        _signal.Set(); 
        
        ((System.Timers.Timer)sender).Dispose();
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
}