using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public GameObject obj;
    public bool isColliding = false;

    void Start()
    {
        obj = null;
    }

    void OnTriggerEnter(Collider collider)
    {
        obj = collider.gameObject;
        UpdateColliding();
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == obj) {
            obj = null;
        }
        UpdateColliding();
    }

    void UpdateColliding()
    {
        isColliding = obj != null;
    }
}
