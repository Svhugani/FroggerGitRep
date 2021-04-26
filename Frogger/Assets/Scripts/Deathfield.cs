using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathfield : MonoBehaviour
{
    private SpriteRenderer _spr;

    void OnTriggerEnter2D(Collider2D other) 
    {
        //other.GetComponent<Frog>().ResetPosition(true);
    }

    private IEnumerator SpriteFlipper(float time)
    {
        while(true)
        {
            yield return new WaitForSeconds(time);
            _spr.flipX = !_spr.flipX;
        }

    }
    void Awake() 
    {
        _spr = this.GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        StartCoroutine(SpriteFlipper(1f));
    }


    void Update()
    {
        
    }
}
