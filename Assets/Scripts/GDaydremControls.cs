using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDaydremControls : MonoBehaviour
{
    public float ThrustControlsModifier = 0;
    public Boolean ThrustPowerControls = true;
    public Quaternion OrientationModifier;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		HandleDaydreamInput();
        HandleDaydreamOrientation();
	}

    // Button and Touch-Pad Controls
    private void HandleDaydreamInput()
    {
        if (GvrControllerInput.TouchDown)
        {
            ThrustControlsModifier = GvrControllerInput.TouchPosCentered.y * 25;
        }
//        else if (GvrControllerInput.AppButton)
//        {
//            ThrustPowerControls = !ThrustPowerControls;
//        }
    }

    //Orientation of Daydream Controller
    private void HandleDaydreamOrientation()
    {
        OrientationModifier = GvrControllerInput.Orientation;
    }
}
