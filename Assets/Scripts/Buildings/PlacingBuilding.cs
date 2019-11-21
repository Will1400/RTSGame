using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacingBuilding : MonoBehaviour
{
    public List<Collider> Colliders;


    private void Start()
    {
        Colliders = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Building")
        {
            Colliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Building")
        {
            Colliders.Remove(other);
        }
    }
}
