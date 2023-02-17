using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static event Action<PlaceableObject, IUIInteractive> OnSelectedObjectChange;
    private static PlaceableObject selectedObject;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public static void SetSelectedObject(PlaceableObject _selectedObject)
    {
        selectedObject = _selectedObject;
        IUIInteractive selectedObjectInteraction = selectedObject.GetComponent<IUIInteractive>();
        OnSelectedObjectChange?.Invoke(selectedObject, selectedObjectInteraction);
    }

    public static void SetSelectedObject(PlaceableObject _selectedObject, IUIInteractive _interaction)
    {
        selectedObject = _selectedObject;
        OnSelectedObjectChange?.Invoke(selectedObject, _interaction);
    }

    enum GameStates
    {

    }
}
