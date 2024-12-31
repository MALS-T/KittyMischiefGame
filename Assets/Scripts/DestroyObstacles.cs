using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObstacles : MonoBehaviour
{
    [SerializeField] Spawner spawnerScript;

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject parent = collider.transform.root.gameObject;
        if(parent != null)
        {
            spawnerScript.gameObjectsSpawned.Remove(parent);
            Destroy(parent);
        }
        spawnerScript.gameObjectsSpawned.Remove(collider.gameObject);
        Destroy(collider.gameObject);
    }
}
