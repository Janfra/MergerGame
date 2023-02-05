using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChainedCommand : ICommand
{
    ICommand ChainedCommand { get; set; }
    GameObject GetOwner();
}
