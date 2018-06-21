using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyFactory : MonoBehaviour
{
    public GameObject EnergyPrefab;
    private ProjectileLauncher _launcher;

    void Awake()
    {
        _launcher = new ProjectileLauncher();
    }

    public int Harvest(Vector3 from, Vector3 direction, float damage, float health, bool render)
    {
        int amount = 0;

        // If health will not be 0 after being damaged
        if (health - damage > 0)
            // Create resources with the value that will be subtracted from our health
            amount = (int)Mathf.Ceil(damage / 5.0f);
        else
            // Otherwise, create resources with our remaining health, which is always positive
            amount = (int)Mathf.Ceil(health / 5.0f);

        // Physically create the Energy object
        if (render == true)
            CreateEnergy(from, direction, amount);

        return amount;
    }

    void CreateEnergy(Vector3 from, Vector3 direction, int amount)
    {
		// Instantiate new Energy prefab and get a reference to its interaction component
        GameObject energy = Instantiate(EnergyPrefab, new Vector3(from.x, from.y, from.z), Quaternion.Euler(-90f, 0, 0));
        Interactable interact = energy.GetComponent<Interactable>();

		// If it has one, then create a new Energy ScriptableObject and add it to the object
        if (interact != null)
            interact.attributes = new Energy(amount);

		// Set the launcher projectile to the newly created Energy instance, then launch
        _launcher.ProjectileType = energy;
        _launcher.LaunchProjectile(from, direction, 5f, false);

		Debug.Log(string.Format("Created {0} Energy from {1}", amount, transform.name));
    }
}