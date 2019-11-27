using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementValidator : MonoBehaviour
{
    private List<Collider> colliders;

    private Rigidbody rigidbody;
    bool isColliderTrigger;


    private void Start()
    {
        colliders = new List<Collider>();
        if (!TryGetComponent<Rigidbody>(out _))
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        isColliderTrigger = gameObject.GetComponent<Collider>().isTrigger;
        gameObject.GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Building")
        {
            colliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Building")
        {
            colliders.Remove(other);
        }
    }

    public bool IsValidPosition()
    {
        return colliders.Count == 0;
    }

    private void OnDestroy()
    {
        if (rigidbody != null)
        {
            Destroy(rigidbody);
        }

        if (!isColliderTrigger)
            gameObject.GetComponent<Collider>().isTrigger = false;
    }
}
