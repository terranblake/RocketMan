using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleController : MonoBehaviour
{
    public Camera[] CameraArray;
    public Material ConsoleDisplayMaterial;
    public RenderTexture DisplayTexture;

    private Camera _activeCamera;
    private int _activeCameraIndex = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GvrControllerInput.AppButton || Input.GetKeyDown(KeyCode.S))
        {
            SetConsoleCamera();
        }
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
}