using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHandler : MonoBehaviour
{
    public int maxAmmo = 999;
    public GameObject[] muzzleFlashes;
    public GameObject muzzleFlashEffect;
    public GameObject impactEffect;
    public PulseEffect[] pulseEffects;

    private Camera _playerCamera;
    private float nextTimeToFire = 0f;
    private int currentAmmo = -1;
    private ProjectileLauncher _grenadeThrower;
    private bool _isFiring = false;
    private Weapon _stats;

    void Start()
    {
        if (currentAmmo == -1)
            currentAmmo = maxAmmo;

        // currentAmmo = _stats.clipSize
    }

    void OnEnable()
    {
        _grenadeThrower = gameObject.GetComponent<ProjectileLauncher>();
    }

    void Update()
    {
        if (_playerCamera == null && transform.parent != null && transform.parent.GetComponent<Inventory>() != null)
        {
            _playerCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            _stats = (Weapon)GetComponent<Interactable>().attributes;

            Debug.Log(_stats.PrintWeapon());
        }

        if (_isFiring)
            return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && transform.parent != null)
        {
            if (currentAmmo >= 1)
            {
                nextTimeToFire = Time.time + 1f / _stats.fireRate;

                foreach (PulseEffect effect in pulseEffects)
                {
                    StartCoroutine(effect.Pulse());
                }
                StartCoroutine(HandleTrigger());
            }
            else
            {
                Debug.Log(string.Format("<{0}> is out of ammo!", gameObject.name));
            }
        }
    }

    IEnumerator HandleTrigger()
    {
        // Gun firing logic
        Debug.Log(string.Format("{0} triggered.", transform.name));
        _isFiring = true;

        Animator animator = GetComponentInParent<Animator>();
        animator.SetBool("Firing", true);

        // Loop through all muzzle flash locations
        foreach (GameObject location in muzzleFlashes)
        {
            // Instantiate new effect object and set parent
            GameObject muzzleFlash = Instantiate(muzzleFlashEffect, location.transform.position, Quaternion.LookRotation(location.transform.forward));
            muzzleFlash.transform.SetParent(gameObject.transform);

            // Start associated particle system
            muzzleFlash.GetComponent<ParticleSystem>().Play();

            // Destroy after half second
            Destroy(muzzleFlash, 0.5f);
        }

        // If weapon has grenade throwing abilities
        if (_grenadeThrower != null)
            _grenadeThrower.LaunchProjectile(muzzleFlashes[0].transform.position, muzzleFlashes[0].transform.forward, _stats.launchForce, true);
        // Otherwise, shoot normal bullet
        else
            ShootBullet();

        // Decrement ammo and log stats
        currentAmmo -= 1;
        Debug.Log(string.Format("{0} has {1} ammo left.", gameObject.name, currentAmmo));

        // Gun firing logic
        yield return new WaitForSeconds(1.0f / (_stats.fireRate * 8.0f));

        animator.SetBool("Firing", false);
        _isFiring = false;
    }

    void ShootBullet()
    {
        float range = _stats.range;
        if (range == -1)
            range = 10 * 10 ^ 10;

        RaycastHit hit;
        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, range))
        {
            Interactable target = hit.transform.GetComponent<Interactable>();
            EnergyFactory factory = hit.transform.GetComponent<EnergyFactory>();

            Debug.Log(string.Format(
                "{0} hit at location {1} local, and {2} global",
                hit.transform.name, hit.transform.localPosition,
                hit.transform.position
            ));

            Debug.Log(string.Format(
                "HIT NORMAL\t{0}",
                hit.normal
            ));

            if (target != null)
            {
                // If this Interactable has an energy factory and we are using a harvesting tool
                if (factory != null && transform.name == "Harvesting")
                {
                    // Since we are harvesting, we want to increment our Energy variable in our inventory
                    int energyToAdd = factory.Harvest(hit.transform.position, hit.normal, _stats.damage, target.GetHealth(), false);
                    int overflowEnergy = transform.parent.GetComponent<Inventory>().AddEnergy(energyToAdd);

                    if (overflowEnergy > 0)
                        factory.Harvest(hit.transform.position, hit.normal, overflowEnergy * 5, overflowEnergy * 5, true);
                }

                target.TakeDamage(_stats.damage);
            }

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * _stats.impactForce);

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
    }
}
