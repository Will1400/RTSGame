using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementValidator : MonoBehaviour
{
    private List<Collider> colliders;

    private void Start()
    {
        colliders = new List<Collider>();
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
}
