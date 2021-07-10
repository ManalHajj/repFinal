using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 speed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime);
    }
}
