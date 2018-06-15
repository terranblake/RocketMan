using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform Head;
    public Transform Body;
    public Transform ItemHolder;
    public Camera PlayerCamera;
    public Inventory inventory;

    void Awake()
    {
        Debug.Log(string.Format("Ignoring collision between <{0}> and <{1}>", LayerMask.LayerToName(8), LayerMask.LayerToName(9)));
        Debug.Log(Physics.GetIgnoreLayerCollision(8, 9));
    }

    void Start()
    {
        // Initialize inventory selection
        inventory.SelectItem();
    }

    void Update()
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

    void CastForInteractables()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;

        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, 1.5f, layerMask))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            // Check type of interactable

            if (interactable != null)
            {
                inventory.AddItem(hit.transform);
            }
        }
    }
}