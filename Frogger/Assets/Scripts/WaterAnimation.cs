using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimation : MonoBehaviour
{
    public Sprite[] spriteArray;
    private SpriteRenderer spriteRenderer;
    private int iterator = 0;
    private int maxIterations;

    IEnumerator Animate(float time)
    {
        while(true)
        {
            iterator = iterator > maxIterations - 1 ? 0 : iterator;
            yield return new WaitForSeconds(time);
            spriteRenderer.sprite = spriteArray[iterator];
            iterator ++;
        }
    }


    void Awake() 
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        maxIterations = spriteArray.Length;
    }
    void Start()
    {
        StartCoroutine(Animate(0.5f));
    }


    void Update()
    {
        
    }
}
