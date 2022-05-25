using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] protected float speed;
    [SerializeField] protected float speedBoostSpeed;
    [SerializeField] protected float speedBoostDuration;
    protected float speedDurationTimer;
    protected float currentSpeed;
    protected bool speedBoostActive;
    [SerializeField] protected GameObject shield;
    protected bool shieldActive;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected GameObject[] firepowerProjectiles;
    [SerializeField] protected GameObject[] energyProjectiles;
    [SerializeField] protected GameObject[] missileProjectiles;
    protected int currentUpgrade;
    [SerializeField] protected int upgradeLevel;
    [SerializeField] protected float fireRate;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int lives;
    protected float currentTime;
    protected GameObject projectileGroup;
    [SerializeField] protected Transform firePoint;
    protected Vector2 screenBounds;
    [SerializeField] private float screenBoundsXOffset;
    [SerializeField] private float screenboundsYOffset;
    // Start is called before the first frame update

    private void OnEnable()
    {
        GameEvents.PlayerDestroyed += Death;
        GameEvents.PlayerHit += TakeDamage;
        GameEvents.PowerupPickedUp += SetUpgrade;
    }

    private void OnDisable()
    {
        GameEvents.PlayerDestroyed -= Death;
        GameEvents.PlayerHit -= TakeDamage;
        GameEvents.PowerupPickedUp -= SetUpgrade;
    }

    void Start()
    {
        transform.position = Vector3.zero;
        projectileGroup = new GameObject("Projectiles");
        currentHealth = maxHealth;
        currentSpeed = speed;

        if (Camera.main != null)
        {
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        speedDurationTimer += Time.deltaTime;

        Move();
        Fire();
    }

    private void Move()
    {
        if (speedDurationTimer >= speedBoostDuration && speedBoostActive)
        {
            currentSpeed = speed;
        }
        
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        transform.Translate(new Vector3(horizontalMovement, verticalMovement, 0) * currentSpeed * Time.deltaTime);
    }

    private void Fire()
    {
        if (Input.GetButton("Fire1") && projectile != null && currentTime >= fireRate)
        {
            GameObject tempProjectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
            tempProjectile.transform.parent = projectileGroup.transform;

            currentTime = 0f;
        }
    }

    private void SetUpgrade(int powerupId)
    {
        //TODO set powerup by it's ID and upgrade if it's not at max otherwise switch
        switch (powerupId)
        {
            case 0:
                //Firepower
                if (currentUpgrade == 0)
                {
                    if (upgradeLevel < firepowerProjectiles.Length - 1)
                    {
                        upgradeLevel++;
                        projectile = firepowerProjectiles[upgradeLevel];
                    }
                    else if (currentUpgrade == 1 && upgradeLevel >= firepowerProjectiles.Length - 1)
                    {
                        //TODO Point Bonus
                        print("Point Bonus");
                    }
                }
                else
                {
                    currentUpgrade = 0;
                    upgradeLevel = 0;
                    projectile = firepowerProjectiles[0];
                }
                
                break;
            
            case 1:
                //Energy
                if (currentUpgrade == 1)
                {
                    if (upgradeLevel < energyProjectiles.Length - 1)
                    {
                        upgradeLevel++;
                        projectile = energyProjectiles[upgradeLevel];
                    }
                    else if (upgradeLevel >= energyProjectiles.Length - 1)
                    {
                        //TODO Point Bonus
                        print("Point Bonus");
                    }
                }
                else
                {
                    currentUpgrade = 1;
                    upgradeLevel = 0;
                    projectile = energyProjectiles[0];
                }
                
                break;
                
            case 2:
                //Missile
                if (currentUpgrade == 2)
                {
                    if (upgradeLevel < missileProjectiles.Length - 1)
                    {
                        upgradeLevel++;
                        projectile = missileProjectiles[upgradeLevel];
                    }
                    else if (upgradeLevel >= missileProjectiles.Length - 1)
                    {
                        //TODO Point Bonus
                        print("Point Bonus");
                    }
                }
                else
                {
                    currentUpgrade = 2;
                    upgradeLevel = 0;
                    projectile = missileProjectiles[0];
                }
                
                break;
            
            case 3:
                //Shield
                if (!shieldActive)
                {
                    shieldActive = true;
                    shield.SetActive(true);
                    shield.GetComponent<Animator>().SetBool("popped", false);
                }
                else
                {
                    //TODO Point Bonus
                }
                
                break;
            
            case 4:
                //Speed
                if (!speedBoostActive)
                {
                    speedDurationTimer = 0f;
                    currentSpeed = speedBoostSpeed;
                    speedBoostActive = true;
                    
                    //TODO Animate Speed Boost
                }
                else
                {
                    //TODO Point Bonus
                }

                break;
            
            case 5:
                //Health
                print("Health!");
                //TODO Health bump up to max, bonus points if over.
                break;

        }
    }

    private void LateUpdate()
    {
        Boundaries();
    }

    private void Boundaries()
    {
        if (transform.position.x >= screenBounds.x - screenBoundsXOffset)
        {
            transform.position = new Vector3(screenBounds.x - screenBoundsXOffset, transform.position.y);
        }
        else if (transform.position.x <= -screenBounds.x + screenBoundsXOffset)
        {
            transform.position = new Vector3(-screenBounds.x + screenBoundsXOffset, transform.position.y);
        }

        if (transform.position.y >= screenBounds.y - screenboundsYOffset)
        {
            transform.position = new Vector3(transform.position.x, screenBounds.y - screenboundsYOffset);
        }
        else if (transform.position.y <= 0 - screenboundsYOffset)
        {
            transform.position = new Vector3(transform.position.x, 0 - screenboundsYOffset);
        }
    }

    private void TakeDamage()
    {
        if (shieldActive)
        {
            //TODO Shield dispel animation
            shield.GetComponent<Animator>().SetBool("popped", true);
            
            shieldActive = false;
            shield.SetActive(false);
        }
        else
        {
            currentHealth--;
        
            //TODO Show damage
            GetComponent<Animator>().SetTrigger("damaged");

            if (currentHealth <= 0)
            {
                lives--;
                currentHealth = maxHealth;
            
                print("New life.");
            }
        
            if (lives <= 0)
            {
                Death();
            }
        }
        
    }

    private void Death()
    {
        GameEvents.EnemyDestroyed();
        //TODO Explosion VFX
        //TODO end game logic
        Destroy(gameObject);
    }
}
