using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public Material environmentMaterial;
    public Vector2 tilingConstraints;
    public float speed;
	public Vector2 relativeSpeed;

    private Vector2 _tilingValues;

    // Update is called once per frame
    void Update()
    {
        _tilingValues = environmentMaterial.mainTextureScale;
        if (_tilingValues.x < tilingConstraints.x || _tilingValues.y < tilingConstraints.y)
            environmentMaterial.mainTextureScale = new Vector2(_tilingValues.x + (speed / relativeSpeed.x), _tilingValues.y + (speed / relativeSpeed.y));
    }

    void OnApplicationQuit()
    {
        environmentMaterial.mainTextureScale = new Vector2(0, 0);
    }
}
