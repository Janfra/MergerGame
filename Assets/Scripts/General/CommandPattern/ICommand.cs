using UnityEngine;

public interface ICommand
{
    public static bool IsCompleted;

    void Execute();

    void Undo();
}
