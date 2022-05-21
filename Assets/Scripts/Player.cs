using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] protected float speed;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected float fireRate;
    [SerializeField] protected int maxHealth;
    protected int currentHealth;
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
    }

    private void OnDisable()
    {
        GameEvents.PlayerDestroyed -= Death;
        GameEvents.PlayerHit -= TakeDamage;
    }

    void Start()
    {
        transform.position = Vector3.zero;
        projectileGroup = new GameObject("Projectiles");
        currentHealth = maxHealth;

        if (Camera.main != null)
        {
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        
        Move();
        Fire();
    }

    private void Move()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        transform.Translate(new Vector3(horizontalMovement, verticalMovement, 0) * speed * Time.deltaTime);
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
        currentHealth--;
        
        //TODO Show damage
        GetComponent<Animator>().SetTrigger("damaged");

        if (currentHealth <= 0)
        {
            lives--;
        }
        
        if (lives <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        //TODO Explosion VFX
        //TODO end game logic
        Destroy(gameObject);
    }
}
