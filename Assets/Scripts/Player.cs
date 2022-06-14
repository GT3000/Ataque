using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int lives;
    [SerializeField] protected float speed;
    protected Vector3 startPos;
    [Header("Supports")]
    [SerializeField] protected float speedBoostSpeed;
    [SerializeField] protected float speedBoostDuration;
    [SerializeField] protected float thrusterBoost;
    protected float speedDurationTimer;
    protected float currentSpeed;
    protected bool speedBoostActive;
    [SerializeField] protected GameObject shield;
    [SerializeField] protected GameObject[] shieldStages;
    [SerializeField] protected int shieldHits;
    protected bool shieldActive;
    [Header("Weapons")]
    [SerializeField] protected List<GameObject> damagePoints;
    [SerializeField] protected GameObject damagePrefab;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected GameObject[] firepowerProjectiles;
    [SerializeField] protected GameObject[] energyProjectiles;
    [SerializeField] protected GameObject[] missileProjectiles;
    protected int currentUpgrade;
    [SerializeField] protected int upgradeLevel;
    [SerializeField] protected float fireRate;
    protected float currentTime;
    protected GameObject projectileGroup;
    [SerializeField] protected Transform firePoint;
    protected Vector2 screenBounds;
    [Header("SFX")] 
    [SerializeField] protected AudioClip playerHurt;
    [Header("Screen Bounds")]
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
        startPos = Vector3.zero;
        transform.position = startPos;
        
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
        if (Input.GetKey(KeyCode.LeftShift) && !speedBoostActive)
        {
            currentSpeed = speed * thrusterBoost;
        }

        if (speedDurationTimer <= speedBoostDuration && speedBoostActive)
        {
            currentSpeed = speedBoostSpeed;
        }
        else
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
                    shieldHits = shieldStages.Length;
                    shieldActive = true;
                    shield.SetActive(true);
                    shieldStages[0].SetActive(true);
                    shieldStages[1].SetActive(true);
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
                if (currentHealth < maxHealth)
                {
                    Heal();
                }
                else
                {
                    //TODO Point Bonus
                }
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
            shieldHits--;

            if (shieldHits < 0)
            {
                shield.GetComponent<Animator>().SetBool("popped", true);

                shieldActive = false;
                shield.SetActive(false);
            }
            else
            {
                shieldStages[shieldHits].SetActive(false);
            }
        }
        else
        {
            currentHealth--;
            GameEvents.UpdateHealth(currentHealth);

            //TODO Show damage
            GetComponent<Animator>().SetTrigger("damaged");

            if (damagePoints != null)
            {
                if (currentHealth == 2)
                {
                    damagePoints[0].SetActive(true);
                }

                if (currentHealth == 1)
                {
                    damagePoints[1].SetActive(true);
                }

                GameEvents.CameraShake(0.2f, 1.0f, 0.25f);
            }

            if (currentHealth <= 0 && lives > 0)
            {
                lives--;
                currentHealth = maxHealth;
                GameEvents.NewLife(lives);
                transform.position = startPos;

                foreach (var damage in damagePoints)
                {
                    damage.SetActive(false);
                }
            }
        
            if (lives <= 0)
            {
                Death();
            }
        }
        
    }

    private void Heal()
    {
        currentHealth++;
        GameEvents.UpdateHealth(currentHealth);

        //TODO Show health animation

        if (damagePoints != null)
        {
            if (currentHealth >= 2)
            {
                damagePoints[0].SetActive(false);
            }
            else if (currentHealth == 1)
            {
                damagePoints[1].SetActive(false);
            }
        }
    }

    private void Death()
    {
        GameEvents.EnemyDestroyed();
        GameEvents.GameOver();
        //TODO Explosion VFX
        //TODO end game logic
        Destroy(gameObject);
    }
}
