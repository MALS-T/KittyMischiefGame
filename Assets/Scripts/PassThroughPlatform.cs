using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughPlatform : MonoBehaviour
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private bool playerOnPlatform;

    // Start is called before the first frame update
    void Start()
    {
        //get collider component that u want to enable/disable
        playerCollider = GetComponent<Collider2D>(); //*ensure PlayerCollider object is above ObstacleCollider coz it returns first match
    }

    // Update is called once per frame
    void Update()
    {
        if(playerOnPlatform && Input.GetKey(KeyCode.S))
        {
            playerCollider.enabled = false;
            StartCoroutine(EnableCollider());
        }
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.3f);
        playerCollider.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerOnPlatform = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerOnPlatform = false;
        }
    }

}
