using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected List<GameObject> shots;
    [SerializeField] protected AudioClip sfx;
    [SerializeField] protected bool enemyProjectile;
    [SerializeField] protected float speed;
    [SerializeField] protected int damage;
    [SerializeField] protected bool perforates;
    [SerializeField] protected int totalHits;
    protected int currentHits;

    public bool EnemyProjectile => enemyProjectile;
    public int Damage => damage;

    // Start is called before the first frame update
    void Start()
    {
        if (sfx != null)
        {
            ProjectileSfx();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyProjectile)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !perforates && !enemyProjectile)
        {
            Destroy(gameObject);
        }
        
        if (col.CompareTag("Enemy") && perforates && !enemyProjectile && currentHits > totalHits)
        {
            Destroy(gameObject);
        }
        else
        {
            currentHits++;
        }

        if (col.CompareTag("Player") && !perforates && enemyProjectile)
        {
            GameEvents.PlayerHit();
            Destroy(gameObject);
        }
    }

    private void ProjectileSfx()
    {
        GameEvents.PlaySfx(sfx);
    }
}
