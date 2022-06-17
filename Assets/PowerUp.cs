using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum PowerupType
{
    Firepower,
    Energy,
    Missile,
    Shield,
    Speed,
    Health,
    Ammo
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] protected float lifetime;
    [SerializeField] protected int spawnWeight;
    [SerializeField] protected AudioClip pickupSfx;
    [SerializeField] protected PowerupType powerup;
    [SerializeField] protected float speed;
    [SerializeField] protected bool randomPattern;
    protected Vector3 screenBounds;
    private float currentTime;
    
    public int SpawnWeight => spawnWeight;

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        
        RandomSpawnSpot(screenBounds);
    }
    
    private void RandomSpawnSpot(Vector3 screenBounds)
    {
        if (randomPattern)
        {
            Vector3 randomPos = new Vector3(Random.Range(-screenBounds.x, screenBounds.x), screenBounds.y + 1, 0);

            transform.position = randomPos;
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (lifetime <= currentTime)
        {
            //TODO Pool instead of destroy
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameEvents.PowerupPickedUp((int)powerup);
            GameEvents.PlaySfx(pickupSfx);
            
            //TODO Pool instead of destroy
            Destroy(gameObject);
        }
    }
}
