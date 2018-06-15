using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float GrabRange = 1.5f;
    public bool IsDropped = false;
    public GameObject DroppedEffect;
    public float health = -1f;
    public GameObject destroyEffect;
    public ScriptableObject attributes;

    private bool _previousDroppedState = true;
    private GameObject _createdDropEffect;
    private bool _canBeDestroyed = true;
    private bool _hasRigidbody = false;

    void OnDrawGizmosSelected()
    {
        // Draw Sphere for debugging Interactable issues
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GrabRange);
    }

    void Awake()
    {
        // Set Indestructible and Interactable Layers before anything else runs
        if (health == -1)
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

        // Dropped effect should always contain a Particle System
        ParticleSystem droppedParticles = _createdDropEffect.GetComponent<ParticleSystem>();
        droppedParticles.Play();
    }

    void InInventory()
    {
        // If the item is in the player's inventory, then
        //  we want to destroy the existing effect
        if (_createdDropEffect != null)
            Destroy(_createdDropEffect);
    }

    void ProppingUpInteractable()
    {
        if (_hasRigidbody == true)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb.velocity == new Vector3(0, 0, 0))
            {
                Destroy(rb);
                ScaleColliderSize(gameObject.GetComponent<BoxCollider>(), 0.5f);

                _hasRigidbody = false;
            }
        }
        else
        {
            gameObject.AddComponent<Rigidbody>();
            ScaleColliderSize(gameObject.GetComponent<BoxCollider>(), 2.0f);

            _hasRigidbody = true;
        }
    }

    void ScaleColliderSize(BoxCollider collider, float newY)
    {
        collider.size = new Vector3(collider.size.x, collider.size.y * newY, collider.size.z);
    }

    public void TakeDamage(float amount)
    {
        // Interactable can be destroyed
        if (_canBeDestroyed)
        {
            // Decrement health
            health -= amount;
            Debug.Log(string.Format(
                "Removed {0} hp from {1}.\n{1} has {2} hp remaining.",
                amount,
                gameObject.name,
                health
            ));

            // Health is less than/equal to 0
            if (health <= 0)
                DestroyMe();
        }
    }

    void DestroyMe()
    {
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