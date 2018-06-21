using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool DebuggingActive = false;
    public int maxItems = 5;
    public int maxEnergy = 999;
    public int selectedItem = 0;
    public int _energyCount = 0;

    private NetworkedActions _networkActions;
    private Vector3 _staticPosition = new Vector3(-0.501f, 0.466f, 0.619f);
    private GameObject _mainCamera;

    void Awake()
    {
        //_mainCamera = GameObject.Find("MainCamera");
        GameObject go = GameObject.Find("NetworkActions");
        _networkActions = go.GetComponent<NetworkedActions>();
    }

    void Update() {
        //transform.position = _staticPosition;
        //transform.rotation = _mainCamera.transform.rotation;
    }

    void Start()
    {
    }

    public void SelectionFiltering(Transform Item)
    {
        ScriptableObject checkForEnergy = IsEnergy(Item);

        if (checkForEnergy != null)
        {
            Energy toConsume = (Energy)checkForEnergy;
            AddEnergy(toConsume.Consume());

            _networkActions.DestroyGameObject(Item.GetComponent<PhotonView>().viewID, 0.5f);
        }
        else
        {
            AddItem(Item);
        }
    }

    public void AddItem(Transform Item)
    {
        // Check if inventory is full
        if (transform.childCount < maxItems)
        {
            Transform updatedTransform = Item;

            // Set parent of new item to ItemHolder
            //Item.parent = transform;
            //updatedTransform.parent = transform;

            // Set position and rotation to 0
            //Item.localPosition = new Vector3(0, 0, 0);
            //Item.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            //updatedTransform.localPosition = new Vector3(0, 0, 0);
            //updatedTransform.eulerAngles = new Vector3(0, 0, 0);

            int itemID = Item.GetComponent<PhotonView>().viewID;
            int inventoryID = GetComponent<PhotonView>().viewID;

            Debug.Log(string.Format("Item:\t{0}", itemID));
            Debug.Log(string.Format("Inventory:\t{0}", inventoryID));

            _networkActions.UpdateTransform(itemID, inventoryID, Vector3.zero, Vector3.zero);

            Collider ItemCollider = Item.GetComponent<Collider>();
            if (ItemCollider != null)
            {
                ItemCollider.enabled = false;
                _networkActions.UpdateCollider(itemID, ItemCollider);
            }

            // Disable if not the only item
            // if (transform.childCount - 1 != 1)
            //     Item.gameObject.SetActive(false);
            if (transform.childCount - 1 != 1)
            {
                _networkActions.SetGameObjectState(itemID, false);
            }

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

    public void DropItem()
    {
        // Check if inventory is empty
        if (transform.childCount != 0)
        {
            // TODO :: Handle case when user has already dropped the currently selected item
            // Find item
            Transform Item = transform.GetChild(selectedItem);
            //Transform updatedTransform = Item;

            // Flatten rotation of dropped item
            //Item.eulerAngles = new Vector3(0, Item.eulerAngles.y, 0);
            //updatedTransform.eulerAngles = new Vector3(0, Item.eulerAngles.y, 0);

            // Set parent of new item to null
            //Item.parent = null;
            //updatedTransform.parent = null;

            int itemID = Item.GetComponent<PhotonView>().viewID;
            _networkActions.UpdateTransform(itemID, -1, transform.position, new Vector3(0, Item.eulerAngles.y, 0));

            Collider ItemCollider = Item.GetComponent<Collider>();
            if (ItemCollider != null)
            {
                ItemCollider.enabled = true;
                _networkActions.UpdateCollider(itemID, ItemCollider.enabled);
            }

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

        foreach (Transform Item in transform)
        {
            bool isActive;

            if (i == selectedItem)
                isActive = true;
            else
                isActive = false;

            int itemID = Item.GetComponent<PhotonView>().viewID;
            _networkActions.SetGameObjectState(itemID, isActive);

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

    private ScriptableObject IsEnergy(Transform Item)
    {
        ScriptableObject temp = Item.GetComponent<Interactable>().attributes;
        if (temp.GetType() == typeof(Energy))
            return temp;

        return null;
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
