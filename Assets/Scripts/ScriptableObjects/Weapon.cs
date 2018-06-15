using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject {
    public string name;
    public string description;
    public int damage;
    public int clipSize;
    public float range;
    public float fireRate;
    public float reloadRate;
    public float impactForce;
    public float launchForce;

    public string PrintWeapon() {
        return (
            "Name:\t\t" + name +
            "\nDescription:\t" + description +
            "\nDamage:\t\t" + damage + 
            "\nClip Size:\t\t" + clipSize +
            "\nRange:\t\t" + range +
            "\nFire Rate:\t\t" + fireRate +
            "\nReload Rate:\t" + reloadRate +
            "\nImpact Force:\t" + impactForce + 
            "\nLaunch Force:\t" + launchForce 
        );
    }
}