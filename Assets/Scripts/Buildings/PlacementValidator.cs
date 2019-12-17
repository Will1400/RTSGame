using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementValidator : MonoBehaviour
{
    [SerializeField]
    private List<Collider> collidesWith;

    private Rigidbody rigidbody;
    bool isColliderTrigger;
    private Collider collider;

    private void Start()
    {
        collidesWith = new List<Collider>();
        if (!TryGetComponent<Rigidbody>(out _))
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
        }
        if (TryGetComponent(out Collider _collider))
        {
            collider = _collider;
        }
        else
        {
            collider = GetComponentInChildren<Collider>();
        }

        isColliderTrigger = collider.isTrigger;
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Building")
        {
            collidesWith.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Building")
        {
            collidesWith.Remove(other);
        }
    }

    public bool IsValidPosition()
    {
        return collidesWith.Count == 0;
    }

    private void OnDestroy()
    {
        if (rigidbody != null)
        {
            Destroy(rigidbody);
        }

        if (!isColliderTrigger && collider != null)
        {
            collider.isTrigger = false;
        }
    }
}
