using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static Action SpawningStarted;
    public static Action PlayerDestroyed;
    public static Action PlayerHit;
    public static Action<GameObject> EnemyDestroyed;
    public static Action PowerupDestroyed;
    public static Action<Vector3> PlayerPosition;
    public static Action PingEnemyList;
    public static Action<List<GameObject>> GetAllCurrentEnemies;
    public static Action<int> UpdateCash;
    public static Action<int> DisplayCash;
    public static Action<int, int> UpdateAmmo;
    public static Action<int> UpdateHealth;
    public static Action<int> NewLife;
    public static Action<int> PowerupPickedUp;
    public static Action<float, float, float> CameraShake;
    public static Action<float> ThrusterSupply;
    public static Action<float> SetThrusterMax;
    public static Action<AudioClip> PlaySfx;
    public static Action GameOver;
    public static Action NextLevel;
    public static Action Restart;
    public static Action Quit;
}
