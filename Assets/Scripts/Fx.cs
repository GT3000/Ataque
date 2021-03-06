using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fx : MonoBehaviour
{
    protected Transform parent;
    
    // Start is called before the first frame update
    void Start()
    {
        if (transform.GetComponentInParent<Transform>() != null)
        {
            parent = transform.GetComponentInParent<Transform>();
        }

        StartCoroutine(DestroyFx());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerator DestroyFx()
    {
        yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);

        if (parent != null)
        {
            Destroy(transform.GetComponentInParent<Transform>().gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
