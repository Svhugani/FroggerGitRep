using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Frog : MonoBehaviour
{
    [HideInInspector] public GameObject scoreCount;
    [HideInInspector] public GameObject lifesCount;
    [HideInInspector] public GameObject gameTimer;
    private TMP_Text _scoreText;
    private TMP_Text _lifesText;
    private TImer _timer;
    private float _leftRightStep;
    private List<float> _verticalGrid;
    private Rigidbody2D _rigidBody;
    private int _rowPosition;
    private int _vertGridSize;
    private float[] _leftRightLimits;
    private int _currentRotation;
    private int _healthPoints;
    private int _deathLayer;
    private int _dragLayer;
    private bool _isDragged = false;
    private Vector2 _dragVelocity = Vector2.zero;
    private Vector2 _initPosition;
    private int _startRow;
    private int _midRow;
    private int _endRow;
    private int _numOfStreetRows;
    private int _numOfRiverRows;
    private int _basePoints;
    private int _maxBasePoints;
    private int _scores;
    private int _highestPoint;
    private GameManager gm;
    private bool _enteredDragObject = false;
    private bool _cheatAllowed = true;

    // GET SET METHODS TO ACCES VARIABLES
    public List<float> VerticalGrid
    {
        set {_verticalGrid = value;}
    }

    public float LeftRightStep
    {
        set {_leftRightStep = value;}
    }

    public int RowPosition
    {
        set {_rowPosition = value;}
    }

    public float[] LeftRightLimits
    {
        set {_leftRightLimits = value;}
    }

    public int HealthPoints
    {
        get {return _healthPoints;}
        set {_healthPoints = value;}
    }

    public int BasePoints
    {
        set{_maxBasePoints = value;}
    }

    public GameManager SetGameManagerRef
    {
        set {gm = value;}
    }

    // RESET POSITION FUNCTION, PUTS FROG TO INIT POSITION, UDPATES HEALTH POINTS IF NEEDED
    public void ResetPosition(bool death)
    {
        if (death)
        {
            _healthPoints--;
            _lifesText.text = "LIVES: " + _healthPoints.ToString();
            if (_healthPoints == 0)
            {
                gm.GameLost();
            }
        }
        _rowPosition = 0;
        _rigidBody.position = new Vector2(_initPosition.x, _verticalGrid[_rowPosition]);
        _dragVelocity = Vector2.zero;
        
    }

    // GET LAYER NAMES FROM GAME MANAGER TO DETERMIN WHETER OBJECT IS DEATHLY OR ITS A DRAG-TYPE OBJECT
    public void SetLayers(int deathLayer, int dragLayer)
    {
        _deathLayer = deathLayer;
        _dragLayer = dragLayer;
    }
    
    // SETS ROW NUMBERS FOR PROPER ORIENTATION (WE NEED TO KNOW WHERE IS WATER)
    public void SetRowNumbers(int startRowNum, int midRowNum, int endRowNum, int numOfStreetRows, int numOfRiverRows)
    {
        _startRow = startRowNum;
        _midRow = midRowNum;
        _endRow = endRowNum;
        _numOfStreetRows = numOfStreetRows;
        _numOfRiverRows = numOfRiverRows;
    }

    // SIMPLE FUNCTION TO UPDATE TOTAL SCORE
    public void ScoreSystem()
    {
        if(_rowPosition > _highestPoint)
        {
            _highestPoint = _rowPosition;
            _scores += gm.pointsPerRow;
            _scoreText.text = "TOTAL SCORE: " + _scores.ToString();
        }
    }

    // FUNCTION USED WHEN BASE-TYPE OBJECT DETECTS OUR FROG, IF WE COMPLETE ALL WE WIN LEVEL
    public void BaseCompleted()
    {
        _basePoints ++;
        _scores += gm.pointsPerBase;
        _scores += (int)_timer.TimeLeft * gm.pointsPerTimeLeft;
        _timer.TimeReset();
        _scoreText.text = "TOTAL SCORE: " + _scores.ToString();

        if (_basePoints == _maxBasePoints)
        {
            _basePoints = 0;
            _highestPoint = 0;
            gm.NewLevel();
        }
    }

    // CHEAT CODE, WE SHIFT TO ANOTHER LEVEL
    IEnumerator CheatCode()
    {   
        _cheatAllowed = false;
        _scores += 2000;
        _scoreText.text = "TOTAL SCORE: " + _scores.ToString();
        _highestPoint = 0;
        gm.NewLevel();
        _timer.TimeReset();
        yield return new WaitForSeconds(1f);
        _cheatAllowed = true;
    }

    // ROTATE SPRITE TO FOLLOW MOVEMENT DIRECTION
    void MovementRotation(int rotation)
    {
        int angle = 90 * rotation;
        this.transform.localRotation = Quaternion.Euler(angle * Vector3.forward);
    }

    // MAIN FUNCTION FOR MOVEMENT CONTROLL OF OUR FROG. W-A-S-D. 
    // THE IDEA IS TO FOLLOW BASE BOINTS IN Y DIRECTION, SO THERE IS DISCRETE(NOT continuous) MOVEMENT. IT IS EASIER SINCE WE CAN DRAG THOSE POINTS IN THE EDITOR 
    void MovementControl()
    {
        float jump;
        int rotation;

        _rigidBody.velocity = _dragVelocity;
        
        if (Input.GetKeyDown(KeyCode.W) && (_rowPosition < _vertGridSize -1))
        {   
            _rowPosition ++;
            jump = Mathf.Abs(_verticalGrid[_rowPosition] - _verticalGrid[_rowPosition - 1]);
            _rigidBody.MovePosition(_rigidBody.position + Vector2.up * jump);
            rotation = 0;
        }
        else if (Input.GetKeyDown(KeyCode.S) && (_rowPosition > 0))
        {
            _rowPosition --;
            jump = Mathf.Abs(_verticalGrid[_rowPosition] - _verticalGrid[_rowPosition + 1]);
            _rigidBody.MovePosition(_rigidBody.position + Vector2.down * jump);       
            rotation = 2;    
        }
        else if (Input.GetKeyDown(KeyCode.D) && (_rigidBody.position.x + _leftRightStep) < _leftRightLimits[1])
        {
            _rigidBody.MovePosition(_rigidBody.position + Vector2.right * _leftRightStep);
            rotation = 3;
        }
        else if (Input.GetKeyDown(KeyCode.A) && (_rigidBody.position.x - _leftRightStep) > _leftRightLimits[0])
        {
            _rigidBody.MovePosition(_rigidBody.position + Vector2.left * _leftRightStep);
            rotation = 1;
        }
        else 
        {
            rotation = _currentRotation;
        }
        

        if (_currentRotation != rotation )
        {
            MovementRotation(rotation);
            _currentRotation = rotation;
        }

    }

    // FUNCTION TO CHECK IF WE ARE DRAGGED BY TREE OR TURTLE, IF WE're NOT THEN RESSET POSOTION (lOOSE LIFE)
    IEnumerator CheckGround()
    {
        yield return new WaitForFixedUpdate();
        if(!_enteredDragObject)
        {
            if (_rowPosition > _midRow && _rowPosition < _endRow)
            {
                ResetPosition(true);
            }
            else 
            {
                _dragVelocity = Vector2.zero;
            }
        }
    }

    // WHEN COLIDE WITH DEATH LAYER (CAR) THEN LOOSE LIFE, WHEN COLLIDE WITH DRAG LAYER ( TREES, TURTLES) THEN INHERIT MOVING OBJECT VELOCITY
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.layer == _deathLayer)
        {
            ResetPosition(true);
        }

        else if (other.gameObject.layer == _dragLayer)
        {   
            _enteredDragObject = true;
            _dragVelocity = other.gameObject.GetComponent<MoovingObject>().Velocity * Vector2.right;

        }
    }

    // ALSO DETECT WHEN STILL INSIDE DRAG-TYPE OBJECT/COLLIDER TO ENSURE THAT FROGS POSITION WON'T GET RESETED 
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == _dragLayer)
        {   
            _enteredDragObject = true;
            _dragVelocity = other.gameObject.GetComponent<MoovingObject>().Velocity * Vector2.right;
        }
    }

    void Awake()
    {
        _rigidBody = this.GetComponent<Rigidbody2D>();
        _initPosition = this.gameObject.transform.position;

    }

    void Start()
    {
        // GET NUMBER OF Y-DIRECTION ROWS
        _vertGridSize = _verticalGrid.Count;
        _basePoints = 0;

        //SET UP REFS TO TEXT UI
        scoreCount = gm.scoreCount;
        lifesCount = gm.lifesCount;

        _scores = 0;
        _scoreText = scoreCount.GetComponent<TMP_Text>();
        _scoreText.text = "TOTAL SCORE: " + _scores.ToString();
        _lifesText = lifesCount.GetComponent<TMP_Text>();
        _lifesText.text = "LIVES: " + _healthPoints.ToString();
        _timer = gameTimer.GetComponent<TImer>();
        _highestPoint = 0;

        // RESET TIMER WHEN FROG IS CREATED
        _timer.TimeReset();
    }

    void FixedUpdate()
    {
        //ASSUME THAT NOT ON THE DRAG-TYPE OBJECT (TURTLE, TREE)
        _enteredDragObject = false;

        // THEN CHANGE THAT ASSUMPTION ACCORDINGLY
        StartCoroutine(CheckGround());


    }

    void Update()
    {
        // MOVE FROG, UPDATE SCORE AND ALLOW FOR L CTRL  AND L ALT COMBINATION TO WIN LEVEL (CHEAT CODE)
        MovementControl();
        ScoreSystem();
        if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl) && _cheatAllowed)
        {
            StartCoroutine(CheatCode());
        }
    }
}
