using UnityEngine;

public class DestroyProjectiles : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Projectile") )
        {
            //TODO Pool projectiles
            Destroy(col.gameObject);
        }
    }
}
