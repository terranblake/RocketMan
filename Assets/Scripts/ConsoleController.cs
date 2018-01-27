using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    public Camera[] CameraArray;
    public Material ConsoleDisplayMaterial;
    public RenderTexture DisplayTexture;
    public Slider ThrustSlider;
    public Thrust ThrustScript;
    public Material InfoMaterial;
    public Text InfoText;

    private Camera _activeCamera;
    private int _activeCameraIndex = 0;
    private bool _appButtonInUse = false;
    private float lerpTime = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ControllerState();
        UpdateThrustSlider();

        if (GvrControllerInput.AppButton && !_appButtonInUse)
        {
            _appButtonInUse = true;
            SetConsoleCamera();
        }
        
        if (!GvrControllerInput.AppButton)
            _appButtonInUse = false;
    }

    void ControllerState(){
        if(GvrControllerInput.State == GvrConnectionState.Disconnected){
            //InfoMaterial.color = Color.yellow;
            InfoMaterial.color = Color.Lerp(InfoMaterial.color, Color.yellow, lerpTime);

            InfoText.color = Color.red;
            InfoText.text = "Warning !\nYour DayDream Controller is not connected.\nWarning !";

            if (lerpTime < 1){ // while t below the end limit...
                // increment it at the desired rate every update:
                lerpTime += Time.deltaTime/5.0f;
            }
        }
        else if(InfoMaterial.color != Color.green){
            //InfoMaterial.color = Color.green;
            InfoMaterial.color = Color.Lerp(InfoMaterial.color, Color.green, lerpTime);

            InfoText.color = Color.black;
            InfoText.text = "\n\nResume Normal Flight Procedures\n\n";

            if (lerpTime < 1){ // while t below the end limit...
            // increment it at the desired rate every update:
                lerpTime += Time.deltaTime/5.0f;
            }
        }
        else
            lerpTime = 0;
    }

    void SetConsoleCamera()
    {
        ConsoleDisplayMaterial.mainTexture = DisplayTexture;

        if (_activeCameraIndex == CameraArray.Length)
        {
            _activeCameraIndex = 0;
        }
        _activeCameraIndex++;

        _activeCamera = CameraArray[_activeCameraIndex - 1];
        _activeCamera.targetTexture = null;

        for (int x = 0; x < CameraArray.Length; x++)
        {
            if (CameraArray[x] != _activeCamera)
                CameraArray[x].enabled = false;
            else
                CameraArray[x].enabled = true;
        }

        print(_activeCamera);
        print(_activeCameraIndex);

        _activeCamera.aspect = 0.195f / 0.1f;
        _activeCamera.targetTexture = DisplayTexture;
    }

    void UpdateThrustSlider(){
        ThrustSlider.value = ThrustScript._mainThrust / 23;
    }
}