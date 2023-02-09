using System;

public interface ICommand
{
    public static event Action IsCompleted;

    /// <summary>
    /// Complete command and call event
    /// </summary>
    protected static void CommandCompleted()
    {
        IsCompleted?.Invoke();
    }

    /// <summary>
    /// Start command logic
    /// </summary>
    void Execute();

    /// <summary>
    /// Cancel this command
    /// </summary>
    void Undo();
}
