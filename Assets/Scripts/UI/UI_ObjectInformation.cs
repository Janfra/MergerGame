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
    private TextMeshProUGUI objectIntent;

    private void Awake()
    {
        
    }

    private void SetObjectName(string _objectName)
    {
        objectNameText.text = _objectName;
    }

    private void SetObjectIntent(string _objectIntent)
    {
        objectIntent.text = _objectIntent;
    }

    public void ToggleOpen()
    {
        bool isOpen = !objectInfoAnimator.GetBool("IsOpen");
        objectInfoAnimator.SetBool("IsOpen", isOpen);
    }
}
