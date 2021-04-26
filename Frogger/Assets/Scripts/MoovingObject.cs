using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoovingObject : MonoBehaviour
{
    private float _velocity;
    private bool _rightDirection;
    private Rigidbody2D _rigidBody;
    private float _leftBound;
    private float _rightBound;
    private GameManager gm;
    private int _objectType;


    public float Velocity
    {
        get {return _velocity;}
    }
    public int ObjectType
    {
        get {return _objectType;}
        set {_objectType = value;}
    }

    public GameManager SetGameManagerRef
    {
        set {gm = value;}
    }
    public void InitializeObject(Vector3 position, float velocity)
    {
        ActivateObject(true);
        _velocity = velocity;
        _rigidBody.position = position;
        if (_velocity < 0 )
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
        }

    }

    public void SetBounds(float leftBound, float rightBound)
    {
        _leftBound = leftBound;
        _rightBound = rightBound;
    }

    public void ActivateObject(bool activate)
    {
        if (activate)
        {
            this.gameObject.SetActive(true);
        }
        else 
        {
            this.gameObject.SetActive(false);
        }

    }

    void Awake()
    {
        _rigidBody = this.GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if(gm.DeactivateOnForce)
        {
            gm.PullObject(this);
        }
    }
    void FixedUpdate()
    {
        _rigidBody.MovePosition(_rigidBody.position + Vector2.right * Time.fixedDeltaTime * _velocity);
        
        if ((_rigidBody.position.x > _rightBound && _velocity > 0)|| (_rigidBody.position.x <_leftBound && _velocity < 0))
        {
            gm.PullObject(this);
        }
    }
}
