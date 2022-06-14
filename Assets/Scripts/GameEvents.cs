using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static Action SpawningStarted;
    public static Action PlayerDestroyed;
    public static Action PlayerHit;
    public static Action EnemyDestroyed;
    public static Action<int> UpdateCash;
    public static Action<int> DisplayCash;
    public static Action<int> UpdateHealth;
    public static Action<int> NewLife;
    public static Action<int> PowerupPickedUp;
    public static Action<float, float, float> CameraShake;
    public static Action<AudioClip> PlaySfx;
    public static Action GameOver;
    public static Action NextLevel;
    public static Action Restart;
    public static Action Quit;
}
