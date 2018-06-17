using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool DebuggingActive = false;
    public int maxItems = 5;
    public int maxEnergy = 999;
    public int selectedItem = 0;

    public int _energyCount = 0;

    public void AddItem(Transform Item)
    {
        bool checkForEnergy = IsItemEnergy(Item);
        if(checkForEnergy == true){
            Destroy(Item.gameObject);
            return;
        }

        // Check if inventory is full
        if (transform.childCount < maxItems)
        {
            // Set parent of new item to ItemHolder
            Item.parent = transform;

            // Set position and rotation to 0
            Item.localPosition = new Vector3(0, 0, 0);
            Item.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            Collider ItemCollider = Item.GetComponent<Collider>();
            if (ItemCollider != null)
            {
                ItemCollider.enabled = false;
            }

            // Disable if not the only item
            if (transform.childCount - 1 != 1)
                Item.gameObject.SetActive(false);

            SelectItem();

            if (DebuggingActive)
            {
                Debug.Log(string.Format("{0} added to {1}.", Item.name, transform.name));
                PrintInventory();
            }
        }
        else
        {
            Debug.Log("Inventory is full.");
        }
    }

    public int AddEnergy(int amount)
    {
        Debug.Log(string.Format("Adding {0} energy to player's {1}", amount, _energyCount));

        if (_energyCount == 999)
        {
            Debug.Log("Energy capacity reached.");
            return amount;
        }

        _energyCount = _energyCount + amount;
        if (_energyCount > maxEnergy)
        {
            _energyCount = 999;
            return (_energyCount + amount) - 999;
        }
        return 0;

    }

    public void DropItem()
    {
        // Check if inventory is empty
        if (transform.childCount != 0)
        {
            // TODO :: Handle case when user has already dropped the currently selected item
            // Find item
            Transform Item = transform.GetChild(selectedItem);

            // Flatten rotation of dropped item
            Item.eulerAngles = new Vector3(0, Item.eulerAngles.y, 0);

            Collider ItemCollider = Item.GetComponent<Collider>();
            if (ItemCollider != null)
            {
                ItemCollider.enabled = true;
            }

            // Set parent of new item to null
            Item.parent = null;

            // Re-select Item
            SelectItem();

            if (DebuggingActive)
                PrintInventory();
        }
        else
        {
            Debug.Log("Inventory is empty.");
        }
    }

    public void ItemSelection()
    {
        int previousSelectedItem = selectedItem;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            IncrementSelectedItem();

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            DecrementSelectedItem();

        if (previousSelectedItem != selectedItem)
            SelectItem();
    }

    public void SelectItem()
    {
        int i = 0;

        foreach (Transform item in transform)
        {
            if (i == selectedItem)
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);

            i++;
        }
    }

    public void PrintInventory()
    {
        string items = "";
        int i = 0;
        foreach (Transform item in transform)
        {
            items = items + string.Format("\n{0} at position {1}", item.name, i);
            i++;
        }

        Debug.Log(items);
    }

    private bool IsItemEnergy(Transform Item)
    {
        ScriptableObject temp = Item.GetComponent<Interactable>().attributes;
        if (temp.GetType() == typeof(Energy))
        {
            Energy toConsume = (Energy)temp;
            AddEnergy(toConsume.Consume());
            return true;
        }
        return false;

    }

    void IncrementSelectedItem()
    {
        if (selectedItem >= transform.childCount - 1)
            selectedItem = 0;
        else
            selectedItem++;
    }

    void DecrementSelectedItem()
    {
        if (selectedItem <= 0)
            selectedItem = transform.childCount - 1;
        else
            selectedItem--;
    }
}
