using UnityEngine;

public class GrenadeThrower : MonoBehaviour {
	public GameObject AmmoType;
	public bool isSticky;
	public void ShootGrenade(Vector3 fromHere, Vector3 rotateLike, float throwForce){
		    GameObject grenade = Instantiate(AmmoType, fromHere, Quaternion.LookRotation(rotateLike));

			if(isSticky == true){
				grenade.AddComponent<Sticky>();
			}

            grenade.GetComponent<Rigidbody>().AddForce(rotateLike * throwForce);
			Destroy(grenade, 5f);
	}
}