using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Detector : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider2D _collider;
    private GameManager gm;
    private int[] _layers;
    private bool _initFrog = false;
    public GameManager SetGameManagerRef
    {
        set {gm = value;}
    }

    public int[] SetLayers
    {
        set{_layers = value;}
    }

    void Awake() 
    {
        _collider = this.GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (_layers.Contains(other.gameObject.layer))
        {
            _initFrog =true;
        }  
    }

    void FixedUpdate()
    {
        if(_initFrog)
        {
            gm.SetUpFrog();
            _collider.enabled =false;
            _initFrog = false;
        }

    }
}
