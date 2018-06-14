using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float Radius = 1.5f;
    public bool IsDropped = false;
    public GameObject DroppedEffect;
    public float health = -1f;
    public GameObject destroyPlantEffect;
    public ScriptableObject attributes;

    private bool _droppedState = true;
    private GameObject _createdDropEffect;
    private bool _canBeDestroyed = true;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    void Awake()
    {
        if (health == -1)
            _canBeDestroyed = false;
    }

    void Update()
    {
        if (!_canBeDestroyed)
        {
            _droppedState = IsDropped;

            if (transform.parent != null && transform.parent.name == "Inventory")
                IsDropped = false;
            else
                IsDropped = true;

            if (_droppedState != IsDropped)
                UpdateEffects();
        }

    }

    void UpdateEffects()
    {
        if (IsDropped == true)
        {
            Debug.Log(string.Format("Created drop effect on {0}", transform.name));

            // Create new dropped effect GameObject
            _createdDropEffect = Instantiate(DroppedEffect, transform);

            // Dropped effect should always contain a Particle System
            ParticleSystem droppedParticles = _createdDropEffect.GetComponent<ParticleSystem>();
            droppedParticles.Play();
        }
        else
        {
            if (_createdDropEffect != null)
                Destroy(_createdDropEffect);
        }
    }

    public void TakeDamage(float amount)
    {
        if (_canBeDestroyed)
        {
            health -= amount;
            Debug.Log(string.Format(gameObject.name + " has {0} hp left.", health));

            if (health <= 0)
                OnDestroy();
        }
    }

    void OnDestroy()
    {
        // Temporary environment destruction effect
        if (gameObject.transform.parent.name == "Environment")
        {
            GameObject dieEffect = Instantiate(destroyPlantEffect, gameObject.transform);
            dieEffect.gameObject.transform.parent = null;

            ParticleSystem.ShapeModule shapeModule = dieEffect.GetComponent<ParticleSystem>().shape;
            shapeModule.shapeType = ParticleSystemShapeType.MeshRenderer;
            shapeModule.meshRenderer = gameObject.GetComponent<MeshRenderer>();

            dieEffect.GetComponent<ParticleSystem>().Play();

            Destroy(dieEffect, 1f);
        }

        Destroy(gameObject, 1f);
    }
}