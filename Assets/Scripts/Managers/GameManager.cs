using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static PlayerPlacement selectedObject;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    enum GameStates
    {

    }
}
