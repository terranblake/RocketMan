using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrust : MonoBehaviour
{
    public Rigidbody vessel;
    public ParticleSystem.EmissionModule emissions;
    public ParticleSystem ThrustParticles;

    public float _mainThrust;
    private float _emissionsRate;
    private float _rotationalThrust;
    private bool _isLaunched = false;
    private Vector3 _emissionFlightPosition;
    private GDaydremControls _controllerInput;
    private Quaternion _upQuaternion;

    public float posThrustModifier = 1;
    public float negThrustModifier = 2;

    //TESTING
    private float timePoint = 0;

    private float timeDuration = 10f;

    // Use this for initialization
    void Start()
    {
        _upQuaternion = vessel.transform.rotation;

        ThrustParticles = vessel.GetComponentInChildren<ParticleSystem>();
        emissions = ThrustParticles.emission;
        _emissionFlightPosition = ThrustParticles.shape.position;
        emissions.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f);

        _controllerInput = new GDaydremControls();

        _rotationalThrust = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        Emissions();
    }

//    public void ThrustButtons(string button)
//    {
//        if (button == "up")
//            _mainThrust += 0.1f;
//        else if (button == "down")
//            _mainThrust -= 0.1f;
//    }

    void Emissions()
    {
        _emissionsRate = emissions.rateOverTime.constant;

        if (_emissionsRate < 1 && _mainThrust < 4)
        {
            emissions.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f);
            _isLaunched = false;
        }
        else if (_mainThrust < 6)
            emissions.rateOverTime = new ParticleSystem.MinMaxCurve(_emissionsRate - 0.01f);
        else
            emissions.rateOverTime = new ParticleSystem.MinMaxCurve(_mainThrust);

        if (_emissionsRate > 9.8 && _isLaunched == false)
        {
            Vector3 emissionPosition = new Vector3(2.8f, -0.07f, -0.28f);
            float pointInTime = 0.0f;
            float duration = 0.1f;
            ParticleSystem.ShapeModule shape = ThrustParticles.shape;
            float radius = shape.radius;

            while (pointInTime <= duration)
            {
                shape.radius = Mathf.Lerp(radius, 1, pointInTime / duration);
                shape.position = Vector3.Lerp(emissionPosition, _emissionFlightPosition, pointInTime / duration);
                pointInTime += Time.deltaTime;
            }
        }
        else if (_emissionsRate < 9.8 && vessel.transform.localPosition.y < 10.0f)
        {
            Vector3 emissionPosition = new Vector3(2.8f, -0.07f, -0.28f);
            float pointInTime = 0.0f;
            float duration = 0.1f;
            ParticleSystem.ShapeModule shape = ThrustParticles.shape;
            float radius = shape.radius;

            while (pointInTime <= duration)
            {
                shape.radius = Mathf.Lerp(radius, 4, pointInTime / duration);
                shape.position = Vector3.Lerp(_emissionFlightPosition, emissionPosition, pointInTime / duration);
                pointInTime += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        Quaternion vesselOrientation = vessel.transform.rotation;

        if (GvrControllerInput.TouchPosCentered.y > 0)
            _mainThrust = GvrControllerInput.TouchPosCentered.y * 15;
        else
            _mainThrust = 0;

        if (_controllerInput.ThrustPowerControls)
        {
            if (_mainThrust / 15 < 0.1)
            {
                vessel.transform.rotation =
                    Quaternion.Slerp(vessel.transform.rotation, _upQuaternion, Time.deltaTime * 3.0f);
                vessel.angularVelocity = new Vector3(0, 0, 0);
            }
            else
            {
                Quaternion controllerOrientation = GvrControllerInput.Orientation;
                vessel.transform.rotation = Quaternion.Slerp(vesselOrientation, controllerOrientation,
                    Time.deltaTime * (3.0f * _mainThrust / 15));
            }

            _rotationalThrust = GvrControllerInput.TouchPosCentered.x;

            if (_controllerInput.ThrustPowerControls)
                vessel.AddRelativeForce(Vector3.forward * _mainThrust);
        }
        else
        {
            vessel.transform.rotation =
                Quaternion.Slerp(vessel.transform.rotation, _upQuaternion, Time.deltaTime * 3.0f);
            vessel.angularVelocity = new Vector3(0, 0, 0);
        }

        //vessel.transform.Rotate(Vector3.back, _rotationalThrust);
        //print(GvrControllerInput.Orientation.eulerAngles);

        
    }
}