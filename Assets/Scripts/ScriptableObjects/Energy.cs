using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Energy", menuName = "Energy")]
public class Energy : ScriptableObject
{
    public string name = "RawEnergy";
    private int _energyAmount;
    private bool _isConsumed = false;

    public Energy(int amount)
    {
        if (amount > 0)
        {
            this._energyAmount = amount;
            this._isConsumed = false;
        }
        else
			throw new UnityException("Energy objects must have a value greater than 0.");
    }

    public int Consume()
    {
        Debug.Log("Consumed!");
		return _energyAmount;
    }
}
