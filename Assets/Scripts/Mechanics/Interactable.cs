using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float GrabRange = 1.5f;
    public bool IsDropped = false;
    public GameObject DroppedEffect;
    [Range(0.01f, 0.1f)]
    public float dropOutlineSize;
    public Color dropColor;
    public GameObject destroyEffect;
    public ScriptableObject attributes;

    private bool _previousDroppedState = true;
    private GameObject _createdDropEffect;
    public bool _canBeDestroyed = true;
    private bool _hasRigidbody = false;
    private float _health = -1f;
    private Material _defaultMaterial;
    private Material _outlineMaterial;

    void OnDrawGizmosSelected()
    {
        // Draw Sphere for debugging Interactable issues
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GrabRange);
    }

    void Awake()
    {
        dropColor = new Color(dropColor.r, dropColor.g, dropColor.b, 1.0f);

        if (GetComponent<EnergyFactory>() != null)
        {
            Resource stats = (Resource)attributes;
            _health = stats.harvestAmount;
        }

        // Set Indestructible and Interactable Layers before anything else runs
        if (_health == -1)
            _canBeDestroyed = false;
    }

    void OnCollisionEnter(Collision collider)
    {
        // Add this Interactable's collider to the list ignored collisions
        //  if this object is indestructible
        if (collider.gameObject.tag == "MainCamera" && _canBeDestroyed == false)
        {
            Physics.IgnoreCollision(collider.collider, gameObject.GetComponent<Collider>());
        }
    }

    void Update()
    {
        if (GetComponent<EnergyFactory>() != null)
            return;

        // If a rigidbody exists, we are in the process of propping it up
        if (_hasRigidbody == true)
            ProppingUpInteractable();

        if (!_canBeDestroyed)
        {
            // Store previous dropped state
            _previousDroppedState = IsDropped;

            // Interactable is in a player's Inventory if parent has Inventory component
            if (transform.parent != null && transform.parent.GetComponent<Inventory>() == true)
                IsDropped = false;
            // Otherwise, it is not in a player's inventory
            else
                IsDropped = true;

            // If dropped state of intreractable has changed
            if (_previousDroppedState != IsDropped)
                UpdateEffects();
        }

    }

    void UpdateEffects()
    {
        // Methods for handling effect changes based on whether
        //      this Interactable is in a player's inventory

        if (IsDropped == true)
        {
            ProppingUpInteractable();
            NotInInventory();
        }
        else
            InInventory();
    }

    void NotInInventory()
    {
        // Create new dropped effect GameObject
        _createdDropEffect = Instantiate(DroppedEffect, transform);
        Debug.Log(string.Format("Created drop effect on {0}", transform.name));

        // TODO :: Dependent on Interactables having a child GameObject with the same name as its parent
        // Change shader to outline
        UpdateAllMaterials(GameObject.Find(transform.name).GetComponent<Renderer>(), "Standard", "Outlined/Diffuse");

        // Dropped effect should always contain a Particle System
        ParticleSystem droppedParticles = _createdDropEffect.GetComponent<ParticleSystem>();
        
        // Allow for editting of particle system's color
        ParticleSystem.MainModule mainModule = droppedParticles.main;
        mainModule.startColor = new Color(dropColor.r, dropColor.g, dropColor.b, 63);

        droppedParticles.Play();
    }

    void InInventory()
    {
        // TODO :: Dependent on Interactables having a child GameObject with the same name as its parent
        // Change shader to outline
        UpdateAllMaterials(GameObject.Find(transform.name).GetComponent<Renderer>(), "Outlined/Diffuse", "Standard");

        // If the item is in the player's inventory, then
        //  we want to destroy the existing effect
        if (_createdDropEffect != null)
            Destroy(_createdDropEffect);
    }

    void UpdateAllMaterials(Renderer renderer, string from, string to)
    {
        // Loop through all materials (from -> to) in renderer
        Material[] updatedMaterials = renderer.materials;

        for (int x = 0; x < updatedMaterials.Length; x++)
        {
            Material material = new Material(updatedMaterials[x]);
            material.shader = Shader.Find(to);

            if (to == "Outlined/Diffuse")
            {
                material.SetColor("_OutlineColor", dropColor);
                material.SetFloat("_OutlineWidth", dropOutlineSize);
            }
            updatedMaterials[x] = material;

        }
        renderer.materials = updatedMaterials;

    }

    void ProppingUpInteractable()
    {
        if (_hasRigidbody == true)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb.velocity == new Vector3(0, 0, 0))
            {
                Destroy(rb);
                ScaleColliderSize(gameObject.GetComponent<BoxCollider>(), 0.05f, 0.25f, 1.0f);

                _hasRigidbody = false;
            }
        }
        else
        {
            gameObject.AddComponent<Rigidbody>();
            ScaleColliderSize(gameObject.GetComponent<BoxCollider>(), 20.0f, 4.0f, 1.0f);

            _hasRigidbody = true;
        }
    }

    void ScaleColliderSize(BoxCollider collider, float multX, float multY, float multZ)
    {
        collider.size = new Vector3(collider.size.x * multX, collider.size.y * multY, collider.size.z * multZ);
    }

    public void TakeDamage(float amount)
    {
        // Interactable can be destroyed
        if (_canBeDestroyed && _health > 0)
        {
            // Decrement health
            _health -= amount;
            Debug.Log(string.Format(
                "Removed {0} hp from {1}.\n{1} has {2} hp remaining.",
                amount,
                gameObject.name,
                _health
            ));

            // _health is less than/equal to 0
            if (_health <= 0)
                DestroyMe();
        }
    }

    void DestroyMe()
    {
        GetComponent<Collider>().enabled = false;

        // Temporary environment destruction effect
        if (gameObject.transform.parent.name == "Environment")
        {
            // Instantiate new destruction effect
            GameObject dieEffect = Instantiate(destroyEffect, gameObject.transform);
            dieEffect.gameObject.transform.parent = null;

            // Modify particle systems shape module
            ParticleSystem.ShapeModule shapeModule = dieEffect.GetComponent<ParticleSystem>().shape;
            shapeModule.shapeType = ParticleSystemShapeType.MeshRenderer;
            shapeModule.meshRenderer = gameObject.GetComponent<MeshRenderer>();

            // Start and destroy after 1 second
            dieEffect.GetComponent<ParticleSystem>().Play();
            Destroy(dieEffect, 1f);
        }

        // Destroy this Interactable instance
        Destroy(gameObject, 1f);
    }
}