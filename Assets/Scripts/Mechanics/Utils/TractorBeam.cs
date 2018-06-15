using System.Collections;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    public Camera tractorCam;
    public float speed;
    public Direction direction;

    public enum Direction { Normal, Reversed };

    void Update()
    {

        if (Input.GetKey(KeyCode.Z))
        {
            // Do a raycast
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f))
            {
                Debug.Log(hit.transform.name);
                // move towards the hit object
                if (direction == Direction.Normal)
                {
                    transform.position = Vector3.MoveTowards(transform.position, hit.transform.position, speed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, hit.transform.position, -1 * speed * Time.deltaTime);
                }

                // makes us look at the hit object
                transform.LookAt(hit.transform);
                print("I'm looking at " + hit.transform.name);
            }
            else
            {
                print("I'm looking at nothing!");
            }
            // you will have no hit.point if nothing was hit - use DrawRay instead    
            Debug.DrawRay(transform.position, transform.forward, Color.red, 1.0f);
            // but since the ray starts at the center of the viewport, you will    
            // only see a red dot anyway.
        }
    }
}