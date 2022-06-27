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
    [SerializeField] protected bool homing;
    protected int currentHits;
    protected bool fireBackwards;
    [SerializeField] protected List<GameObject> enemiesOnField;
    [SerializeField] protected Transform homingTarget;
    [SerializeField] protected float homingIntervalCheck;
    protected float homingCheckTimer;

    public bool EnemyProjectile => enemyProjectile;
    public int Damage => damage;

    public bool FireBackwards
    {
        get => fireBackwards;
        set => fireBackwards = value;
    }

    private void OnEnable()
    {
        GameEvents.GetAllCurrentEnemies += GetAllCurrentEnemies;
    }

    private void OnDisable()
    {
        GameEvents.GetAllCurrentEnemies -= GetAllCurrentEnemies;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (sfx != null)
        {
            ProjectileSfx();
        }

        if (homing)
        {
            GameEvents.PingEnemyList();
            homingTarget = GetNewTarget();
        }
    }

    // Update is called once per frame
    void Update()
    {
        homingCheckTimer += Time.deltaTime;
        
        if (!enemyProjectile)
        {
            if (homing && homingTarget != null && homingTarget.GetComponent<Enemy>().IsAlive)
            {
                Vector3 direction = homingTarget.position - transform.position;
                
                transform.position =  Vector3.MoveTowards(transform.position, homingTarget.transform.position, speed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            }
            else
            {
                homingTarget = GetNewTarget();

                if (homingTarget == null)
                {
                    transform.Translate(Vector3.up * speed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (fireBackwards)
            {
                transform.Translate(Vector3.up * speed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
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

        if (col.GetComponent<PowerUp>() != null)
        {
            if (!col.GetComponent<PowerUp>().NegativePowerup && enemyProjectile)
            {
                GameEvents.PowerupDestroyed();
                Destroy(gameObject);
            }
        }
    }

    private void ProjectileSfx()
    {
        GameEvents.PlaySfx(sfx);
    }

    private void GetAllCurrentEnemies(List<GameObject> currentEnemies)
    {
        enemiesOnField = currentEnemies;
    }

    private Transform GetNewTarget()
    {
        Transform bestTarget = null;
        
        if (homingTarget == null && enemiesOnField != null)
        {
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPos = transform.position;

            for (int i = 0; i < enemiesOnField.Count; i++)
            {
                if (enemiesOnField[i].GetComponent<Enemy>().IsAlive)
                {
                    Vector3 directToTarget = enemiesOnField[i].transform.position - currentPos;
                    float dSqrToTarget = directToTarget.sqrMagnitude;

                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = enemiesOnField[i].transform;
                    }
                }
            }
        }
        
        return bestTarget;
    }
}
