using System;
namespace SnakeGame;

public interface ITools
{
    Map DeepCopy(Map obj); //深拷贝接口！
    Stack<Vector> DeepCopy(Queue<Vector> obj);
}