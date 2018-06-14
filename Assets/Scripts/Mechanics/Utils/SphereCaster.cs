using UnityEngine;
using System.Collections;

public class SphereCaster : MonoBehaviour
{
    void Cast(float castingRadius, float castingDistance)
    {
        RaycastHit sphereHit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2f, Camera.main.pixelHeight / 2f, 0f));

        if (Physics.SphereCast(ray, castingRadius, out sphereHit, castingDistance))
        {
            Debug.Log(sphereHit.transform.name);
        }
    }
}