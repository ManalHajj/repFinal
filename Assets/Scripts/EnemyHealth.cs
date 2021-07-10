using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Damageable
{

    public GameObject explosion;

    public override void Death()
    {
        Destroy(gameObject);
    }

    

    public void Explosion()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
    }

}
