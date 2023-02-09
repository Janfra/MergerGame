using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ObjectInformation : MonoBehaviour
{
    public static event Func<GameObject, ICommand> OnRequestObjectCommand;

    [Header("References")]
    [SerializeField]
    private GameObject objectInfoContainer;
    [SerializeField]
    private Animator objectInfoAnimator;
    [SerializeField]
    private TextMeshProUGUI objectNameText;
    [SerializeField]
    private TextMeshProUGUI objectIntentText;

    private GameObject selectedObject;

    private void Awake()
    {
        CheckReferences();

        GameManager.OnSelectedObjectChange += GetSelectedObject;
        TurnManager.OnTurnUpdated += (context1, context2) => GetObjectIntent();
    }

    private void OnDestroy()
    {
        TurnManager.OnCommandCreated -= CheckCommand;
        TurnManager.OnTurnUpdated -= (context1, context2) => GetObjectIntent();
    }

    /// <summary>
    /// Check if any references are missing on awake.
    /// </summary>
    private void CheckReferences()
    {
        if(!objectInfoAnimator)
        {
            Debug.LogError($"Animator not set in {gameObject.name}");
            objectInfoAnimator = GetComponent<Animator>();
        }
        if (!objectNameText)
        {
            Debug.LogError($"Object name text not set in {gameObject.name}");
        }
        if (!objectIntentText)
        {
            Debug.LogError($"Object intent text not set in {gameObject.name}");
        }
    }

    /// <summary>
    /// Sets selected object and updates information.
    /// </summary>
    /// <param name="_selectedObject">New selected object</param>
    private void GetSelectedObject(PlaceableObject _selectedObject)
    {
        selectedObject = _selectedObject.gameObject;
        SetObjectName(_selectedObject.name);
        GetObjectIntent();
        TurnManager.OnCommandCreated += CheckCommand;
    }

    /// <summary>
    /// Checks if command is for selected object to update intent.
    /// </summary>
    /// <param name="_commandOwner">Object that created the command.</param>
    /// <param name="_command">Command created by owner.</param>
    private void CheckCommand(GameObject _commandOwner, ICommand _command)
    {
        if(_commandOwner == selectedObject)
        {
            SetObjectIntent(_command.ToString());
        }
    }

    /// <summary>
    /// Gets the selected object current intent.
    /// </summary>
    private void GetObjectIntent()
    {
        if(selectedObject != null)
        {
            ICommand objectIntent = OnRequestObjectCommand?.Invoke(selectedObject);
            if (objectIntent != null)
            {
                SetObjectIntent(objectIntent.ToString());
            }
            else
            {
                SetObjectIntent("No Action");
            }
        }
    }

    /// <summary>
    /// Sets the object name text.
    /// </summary>
    /// <param name="_objectName">New object text name.</param>
    private void SetObjectName(string _objectName)
    {
        objectNameText.text = _objectName;
    }

    /// <summary>
    /// Sets the intent text.
    /// </summary>
    /// <param name="_objectIntent">New intent text.</param>
    private void SetObjectIntent(string _objectIntent)
    {
        objectIntentText.text = _objectIntent;
    }

    /// <summary>
    /// Toggles animation for opening and closing UI.
    /// </summary>
    public void ToggleOpen()
    {
        bool isOpen = !objectInfoAnimator.GetBool("IsOpen");
        objectInfoAnimator.SetBool("IsOpen", isOpen);
        if (isOpen && selectedObject != null)
        {
            TurnManager.OnCommandCreated += CheckCommand;
        }
        else
        {
            TurnManager.OnCommandCreated += CheckCommand;
        }
    }
}
