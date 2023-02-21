using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event Colour", menuName = "Scriptable Objects/Event Colour")]
public class SO_EventHighlightColours : ScriptableObject
{
    [SerializeField]
    public Color eventColour;
}
