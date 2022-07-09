using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySfx : MonoBehaviour
{
    [SerializeField] protected AudioClip sfx;
    public void PlayClip()
    {
        GameEvents.PlaySfx(sfx);
    }
}
