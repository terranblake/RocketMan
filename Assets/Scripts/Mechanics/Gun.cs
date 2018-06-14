using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public int maxAmmo = 10;
    public float range = 100f;
    public float fireRate = 15f;
    public float impactForce = 10f;
    public float launchForce = 1f;
    public Camera fpsCam;
    public GameObject[] muzzleFlashes;
    public GameObject muzzleFlashEffect;
    public GameObject impactEffect;
    public PulseEffect[] pulseEffects;
    public Animator animator;

    private float nextTimeToFire = 0f;
    private int currentAmmo = -1;
    private GrenadeThrower _grenadeThrower;
    private bool _isFiring = false;

    void Start()
    {
        if (currentAmmo == -1)
            currentAmmo = maxAmmo;
    }

    void OnEnable() {
        _grenadeThrower = gameObject.GetComponent<GrenadeThrower>();
        animator = gameObject.GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (_isFiring)
            return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && transform.parent)
        {
            if (currentAmmo >= 1)
            {
                nextTimeToFire = Time.time + 1f / fireRate;

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
            _grenadeThrower.ShootGrenade(muzzleFlashes[0].transform.position, muzzleFlashes[0].transform.forward, launchForce);
        // Otherwise, shoot normal bullet
        else
            ShootBullet();

        // Decrement ammo and log stats
        currentAmmo -= 1;
        Debug.Log(string.Format("{0} has {1} ammo left.", gameObject.name, currentAmmo));

        // Gun firing logic
        yield return new WaitForSeconds(1.0f / (fireRate * 4.0f));
        animator.SetBool("Firing", false);
        _isFiring = false;
    }

    void ShootBullet()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Interactable target = hit.transform.GetComponent<Interactable>();

            if (target != null)
                target.TakeDamage(damage);
            
            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
    }
}
