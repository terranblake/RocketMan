using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public GameObject item;
    public GameObject effect;
	public Vector3 colliderCenter;
	public Vector3 colliderSize;

    // Use this for initialization
    void Start()
    {
        // Spawn and scale item
        GameObject itemDropped = Instantiate(item, transform);
        itemDropped.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Spawn and position particle effect
        GameObject partEffect = Instantiate(effect, transform);
        partEffect.transform.Translate(0, -0.164f, 0);

		Bounds colliderBounds = CalculateLocalBounds();

		// Add proportional collider for click detection
        BoxCollider collider = itemDropped.gameObject.AddComponent<BoxCollider>();
        collider.center = colliderBounds.center * 10;
        collider.size = colliderBounds.extents * 20;

    }

	private Bounds CalculateLocalBounds()
     {
         Quaternion currentRotation = this.transform.rotation;
         this.transform.rotation = Quaternion.Euler(0f,0f,0f);
 
         Bounds bounds = new Bounds(this.transform.position, Vector3.zero);
 
         foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
         {
             bounds.Encapsulate(renderer.bounds);
         }
 
         Vector3 localCenter = bounds.center - this.transform.position;
         bounds.center = localCenter;
         Debug.Log("The local bounds of this model is " + bounds);
 
         this.transform.rotation = currentRotation;

		 return bounds;
     }

    // Update is called once per frame
    void Update()
    {

    }
}
