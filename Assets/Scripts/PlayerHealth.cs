using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Damageable
{
    private int defaultLayer;

    private void Start()
    {
        base.Start();
        defaultLayer = gameObject.layer;
    }

    public override void Death()
    {
        Debug.Log("dead");


    }

    public void SetInvencible(bool state)
    {
        if (state)
        {
            
            gameObject.layer = LayerMask.NameToLayer("Invencible");
        }
        else
            gameObject.layer = defaultLayer;
    }





}