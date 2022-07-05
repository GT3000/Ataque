using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum States
{
    Start,
    Idle,
    Phase1,
    Phase2,
    Death
}

public class Boss : MonoBehaviour
{
    [SerializeField] protected States bossState;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int cashValue;
    protected int currentHealth;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected GameObject startPoint;
    [SerializeField] protected GameObject deathFX;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [Header("Camera Shake")] 
    [SerializeField] protected float shakeAmt;
    [SerializeField] protected float shakeSlopeOff;
    [SerializeField] protected float shakeTime;
    [Header("Phase 1")]
    [SerializeField] protected List<GameObject> phase1Firepoints;
    [SerializeField] protected float p1Firerate;
    protected float p1FirepointsTimer;
    [SerializeField] protected List<GameObject> phase1MovementPoints;
    protected int currentP1Waypoint;
    [Header("Phase 2")]
    [SerializeField] protected List<GameObject> phase2Firepoints;
    [SerializeField] protected float p2Firerate;
    [SerializeField] protected float p2ShotDelay;
    protected float p2FirepointsTimer;
    [SerializeField] protected List<GameObject> phase2MovementPoints;

    protected int currentP2Waypoint;
    protected bool inPhase1;
    protected bool inPhase2;
    protected bool idling;
    protected bool halfHealth;
    protected bool firing;
    protected GameObject projectileContainer;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        bossState = States.Start;
        SwitchState(bossState);
        
        projectileContainer = new GameObject(transform.name + "Boss Projectile Container");
    }

    // Update is called once per frame
    void Update()
    {
        p1FirepointsTimer += Time.deltaTime;
        p2FirepointsTimer += Time.deltaTime;
        
        if (currentHealth <= (maxHealth / 2) && !halfHealth)
        {
            print("Entering 2nd Phase");
            halfHealth = true;

            if (bossState == States.Phase1)
            {
                bossState = States.Phase2;
                SwitchState(bossState);
            }
        }

        StartCoroutine(FireWeapons());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Projectile>() && !other.GetComponent<Projectile>().EnemyProjectile)
        {
            print("Hit!");
            TakeDamage(other.GetComponent<Projectile>().Damage);
        }
    }

    private void SwitchState(States newState)
    {
        switch (newState)
        {
            case States.Start:
                StopAllCoroutines();
                StartCoroutine(MoveToStart());
                break;
            
            case States.Idle:
                if (!idling)
                {
                    StopAllCoroutines();
                    StartCoroutine(Idle());
                }
                break;
            
            case States.Phase1:
                if (!inPhase1)
                {
                    StopAllCoroutines();
                    StartCoroutine(Phase1());
                }
                break;
            
            case States.Phase2:
                if (!inPhase2)
                {
                    StopAllCoroutines();
                    StartCoroutine(Phase2());
                }
                break;
            
            case States.Death:
                StartCoroutine(Death());
                break;
        }
    }

    private IEnumerator MoveToStart()
    {
        if (Vector3.Distance(transform.position, startPoint.transform.position) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.transform.position, moveSpeed / 4 * Time.deltaTime);
            
            yield return null;
            
            StartCoroutine(MoveToStart());
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
            
            bossState = States.Phase1;
            SwitchState(bossState);
            StopCoroutine(MoveToStart());
        }

        yield return null;
    }

    private IEnumerator Phase1()
    {
        inPhase1 = true;
        inPhase2 = false;
        
        if (currentP1Waypoint > phase1MovementPoints.Count - 1)
        {
            yield return new WaitForSeconds(1.0f);
            
            bossState = States.Idle;
            SwitchState(bossState);
            StopCoroutine(Phase1());
            currentP1Waypoint = 0;
        }
        else
        {
            if (Vector3.Distance(transform.position, phase1MovementPoints[currentP1Waypoint].transform.position) > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, phase1MovementPoints[currentP1Waypoint].transform.position, moveSpeed / 4 * Time.deltaTime);

                yield return null;
            
                StartCoroutine(Phase1());
            }
            else
            {
                yield return new WaitForSeconds(2.0f);
            
                currentP1Waypoint++;
            
                StartCoroutine(Phase1());
            }

            yield return null;
        }
    }
    
    private IEnumerator Phase2()
    {
        inPhase2 = true;
        inPhase1 = false;
        
        if (currentP2Waypoint > phase2MovementPoints.Count - 1)
        {
            yield return new WaitForSeconds(1.0f);
                    
            currentP2Waypoint = 0;
            
            bossState = States.Idle;
            SwitchState(bossState);
            StopCoroutine(Phase2());
        }
        
        if (Vector3.Distance(transform.position, phase2MovementPoints[currentP2Waypoint].transform.position) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, phase2MovementPoints[currentP2Waypoint].transform.position, moveSpeed / 2 * Time.deltaTime);

            yield return null;
            
            StartCoroutine(Phase2());
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            
            currentP2Waypoint++;
            StartCoroutine(Phase2());
        }
        
        yield return null;
    }

    private IEnumerator FireWeapons()
    {
        if (bossState == States.Phase1)
        {
            if (p1FirepointsTimer >= p1Firerate)
            {
                for (int i = 0; i < phase1Firepoints.Count; i++)
                {
                    if (projectile != null)
                    {
                        GameObject tempProjectile = Instantiate(projectile, phase1Firepoints[i].transform.position, Quaternion.identity);
                    }

                    if (i >= phase1Firepoints.Count - 1)
                    {
                        p1FirepointsTimer = 0f;
                    }
                }
            }
        }

        if (bossState == States.Phase2)
        {
            if (p2FirepointsTimer >= p2Firerate && !firing)
            {
                firing = true;
                
                foreach (var weapon in phase2Firepoints)
                {
                    yield return new WaitForSeconds(p2ShotDelay);
                    
                    if (projectile != null)
                    {
                        GameObject tempProjectile = Instantiate(projectile, weapon.transform.position, Quaternion.identity);
                    }
                }

                p2FirepointsTimer = 0f;
            }
            else if (p2FirepointsTimer < p2Firerate)
            {
                firing = false;
            }
        }

        yield return null;
    }

    private IEnumerator Idle()
    {
        idling = true;
        
        if (Vector3.Distance(transform.position, startPoint.transform.position) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.transform.position, moveSpeed / 4 * Time.deltaTime);

            yield return null;
            
            StartCoroutine(Idle());
        }
        else
        {
            print("Waiting");
            
            yield return new WaitForSeconds(6.0f);
            
            print("Return to Phase");

            if (!halfHealth)
            {
                inPhase1 = false;
                idling = false;

                bossState = States.Phase1;
                SwitchState(bossState);
                StopCoroutine(Idle());
            }
            else
            {
                inPhase2 = false;
                idling = false;

                bossState = States.Phase2;
                SwitchState(bossState);
                StopCoroutine(Idle());
            }
        }
    }

    private void TakeDamage(int damage)
    {
        if (bossState != States.Death)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                bossState = States.Death;
                SwitchState(bossState);
            }
        }
    }

    private IEnumerator Death()
    {
        GameObject tempDeath = Instantiate(deathFX, transform.position, Quaternion.identity);
        GameEvents.CameraShake(shakeAmt, shakeSlopeOff, shakeTime);

        moveSpeed = 0f;
        spriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Animator>().SetBool("death", true);

        Destroy(projectileContainer, 5.0f);
        Destroy(gameObject, 6.0f);

        GameEvents.EnemyDestroyed(gameObject);
        GameEvents.UpdateCash(cashValue);

        yield return new WaitForSeconds(tempDeath.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
        
        GameEvents.GameOver();
    }
}
