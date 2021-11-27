using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameHandler : MonoBehaviour
{
    [Header("Pause System")]
    //We create our variables for the pause menu and the bool to evaluate whether game is paused or not
    public GameObject pauseMenu;
    public bool isPaused;

    [Header("Timer System")]
    //We create a timer to track time between rounds and a time limit between rounds
    public float nonRoundTime = 0.0f;
    public float nonRoundTimeLimit = 30.0f;

    [Header("Level Count System")]
    [SerializeField]
    private GameObject lvlCountTxt;
    //We create our variables for global and round time as well as level count and round limit time
    [SerializeField]
    private float globalTime = 0.0f;
    [SerializeField]
    private float roundTime = 0.0f;
    [SerializeField]
    private float roundTimeLimit = 240.0f;
    [SerializeField]
    private int levelCount = 0;
    //We create a bool to track whether the round is active at any given time.
    public bool roundActive;
    //We create a bool to see if this is the tutorial level
    private bool isTutorialLvl= true;
    //We create a variable to hold our animator for the level notification anim
    [SerializeField]
    private Animator lvlChangeNotification;
    //We create a list of ints to hold the enemy counts we want to output per the level count
    private List<int> _enemyCounts;
    //We create two lists to hold the time we want to spawn our large enemies and small enemies
    private List<float> _seSpawnTime;
    private List<float> _leSpawnTime;
    //We create two floats to hold the values between which we want all our enemies to spawn in during a level
    private float begEnemySpawn = 15.0f;
    private float endEnemySpawn = 0.0f;

    [Header("Enemy Instantiation")]
    //We create variables to hold our small enemy and large enemy prefabs
    [SerializeField]
    private Transform _sePrefab;
    [SerializeField]
    private Transform _lePrefab;
    //We create an int to hold the # of SE & LEs spawned
    private int numOfActiveSE = 0;
    private int numOfActiveLE = 0;

    [Header("SE Spawn System")]
    //We create variables to hold our small enemy spawn points and a list to hold those variables
    private List<Transform> _SESpawnPoints;
    private Transform sesp1L;
    private Transform sesp2L;
    private Transform sesp3R;
    private Transform sesp4R;
    //We create an empty transform to hold our SE spawn point value
    private Transform seSpawnPoint;
    //Bool to test which side of the screen the SE spawned on
    public bool seLSpawn;

    [Header("LE Spawn System")]
    //We create a list to hold the spawnpoint variables and create our public GameObject variables to hold our spawnpoint GameObjects
    private List<Transform> _LEspawnpoints;
    private Transform lesp1L;
    private Transform lesp2L;
    private Transform lesp3R;
    private Transform lesp4R;
    //We create a transform variable to hold the value of the LE spawn point
    private Transform leSpawnpoint;

    /*
    3. Create a spawning system
        a. 

    */


    private void Start()
    {
        //We assign our SE spawn points to transform values for the corresponding game objects
        sesp1L = GameObject.Find("SESpawnLeft").GetComponent<Transform>();
        sesp2L = GameObject.Find("SESpawnLeft1").GetComponent<Transform>();
        sesp3R = GameObject.Find("SESpawnRight").GetComponent<Transform>();
        sesp4R = GameObject.Find("SESpawnRight1").GetComponent<Transform>();

        //We assign our LE spawn points to transform values for the corresponding game objects
        lesp1L = GameObject.Find("LESpawn1").GetComponent<Transform>();
        lesp2L = GameObject.Find("LESpawn2").GetComponent<Transform>();
        lesp3R = GameObject.Find("LESpawn3").GetComponent<Transform>();
        lesp4R = GameObject.Find("LESpawn4").GetComponent<Transform>();
        
        //We initialize the list that will hold our SE spawnpoints
        _SESpawnPoints = new List<Transform>();

        //We initialize the list that will hold our LE spawnpoint
        _LEspawnpoints = new List<Transform>();

        //We initialize the list that will hold our enemy count out per level
        _enemyCounts = new List<int>();
        _enemyCounts.Add(0);
        _enemyCounts.Add(0);

        //We initialize the lists for our spawn timers
        _seSpawnTime = new List<float>();
        _leSpawnTime = new List<float>();

        //Then we add the spawnpoints to the list
        AddToSEList(sesp1L, sesp2L, sesp3R, sesp4R);

        //Then we add the spawnpoints to the list
        AddToLEList(lesp1L, lesp2L, lesp3R, lesp4R);

        //We ensure round one begins as soon as the game starts
        RoundStart();
    }


    private void FixedUpdate()
    {
        //We set globalTime equal to Time.fixedDeltaTime to track how long the player plays and survives
        globalTime += Time.fixedDeltaTime;

        //We check to see if a round is currently active. If it is, we ascribe roundtime to Time.fixedDeltaTime
        if (roundActive)
        {
            roundTime += Time.fixedDeltaTime;
        } else
        {
            //Here we ascribe nonRoundTime to Time.fixedDeltaTime to track the time between rounds and start the next when the limit is passed
            nonRoundTime += Time.fixedDeltaTime;
        }

        //We check to see if the roundTime exceeds the roundTimeLimit. If it does, we call the RoundEnd function
        if (roundTime >= roundTimeLimit && levelCount <= 4)
        {
            RoundEnd();
        }

        //If nonRoundTime exceeds nonRoundTimeLimit the next round starts
        if (nonRoundTime >= nonRoundTimeLimit)
        {
            nonRoundTime = 0.0f;
            RoundStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePause();
        }

        //We check the round time and set the lvlChangeNotification to false if it exceeds 5 seconds
        if (roundTime > 5)
        {
            lvlChangeNotification.SetBool("WaveNotificationStart", false);
        }

        if (roundTime >= 10)
        {
            lvlCountTxt.GetComponent<TextMeshProUGUI>().text = levelCount.ToString();
        }

        //We check each value in _seSpawnTime against roundTime and spawn a small enemy if the round time exceeds the value
        if (roundActive && _seSpawnTime != null && numOfActiveSE < _enemyCounts[0])
        {
            for (int i = 0; i + 1 <= _seSpawnTime.Count; i++)
            {
                if (_seSpawnTime[i] <= roundTime)
                {
                    SpawnSmallEnemy();
                    _seSpawnTime.RemoveAt(i);
                }
            }
        }

        //We check each value in _leSpawnTime against roundTime and spawn a large enemy if the round time exceeds the value
        if (roundActive && _leSpawnTime != null && numOfActiveLE < _enemyCounts[1])
        {
            for (int i = 0; i + 1 <= _leSpawnTime.Count; i++)
            {
                if (_leSpawnTime[i] <= roundTime)
                {
                    SpawnLargeEnemy();
                    _leSpawnTime.RemoveAt(i);
                }
            }
        }
    }

    //We create a function to evaluate whether the game is paused and determine the action thereof
    public void GamePause()
    {
        if (isPaused == false)
        {
            //We change isPaused to true, set pauseMenu to active and change our timescale to 0
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            //We change isPaused to false, deactivate our pause menu and return the timescale to 1
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }


    }

    //We create a function to send us back to the Main Menu and set the game time back to 1
    public void MainMenu()
    {
        SceneManager.LoadScene("StartScene");
        Time.timeScale = 1f;
    }

    //We create a function to exit the game
    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }

    //We create a function to Restart the state of our game and set the game time back to 1
    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
        Time.timeScale = 1f;
    }

    //We create a function that starts the round
    public void RoundStart()
    {
        if (isTutorialLvl)
        {
            //We do not increase the level count on the tutorial level but we still count round time and turn isTutorialLvl to false
            roundTime += Time.fixedDeltaTime;
            roundActive = true;
            isTutorialLvl = false;
        } else
        {
            //We increase the level count, ascribe roundTime to Time.fixedDeltaTime and turn our bool roundActive true
            levelCount++;
            roundTime += Time.fixedDeltaTime;
            roundActive = true;
        }

        //We begin the lvl change notification anim
        lvlChangeNotification.SetBool("WaveNotificationStart", true);

        //We set our enemy spawn count for the level
        DetermineEnemyCountPerLvl();

        //We set our spawn frequency
        SetSpawnFrequency();
    }

    //We create a function that ends the round
    public void RoundEnd()
    {
        //We reset our roundTime to 0.0f and set roundActive to false
        roundTime = 0.0f;
        roundActive = false;
        numOfActiveSE = 0;
        numOfActiveLE = 0;
    }

    //We create a function that spawns in our Large Enemy prefab
    private void SpawnLargeEnemy()
    {
        //We create a local variable to hold our largeEnemy after instantiation
        Transform largeEnemy = Instantiate(_lePrefab);

        //We set the position to a randomized start position
        largeEnemy.position = RandomizeLEStartPosition().position;

        numOfActiveLE++;
    }

    //We create a function that spawns in our Small Enemy prefab
    private void SpawnSmallEnemy()
    {
        //We create a local variable to hold our smallEnemy after instantiation
        Transform smallEnemy = Instantiate(_sePrefab);

        //We set the position to a randomized start position 
        smallEnemy.position = RandomizeSEStartPosition().position;
        
        //If the start position is on the left, we set the seLSpawn bool to true, else false
        if (smallEnemy.position == sesp1L.position || smallEnemy.position == sesp2L.position)
        {
            seLSpawn = true;
        } else
        {
            seLSpawn = false;
        }

        numOfActiveSE++;
    }

    //Nifty AddToList function
    void AddToSEList(params Transform[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            _SESpawnPoints.Add(list[i]);
        }
    }

    //Nifty AddToList function
    void AddToLEList(params Transform[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            _LEspawnpoints.Add(list[i]);
        }
    }

    private Transform RandomizeSEStartPosition()
    {
        if (_SESpawnPoints != null)
        {
            //We declare a random number between 0 & 3
            int randNumber = Random.Range(0, 3);

            //Then we use that # to tell our SmallEnemy which spawnpoint it should spawn on.
            seSpawnPoint = _SESpawnPoints[randNumber];
        }

        return seSpawnPoint;
    }

    //We initialize our spawn point by randomizing between four initial spawn points.
    public Transform RandomizeLEStartPosition()
    {
        if (_LEspawnpoints != null)
        {
            //We declare a random number between 0 & 3
            int randNumber = Random.Range(0, 3);

            //Then we use that # to tell our SmallEnemy which spawnpoint it should spawn on.
            leSpawnpoint = _LEspawnpoints[randNumber];
        }

        return leSpawnpoint;
    }


    //We create a function to check the level count and output a certain level 
    private void DetermineEnemyCountPerLvl()
    {
        //This switch statement takes the level count and outputs values for the enemies to be spawned per level
        //_enemyCounts[0] holds the value for small enemies spawning while _enemyCounts[1] holds the value for the large enemies spawning
        switch (levelCount)
        {
            case 0:
            _enemyCounts[0] = 1;
            _enemyCounts[1] = 0;
            //Debug.Log($"The small enemy count is {_enemyCounts[0]} while the large enemy count is {_enemyCounts[1]}");
            break;

            case 1:
            _enemyCounts[0] = 5;
            _enemyCounts[1] = 0;
            //Debug.Log($"The small enemy count is {_enemyCounts[0]} while the large enemy count is {_enemyCounts[1]}");
            break;

            case 2:
            _enemyCounts[0] = 10;
            _enemyCounts[1] = 0;
            //Debug.Log($"The small enemy count is {_enemyCounts[0]} while the large enemy count is {_enemyCounts[1]}");
            break;

            case 3:
            _enemyCounts[0] = 12;
            _enemyCounts[1] = 1;
            //Debug.Log($"The small enemy count is {_enemyCounts[0]} while the large enemy count is {_enemyCounts[1]}");
            break;

            case 4:
            _enemyCounts[0] = 15;
            _enemyCounts[1] = 2;
            //Debug.Log($"The small enemy count is {_enemyCounts[0]} while the large enemy count is {_enemyCounts[1]}");
            break;

            case 5:
            _enemyCounts[0] = 20;
            _enemyCounts[1] = 5;
            //Debug.Log($"The small enemy count is {_enemyCounts[0]} while the large enemy count is {_enemyCounts[1]}");
            break;
        }
    }

    //We create a function to determine the start and end time of when enemies should spawn and then sets a random value in between that range for when 
    //the SEs and LEs will spawn
    private void SetSpawnFrequency()
    {
        //This switch statement sets a differing end enemy spawn time per level so levels with less enemies have quicker spawn rates than those with more enemies
        switch (levelCount)
        {
            case 0:
            endEnemySpawn = 15.0f;
            break;

            case 1:
            endEnemySpawn = 30.0f;
            break;

            case 2:
            endEnemySpawn = 45.0f;
            break;

            case 3:
            endEnemySpawn = 60.0f;
            break;

            case 4:
            endEnemySpawn = 75.0f;
            break;

            case 5: 
            endEnemySpawn = 120.0f;
            break;
        }

        if (_enemyCounts[0] > 0)
        {
            //We take the beginning and end spawn times and use them to randomly assign times that we'll choose to spawn in the small enemies
            for (int i = 0; i < _enemyCounts[0]; i++)
            {
                float randTime = Random.Range(begEnemySpawn, endEnemySpawn);
            
                if (_seSpawnTime.Count <= i)
                {
                    _seSpawnTime.Add(randTime);
                } else
                {
                    _seSpawnTime[i] = randTime;
                }
            }
        }

        if (_enemyCounts[1] > 0)
        {
            //We take the beginning and end spawn times and use them to randomly assign times that we'll choose to spawn in the large enemies
            for (int i = 0; i < _enemyCounts[1]; i++)
            {
                float randTime = Random.Range(begEnemySpawn, endEnemySpawn);

                if (_leSpawnTime.Count <= i)
                {
                    _leSpawnTime.Add(randTime);
                } else
                {
                    _leSpawnTime[i] = randTime;
                }
            }
        }
    }
}
