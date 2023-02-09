using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChainedCommand : ICommand
{
    /// <summary>
    /// Command dependent of the main command (chained command)
    /// </summary>
    ICommand ChainedCommand { get; set; }

    /// <summary>
    /// Get owner of the main command
    /// </summary>
    /// <returns>Owner set as key in dictionary</returns>
    GameObject GetOwner();
}
