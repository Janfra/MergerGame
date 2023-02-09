using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<PlaceableObject> OnSelectedObjectChange;
    private static PlaceableObject selectedObject;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public static void SetSelectedObject(PlaceableObject _selectedObject)
    {
        selectedObject = _selectedObject;
        OnSelectedObjectChange?.Invoke(selectedObject);
    }

    enum GameStates
    {

    }
}
