using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject scoreCount;
    public int pointsPerRow;
    public int pointsPerBase;
    public int pointsPerTimeLeft;
    public GameObject BasePoints;
    public GameObject lifesCount;
    public GameObject gameTimer;
    public GameObject levelCount;
    private TMP_Text _levelText;
    public GameObject verticalGrid;
    public GameObject movementDetector;
    public GameObject frogPrefab;
    public float frogLeftRighStep;
    public GameObject carPrefab;
    public GameObject treePrefab;
    public GameObject turtlePrefab;
    public float timeToPass;
    public int numberOfLifes;
    private float _carLength;
    private float _treeLength;
    private float _turtleLength;
    public GameObject leftBound;
    public GameObject rightBound;
    public GameObject leftSpawnX;
    public GameObject rightSpawnX;
    public int numOfCarRows = 4;
    public int numOfRiverRows = 4;
    public GameObject bases;
    private List<float> _jumpYCoordinates;
    private Frog _frogSc;
    private List<MoovingObject> _listOfPulledTurtles;
    private List<MoovingObject> _listOfPulledCars;
    private List<MoovingObject> _listOfPulledTrees;
    private bool _generateMovement = true;
    private int _gameLevel;
    private IEnumerator[] _arrayOfCoroutines;
    private bool _deactivateOnForce = false;

    // DEACTIVATE MOVING OBJECT BY FORCE, USED WHEN LEVEL CHANGES
    public bool DeactivateOnForce
    {
        get{return _deactivateOnForce;}
    }

    // GET Y COORDINATES OF POINTS WHICH DETERMINE THE Y DIRECTION GRID FOR FROG MOVEMENT (CHESS BOARD, BUT IN Y ONLY)
    void GetGridCoordinatesY()
    {
        _jumpYCoordinates = new List<float>();

        foreach (Transform child in verticalGrid.transform)
        {
            _jumpYCoordinates.Add(child.position.y);
        }
    }

    // RESET BASE POINTS (GREEN ZONES) MEANING CHANGE SPRITE TO INIT AND SET EMPTY FIELD TO TRUE
    void ResetBasePoints()
    {
        foreach (Transform child in BasePoints.transform)
        {
            child.GetComponent<Base>().ResetBase();
        }
    }

    // PULL OBJECT TO REUSABLE LIST OF OBJECT INSTEAD OF CREATING NEW ONES
    public void PullObject(MoovingObject objectSc)
    {
        objectSc.ActivateObject(false);

        if(objectSc.ObjectType == 0)
        {
            _listOfPulledCars.Add(objectSc);
        }
        else if (objectSc.ObjectType == 1)
        {
            _listOfPulledTrees.Add(objectSc);
        }
        else 
        {
            _listOfPulledTurtles.Add(objectSc);
        }
        
    }

    // GENERATE SINGLE MOVING OBJECT ( CAR, TREE, TURTLE, WITH CORRESPONDING LABELS 0, 1, 2)
    void GenerateSingleObject(float objectSpeed, bool rightDirection, Vector2 position, GameObject prefab, int label)
    {
        List<MoovingObject> listOfPulledObjects;
        if (label == 0)
        {
            listOfPulledObjects = _listOfPulledCars;
        }
        else if (label == 1)
        {
            listOfPulledObjects = _listOfPulledTrees;
        }
        else 
        {
            listOfPulledObjects = _listOfPulledTurtles;
        }


        if (listOfPulledObjects.Count == 0)
        {
            MoovingObject tmpObjectSc = Instantiate(prefab, position, Quaternion.identity).GetComponent<MoovingObject>();
            tmpObjectSc.SetBounds(leftSpawnX.transform.position.x, rightSpawnX.transform.position.x);
            tmpObjectSc.SetGameManagerRef = this;
            tmpObjectSc.ObjectType = label;
            tmpObjectSc.ActivateObject(false);
            listOfPulledObjects.Add(tmpObjectSc);
        }

        MoovingObject objectSc = listOfPulledObjects[0];
        listOfPulledObjects.RemoveAt(0);
        objectSpeed = rightDirection ? objectSpeed : - objectSpeed;
        objectSc.InitializeObject(position, objectSpeed);

    }

    // FUNCTION THAT WILL MANAGE MOVEMENT IN ONE ROW ONLY ( STREET OR WATER), CALCULATES SPACES BETWEEN OBJECTS, CONFIGURES SIZES OF ONE BLOCK OF OBJECTS ETC
    IEnumerator OneRowMovementGenerator(int rowNumber, bool rightDirection, int objectsInOneBlock, float objectsLenBetweenBlocks, float velocity, GameObject prefab, float objectLength, int label)
    {
        
        Vector2 basePosition = new Vector2(0, _jumpYCoordinates[rowNumber]);
        basePosition.x = rightDirection ? leftSpawnX.transform.position.x : rightSpawnX.transform.position.x;
        Vector2 currentPosition;
        int objectOffset = rightDirection ? -1 : 1;
        
        while (_generateMovement)
        {   
            int i = 0;
            currentPosition = basePosition;
            while (i < objectsInOneBlock)
            {
                
                GenerateSingleObject(velocity, rightDirection, currentPosition, prefab, label);
                currentPosition.x += objectLength * objectOffset;
                i ++;
            }

            yield return new WaitForSeconds(objectLength * (objectsLenBetweenBlocks + objectsInOneBlock) / velocity); 
        }

    }

    // NOT ONLY STREET BUT IN GENERAL MANAGER OF MOVEMENT, WE CAN DETERMINE MOVEMENT SPEED, DIRECTION, TYPE OF PREFABS, DIRECTION, EAISILY CAN BE USED TO MAKE FUTURE LEVELS EVEN MORE DIFFICULT
    void StreetManager(int level)
    {
        // Labels: 0 - Car, 1 - Tree, 2 - Turtle
        float difficulty = 1 - (2 - level) * .5f;

        _arrayOfCoroutines[0] = OneRowMovementGenerator(1, true, 3, 4, 2 * difficulty, carPrefab, _carLength, 0);
        _arrayOfCoroutines[1] = OneRowMovementGenerator(2, false, 4, 2, 1 * difficulty, carPrefab, _carLength, 0);
        _arrayOfCoroutines[2] = OneRowMovementGenerator(3, true, 2, 4, 3 * difficulty, carPrefab, _carLength, 0);
        _arrayOfCoroutines[3] = OneRowMovementGenerator(4, false, 2, 2, 1 * difficulty, carPrefab, _carLength, 0);


        _arrayOfCoroutines[4] = OneRowMovementGenerator(6, false, 4, 2, 2 * difficulty, turtlePrefab, _turtleLength, 2);
        _arrayOfCoroutines[5] = OneRowMovementGenerator(7, true, 3, 1, 3 * difficulty, treePrefab, _treeLength, 1);
        _arrayOfCoroutines[6] = OneRowMovementGenerator(8, false, 1, 2, 1 * difficulty, treePrefab, _treeLength, 1);
        _arrayOfCoroutines[7] = OneRowMovementGenerator(9, true, 1, 3, 3 * difficulty, treePrefab, _treeLength, 1);
    }

    // SET UP FOR NEW LEVEL, INCREASE LEVEL COUNT, RESET BASES, DEACTIVATE OLD MOVING OBJECTS, UPDATE UI, STOP OLD COORUTINES, UPDATE STREET MANAGER (RIGHT NOW ONLY VELOCITIES, IN FUTURE DIRECTIONS, CHANGE PREFABS ETC), AND START NEW COROUTINES
    // ITS A COROUTINE SINCE WE MUST WAIT ONE FRAME FOR OLD OBJECTS TO GET DEACTIVATED
    IEnumerator SetUpLevel()
    {
        _gameLevel ++;
        ResetBasePoints();
        _deactivateOnForce = true;
        yield return null;
        _deactivateOnForce = false;
        
        _levelText.text = "LEVEL: " + _gameLevel.ToString();
        if (_gameLevel  > 1)
        {
            for (int i = 0; i < _arrayOfCoroutines.Length; i ++)
            {
                StopCoroutine(_arrayOfCoroutines[i]);
            }            
        }

        StreetManager(_gameLevel);

        for (int i = 0; i < _arrayOfCoroutines.Length; i ++)
        {
            StartCoroutine(_arrayOfCoroutines[i]);
        }
    }

    // START SETUP FOR NEW LEVEL COROUTINE
    public void NewLevel()
    {
        StartCoroutine(SetUpLevel());
    }

    // WHEN LOSE ALL HEALTH
    public void GameLost()
    {
        SceneManager.LoadScene(0);
    }

    // SET UP ORU FROG, FEED INFORMATION ABOUT BOARD ,  LAYER NAMES, REFERENCE TO GAME MANAGER, TIMER, SETUP HEALTH POINTS 
    public void SetUpFrog()
    {
        _frogSc = Instantiate(frogPrefab, new Vector3(0, _jumpYCoordinates[0], 0), Quaternion.identity).GetComponent<Frog>();
        _frogSc.RowPosition = 0;
        _frogSc.LeftRightStep = frogLeftRighStep;
        _frogSc.VerticalGrid = _jumpYCoordinates;
        _frogSc.LeftRightLimits = new float[] {leftBound.transform.position.x, rightBound.transform.position.x};
        _frogSc.SetLayers(LayerMask.NameToLayer("deathLayer"), LayerMask.NameToLayer("dragLayer"));
        _frogSc.SetRowNumbers(0, numOfCarRows + 1, numOfCarRows + numOfRiverRows + 2, numOfCarRows, numOfRiverRows);
        _frogSc.BasePoints = bases.transform.childCount;
        _frogSc.SetGameManagerRef = this;
        _frogSc.lifesCount = lifesCount;
        _frogSc.scoreCount = scoreCount;
        _frogSc.gameTimer = gameTimer;
        _frogSc.HealthPoints = numberOfLifes;
        gameTimer.GetComponent<TImer>().SetFrogRef = _frogSc;
    }

    void Awake()
    {
        // INITIALIZE LISTS, GET LENGTHS OF PREFABS, SETUP REFS, LAYERNAMES
        GetGridCoordinatesY();
        gameTimer.GetComponent<TImer>().MaxTime = timeToPass;
        movementDetector.GetComponent<Detector>().SetGameManagerRef = this;
        movementDetector.GetComponent<Detector>().SetLayers = new int[] {LayerMask.NameToLayer("deathLayer"), LayerMask.NameToLayer("dragLayer")};

        _listOfPulledTurtles = new List<MoovingObject>();
        _listOfPulledCars = new List<MoovingObject>();
        _listOfPulledTrees = new List<MoovingObject>();
        _arrayOfCoroutines = new IEnumerator[numOfCarRows + numOfRiverRows];

        _carLength = carPrefab.GetComponent<CapsuleCollider2D>().size.x * carPrefab.transform.localScale.x;
        _treeLength = treePrefab.GetComponent<CapsuleCollider2D>().size.x * treePrefab.transform.localScale.x;
        _turtleLength = turtlePrefab.GetComponent<BoxCollider2D>().size.x * turtlePrefab.transform.localScale.x;

        _gameLevel = 0;
        _levelText = levelCount.GetComponent<TMP_Text>();
        
    }

    void Start()
    {
        //AT THE BEGINING START FIRST LEVEL
        NewLevel();
    }


    void Update()
    {
        // EXIT TO MENU WHEN ESC PRESSED
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }       
    }
}
