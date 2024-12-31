using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") && collision.gameObject.tag == "Destructible")
        {
            GameObject parent = collision.transform.root.gameObject;
            if(parent != null)
            {
                Destroy(parent);
            }
            
            Destroy(collision.gameObject);
            
            gameManager.AddPoint(10);
            gameManager.AddBreakCount();

            Debug.Log("Destroyed Obstacle");
        }
    }
}
