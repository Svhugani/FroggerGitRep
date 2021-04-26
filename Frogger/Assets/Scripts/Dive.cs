using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dive : MonoBehaviour
{
    private SpriteRenderer _spr;
    private BoxCollider2D _collider;
    private Color _colNormal = Color.white;
    private Color _colSemiDive = new Color(0.15f, 0.75f, 1f, 0.85f);
    private Color _colDive = new Color (0.09f, 0.33f, 0.42f, 0.0f);

    public float timeOnWater;
    public float timeSwitchState;
    public float timeUndrwater;
    private IEnumerator _coroutine;





    IEnumerator Diving(float timeOnWater, float timeUnderWater, float timeSwitchState)
    {
        while(true)
        {   
            _collider.enabled = true;
            _spr.color = _colNormal;
            yield return new WaitForSeconds(timeOnWater);
            _spr.color = _colSemiDive;
            yield return new WaitForSeconds(timeSwitchState);
            _spr.color = _colDive;
            _collider.enabled = false;
            yield return new WaitForSeconds(timeUnderWater);
        }
        
    }
    
    void Awake() 
    {
        _spr = this.GetComponent<SpriteRenderer>();
        _collider = this.GetComponent<BoxCollider2D>();
        _spr.color = _colNormal;
    }
    
    void OnEnable() 
    {
        _coroutine = Diving(timeOnWater, timeSwitchState, timeUndrwater);
        StartCoroutine(_coroutine);
    }

    void OnDisable()
    {
        StopCoroutine(_coroutine);
    }


    void Start()
    {

    }


    void Update()
    {

    }
}
