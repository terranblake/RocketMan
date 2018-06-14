using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class Weapon : ScriptableObject {
    public string name;
    public string description;
    public int damage;
    public int maxAmmo;
    public float maxRange;
    public float impactForce;
    public float launchForce;
}