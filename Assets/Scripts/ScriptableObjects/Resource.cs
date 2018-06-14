using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Resources")]
public class Resource : ScriptableObject {
    public string name;
    public string description;
    public int harvestDifficulty;
}