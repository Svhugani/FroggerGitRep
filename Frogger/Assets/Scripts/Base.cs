using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public Sprite[] spriteArray;
    private SpriteRenderer _spr;
    private bool _isEmpty = true;
    
    public void ResetBase()
    {
        _isEmpty = true;
        _spr.sprite = spriteArray[0];
    }
    void Awake() 
    {
        _spr = this.GetComponent<SpriteRenderer>();
        _spr.sprite = spriteArray[0];
    }
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        _spr.sprite = spriteArray[1];
        if(_isEmpty)
        {
            Frog frog = other.GetComponent<Frog>();
            frog.BaseCompleted();
            frog.ResetPosition(false);
            _isEmpty = false;
        }
    }
    void Update()
    {
        
    }
}
