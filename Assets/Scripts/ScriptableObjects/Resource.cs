using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Resource")]
public class Resource : ScriptableObject {
    public string name;
    public string description;
    public int harvestAmount;
    public int harvestDifficulty;
}