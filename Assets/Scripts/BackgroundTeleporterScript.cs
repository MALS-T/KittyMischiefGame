using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTeleporterScript : MonoBehaviour
{
    [SerializeField] private Transform backgroundStartPos;

    void OnTriggerEnter2D (Collider2D collision)
    {
        collision.gameObject.transform.position = backgroundStartPos.position;
    }

}
