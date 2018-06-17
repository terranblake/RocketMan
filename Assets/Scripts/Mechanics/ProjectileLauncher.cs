using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject ProjectileType;
    public bool isSticky = false;
    private bool _selfDestructs = false;
    public void LaunchProjectile(Vector3 fromHere, Vector3 rotateLike, float throwForce, bool selfDestructs)
    {
		GameObject projectile;

		if(ProjectileType.GetComponent<Grenade>() == null)
        	projectile = ProjectileType;
		else
			projectile = Instantiate(ProjectileType, fromHere, Quaternion.LookRotation(rotateLike));

        Debug.Log(string.Format("Launching {0} from position {1}", ProjectileType.name, fromHere));

        if (isSticky == true)
            projectile.AddComponent<Sticky>();

        projectile.GetComponent<Rigidbody>().AddForce(rotateLike * throwForce);

        if (selfDestructs == true)
            Destroy(projectile, 4f);
    }
}