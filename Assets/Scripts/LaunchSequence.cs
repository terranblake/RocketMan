using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchSequence : MonoBehaviour
{
    public GameObject greenPad;
    public GameObject yellowPad;
    public GameObject redPad;

	// Use this for initialization
	void Start () {
	    LerpTransparency(redPad, 2.0f, true);
//	    LerpTransparency(redPad, 0.5f, false);
//	    LerpTransparency(yellowPad, 2.0f, true);
//	    LerpTransparency(yellowPad, 0.5f, false);
//	    LerpTransparency(greenPad, 2.0f, true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void LerpTransparency(GameObject currentPad, float duration1, bool activate)
    {
    }
}
