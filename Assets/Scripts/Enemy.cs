using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected int cashValue;
    [SerializeField] protected float speed;
    [SerializeField] protected bool randomPattern;
    [SerializeField] protected bool recycle;
    [SerializeField] protected GameObject deathFX;
    [SerializeField] protected AudioClip deathSfx;
    protected bool isAlive;
    protected float defaultSpeed;
    [Header("Camera Shake")]
    [SerializeField] protected float shakeAmt;
    [SerializeField] protected float shakeSlopeOff;
    [SerializeField] protected float shakeTime;
    [Header("AI")]
    //Sideways Sweeping
    [SerializeField] protected bool sweeps;
    [SerializeField] protected float frequency;
    [SerializeField] protected float amplitude;
    protected Vector3 position;
    protected Vector3 axis;
    protected bool wasSweeping;
    [Space]
    //Ramming
    [SerializeField] protected bool rams;
    [SerializeField] protected float rammingSpeed;
    [SerializeField] protected float ramRange;
    [SerializeField] protected float lengthOfRam;
    protected bool isRamming;
    protected float rammingTimer;
    [Space]
    //Shot avoidance
    [SerializeField] protected bool avoidsShots;
    protected bool shotAvoided;
    //Firing Stats
    [Header("Projectiles")]
    [SerializeField] protected bool canFire;
    [SerializeField] protected bool canFireBackwards;
    [SerializeField] protected float minFireRate;
    [SerializeField] protected float maxFireRate;
    private bool directionSet;
    protected float currentFireRate;
    [SerializeField] protected GameObject projectile;
    protected bool firedAtPickup;
    [Header("Support")] 
    //Shield
    [SerializeField] protected bool shielded;
    [SerializeField] protected GameObject shield;
    
    [Header("Helper Variables")]
    [SerializeField] protected float screenBoundsOffset;
    protected Vector3 screenBounds;
    protected GameObject projectileContainer;
    protected float currentTime;
    protected Vector3 currentPlayerPos;

    public bool IsAlive => isAlive;

    private void OnEnable()
    {
        GameEvents.PlayerPosition += GetPlayerPositon;
    }

    private void OnDisable()
    {
        GameEvents.PlayerPosition -= GetPlayerPositon;
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultSpeed = speed;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        isAlive = true;

        projectileContainer = new GameObject(transform.name + "Projectile Container");

        RandomSpawnSpot(screenBounds);
        RandomizeFireRate();

        position = transform.position;
        axis = transform.right;
    }

    private void RandomSpawnSpot(Vector3 screenBounds)
    {
        if (randomPattern)
        {
            Vector3 randomPos = new Vector3(Random.Range(-screenBounds.x + screenBoundsOffset, screenBounds.x - screenBoundsOffset), screenBounds.y + 1, 0);

            transform.position = randomPos;
            position = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        Movement();
        Fire();
        Ram();
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, ramRange);

        if (hit.collider != null && isAlive)
        {
            if (hit.collider.CompareTag("Player") && rams && !isRamming)
            {
                isRamming = true;
                rammingTimer = 0f;
            }

            if (hit.collider.GetComponent<PowerUp>() && !hit.collider.GetComponent<PowerUp>().NegativePowerup && !firedAtPickup && canFire)
            {
                firedAtPickup = true;
                GameObject tempProjectile = Instantiate(projectile, transform.position, quaternion.identity);
                tempProjectile.transform.parent = projectileContainer.transform;
            }
        }
        
        RaycastHit2D circleHit = Physics2D.CircleCast(transform.position, 1, Vector3.down, 1 << LayerMask.NameToLayer("Projectile"));

        if (isAlive)
        {
            if (circleHit.collider.GetComponent<Projectile>() && !circleHit.collider.GetComponent<Projectile>().EnemyProjectile)
            {
                if (!shotAvoided && avoidsShots)
                {
                    float randomDirection = 0;
                
                    if (circleHit.point.x > transform.position.x)
                    {
                        randomDirection = -0.5f;
                    }
                    else if (circleHit.point.x < transform.position.x)
                    {
                        randomDirection = 0.5f;
                    }
                
                    LeanTween.move(gameObject, new Vector2(transform.position.x + randomDirection, transform.position.y), 0.25f);
                    shotAvoided = true;
                }
            }
        }
    }

    private void Fire()
    {
        if (IsAlive)
        {
            if (canFireBackwards && currentFireRate <= currentTime && currentPlayerPos.y > transform.position.y)
            {
                if (projectile != null)
                {
                    GameObject tempProjectile = Instantiate(projectile, transform.position, quaternion.identity);
                    tempProjectile.transform.parent = projectileContainer.transform;
                    tempProjectile.GetComponent<Projectile>().FireBackwards = true;
                    currentTime = 0f;
                }
            }
            else if (canFire && currentFireRate <= currentTime)
            {
                if (projectile != null)
                {
                    GameObject tempProjectile = Instantiate(projectile, transform.position, quaternion.identity);
                    tempProjectile.transform.parent = projectileContainer.transform;
                    currentTime = 0f;
                    firedAtPickup = false;
                }
            }
        }
        
    }
    
    private void RandomizeFireRate()
    {
        float randomFireRate = Random.Range(minFireRate, maxFireRate);
        currentFireRate = randomFireRate;
    }

    private void Movement()
    {
        if (sweeps)
        {
            position += Vector3.down * Time.deltaTime * speed;
            transform.position = position + axis * Mathf.Sin(Time.time * frequency) * amplitude;
        }
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        if (transform.position.y <= -1)
        {
            if (recycle)
            {
                if (rams)
                {
                    ResetRam();
                }

                if (avoidsShots)
                {
                    shotAvoided = false;
                }
                
                RandomSpawnSpot(screenBounds);
            }
            else
            {
                Cleanup();
            }
        }
    }

    private void Ram()
    {
        if (isRamming)
        {
            rammingTimer += Time.deltaTime;
            speed = rammingSpeed;

            if (rammingTimer >= lengthOfRam)
            {
                ResetRam();
            }
        }
    }
    
    private void ResetRam()
    {
        isRamming = false;
        speed = defaultSpeed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") || col.CompareTag("Projectile"))
        {
            if (col.GetComponent<Projectile>() != null)
            {
                if (!col.GetComponent<Projectile>().EnemyProjectile)
                {
                    TakeDamage(col);
                }
            }
            
            if (col.CompareTag("Player"))
            {
                //TODO destroy player and increment score/cash
                GameEvents.PlayerHit();

                Cleanup();
            }
        }
    }

    private void TakeDamage(Collider2D col)
    {
        if (!shielded)
        {
            health -= col.GetComponent<Projectile>().Damage;

            if (health <= 0)
            {
                health = 0;
                Cleanup();
            }
        }
        else
        {
            shield.SetActive(false);
            shielded = false;
        }
    }

    private void Cleanup()
    {
        isAlive = false;
        GameObject tempFx = Instantiate(deathFX, transform.position, Quaternion.identity);
        GameEvents.PlaySfx(deathSfx);
        GameEvents.CameraShake(shakeAmt, shakeSlopeOff, shakeTime);
        
        if (canFire)
        {
            canFire = false;
        }
        
        //TODO Pool enemies instead of destroying
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        Destroy(projectileContainer, 5.0f);
        Destroy(gameObject, 6.0f);

        GameEvents.EnemyDestroyed(gameObject);
        GameEvents.UpdateCash(cashValue);
    }

    private void GetPlayerPositon(Vector3 playerPos)
    {
        if (playerPos != null)
        {
            currentPlayerPos = playerPos;
        }
    }
}
