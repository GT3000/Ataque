using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;         
    private float shakeAmount;                                  
    private float shakeDuration;                                
    private float shakeSlopeOff;                                
    private Vector3 originalPos;
    
    private void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    private void OnEnable()
    {
        originalPos = cameraTransform.localPosition;
        GameEvents.CameraShake += Shake;
    }

    private void OnDisable()
    {
        GameEvents.CameraShake -= Shake;
    }

    void Update()
    {
        ShakeControl();
    }
    
    private void ShakeControl()
    {
        if (shakeDuration > 0)
        {
            cameraTransform.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;
            
            shakeDuration -= Time.unscaledDeltaTime * shakeSlopeOff;
        }
        else
        {
            shakeDuration = 0f;
            cameraTransform.localPosition = originalPos;
        }
    }
    
    private void Shake(float totalShake, float shakeDecrease, float shakeTime)
    {
        shakeDuration = shakeTime;
        shakeSlopeOff = shakeDecrease;
        shakeAmount = totalShake;
    }
}