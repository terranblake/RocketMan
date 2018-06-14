using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing", menuName = "Healing")]
public class Healing : ScriptableObject {
    public string name;
    public string description;
    public int healthBoost;
    public int maxHealth;
    public float consumptionTime;
}