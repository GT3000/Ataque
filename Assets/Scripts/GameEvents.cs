using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static Action PlayerDestroyed;
    public static Action PlayerHit;
    public static Action EnemyDestroyed;
    public static Action<int> PowerupPickedUp;
}
