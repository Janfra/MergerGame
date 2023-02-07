using System;

public interface ICommand
{
    public static event Action IsCompleted;

    static void CommandCompleted()
    {
        IsCompleted?.Invoke();
    }

    void Execute();

    void Undo();
}
