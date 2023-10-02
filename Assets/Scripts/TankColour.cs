using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TankColour : MonoBehaviour
{
    public static TankColour _instance;
    public int colourSelectNumber;

    private void Awake()
    {
        _instance = this;
    }

    public void ColourPick(int val)
    {
        colourSelectNumber = val;
    }

}
