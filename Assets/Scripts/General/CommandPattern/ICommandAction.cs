using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandAction : ICommand
{
    public GameObject[] ObjectsAffected { get; set; }
}
