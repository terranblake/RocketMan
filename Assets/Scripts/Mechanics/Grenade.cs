using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
	public float delay = 3f;
	public float blastRadius = 5f;
	public float explosionForce = 700f;
	public float explosionDamage = 100f;
	public GameObject explosionEffect;
	private float _countdown;
	private bool _hasExploded = false;

	// Use this for initialization
	void Start () {
		_countdown = delay;
	}
	
	// Update is called once per frame
	void Update () {
		_countdown -= Time.deltaTime;
		if(_countdown <= 0f && !_hasExploded) {
			Explode();
			_hasExploded = true;
		}
	}

	void Explode() {
		GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
		Destroy(explosion, 2f);

		Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

		foreach(Collider nearbyObj in colliders){
			Rigidbody rb = nearbyObj.GetComponent<Rigidbody>();
			
			if(rb != null)
				rb.AddExplosionForce(explosionForce, transform.position, blastRadius);

			Interactable target = nearbyObj.transform.GetComponent<Interactable>();
            if (target != null)
                target.TakeDamage(explosionDamage);
		}
		
	}
}
