using System;
using System.Collections;
using System.Collections.Generic;
using Ludiq.PeekCore;
using UnityEngine;

public class DestroyProjectiles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Projectile"))
        {
            //TODO Pool projectiles
            Destroy(col.gameObject);
        }
    }
}
