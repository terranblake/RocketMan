using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera PlayerCamera;
    public Inventory inventory;

    private int _energy;

    void Awake()
    {
        Debug.Log(string.Format("Ignoring collision between <{0}> and <{1}>", LayerMask.LayerToName(8), LayerMask.LayerToName(9)));
        Physics.GetIgnoreLayerCollision(8, 9);
    }

    void Start()
    {
        // Initialize inventory selection
        //inventory.SelectItem();
    }

    void Update()
    {
        if (inventory == null)
        {
            Inventory temp = gameObject.GetComponentInChildren<Inventory>();

            if (temp != null)
                inventory = temp;
        }
        else
        {
            // Check for updates to inventory
            inventory.ItemSelection();

            // Add item to inventory
            if (Input.GetKeyDown(KeyCode.E))
            {
                CastForInteractables();
            }

            // Drop item from inventory
            if (Input.GetKeyDown(KeyCode.Q))
            {
                inventory.DropItem();
            }
        }
    }

    void CastForInteractables()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;

        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, 1.5f, layerMask))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (hit.transform.tag != "Environment")
                    inventory.SelectionFiltering(hit.transform);
            }
        }
    }
}