using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySfx : MonoBehaviour
{
    [SerializeField] protected AudioClip sfx;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayClip()
    {
        GameEvents.PlaySfx(sfx);
    }
}
