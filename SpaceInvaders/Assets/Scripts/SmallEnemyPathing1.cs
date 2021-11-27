using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemyPathing1 : MonoBehaviour
{
    [Header("Movement & Boundaries")]
    //Transform to hold the Small Enemy waypoint
    private Transform waypoint;
    //We create a bool to hold the value to determine whether we spawned on the left or the right
    bool lSpawn;
    //This is probably the wrong way to do this but I'm creating a bool to differentiate between first loop and second loop. This will enable us to make the spawn move up one spawn and down on the next to give the enemies a zig zag movement
    bool run1;
    //We set a float for speed
    public float speed = .5f;
    //We create a public BoxCollider variable to hold our left, right, top, bottom & base out of bounds boxes
    private BoxCollider2D lOutOfBounds;
    private BoxCollider2D rOutOfBounds;
    private BoxCollider2D topOutOfBounds;
    private BoxCollider2D groundOutOfBounds;
    private BoxCollider2D baseOutOfBounds;
    //This bool is to test whether the SmallEnemy is in the top or ground out of bounds regions and act accordingly
    bool tOutOfBounds = false;
    bool gOutOfBounds = false;
    //We create two variables to track the time from the onset of spawn to 3 seconds later.
    //This allows us to conditionally spawn our SmallEnemy out of bounds while keeping it moving in the correct direction.
    private float timeToMove = 3.0f;
    public float timeFromSpawn = 0.0f;
    //Here we create the variable that will hold our segment prefab
    public Transform waypointPrefab;

    [Header("Round End Logic")]
    //We create a bool to hold the value of whether round is active or not
    private bool roundEnd = false;
    //We create a variable to hold our circle collider 2D which will destroy SEs after they exit the circles area
    private CircleCollider2D seOperatingArea;


    // Start is called before the first frame update
    private void Start()
    {
        //We find and assign our out of bounds variables
        lOutOfBounds = GameObject.Find("lOutOfBounds").GetComponent<BoxCollider2D>();
        rOutOfBounds = GameObject.Find("rOutOfBounds").GetComponent<BoxCollider2D>();
        topOutOfBounds = GameObject.Find("topOutOfBounds").GetComponent<BoxCollider2D>();
        groundOutOfBounds = GameObject.Find("groundOutOfBounds").GetComponent<BoxCollider2D>();
        baseOutOfBounds = GameObject.Find("baseOutOfBounds").GetComponent<BoxCollider2D>();
        seOperatingArea = GameObject.Find("enemyOperatingArea").GetComponent<CircleCollider2D>();

        //Here we instantiate our waypoint before calling the RandomizeEnemyWaypoint function
        waypoint = Instantiate(waypointPrefab);
        RandomizeEnemyWaypoint();

        //We pull the lspawn variable from the GameHandler gameobject 
        lSpawn = GameObject.Find("GameHandler").GetComponent<GameHandler>().seLSpawn;
    }

    // Update is called once per frame
    private void Update()
    {
        //Here we make the waypoint spawn elsewhere when it overlaps with the SmallEnemy gameobject
        if (transform.position == waypoint.position)
        {
            //We call the randomizeEnemyWaypoint function
            RandomizeEnemyWaypoint();
        }

        //Here we tell the system to run the RandomizeEnemyWaypoint function again until it is not in the baseOutOfBounds region
        if (baseOutOfBounds.OverlapPoint(waypoint.position))
        {
            RandomizeEnemyWaypoint();
        }

        //Here we check to see whether our  left out of bounds grid area contains the waypoints position and if the timeFromSpawn exceeds timeToMove
        //Change the value of lSpawn to keep the SmallEnemy in the play field of view if both conditions are true
        if (lOutOfBounds.OverlapPoint(waypoint.position) && timeFromSpawn >= timeToMove && roundEnd == false)
        {
            lSpawn = true;
        }

        //Here we check to see whether our right out of bounds grid area contains the waypoints position and if the timeFromSpawn exceeds timeToMove
        //Change the value of lSpawn to keep the SmallEnemy in the play field of view if both conditions are true
        if (rOutOfBounds.OverlapPoint(waypoint.position) && timeFromSpawn >= timeToMove && roundEnd == false)
        {
            lSpawn = false;
        }

        //Here we test to see whether the waypoints position is out of bounds on top
        if (topOutOfBounds.OverlapPoint(waypoint.position))
        {
            tOutOfBounds = true;
        }
        else
        {
            tOutOfBounds = false;
        }

        //Here we test to see whether the waypoints position is out of bounds on the ground
        if (groundOutOfBounds.OverlapPoint(waypoint.position))
        {
            gOutOfBounds = true;
        }
        else
        {
            gOutOfBounds = false;
        }

        if (GameObject.Find("GameHandler").GetComponent<GameHandler>().roundActive == false)
        {
            roundEnd = true;
        } else
        {
            roundEnd = false;
        }

        if (seOperatingArea.OverlapPoint(waypoint.position) == false)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        //We assign timeFromSpawn to Time.fixeddeltatime
        timeFromSpawn += Time.deltaTime;

        MoveToEWaypoint();
    }

    //Bool list to test all our variables and determine where the SE should head next
    private bool ShouldMoveLeftFI()
    {
        return lSpawn == false && run1 == true && tOutOfBounds == false && gOutOfBounds == false;
    }

    private bool ShouldMoveLeftSI()
    {
        return lSpawn == false && run1 == false && tOutOfBounds == false && gOutOfBounds == false;
    }

    private bool ShouldMoveRightFI()
    {
        return lSpawn == true && run1 == true && tOutOfBounds == false && gOutOfBounds == false;
    }

    private bool ShouldMoveRightSI()
    {
        return lSpawn == true && run1 == false && tOutOfBounds == false && gOutOfBounds == false;
    }

    private bool ShouldMoveUpRight()
    {
        return lSpawn == true && gOutOfBounds == true;
    }

    private bool ShouldMoveDownRight()
    {
        return lSpawn == true && tOutOfBounds == true;
    }

    private bool ShouldMoveUpLeft()
    {
        return lSpawn == false && gOutOfBounds == true;
    }

    private bool ShouldMoveDownLeft()
    {
        return lSpawn == false && tOutOfBounds == true;
    }


    //We use this function to randomize the enemy waypoint... Go figure...
    void RandomizeEnemyWaypoint()
    {
        //Here we create fioats and use them to hold random values that allow us to change the spawn of the waypoints next location
        float x = Random.Range(0.25f, .75f);
        float y = Random.Range(0.25f, .75f);
        int z = 0;


        //Here we check the criteria for where we want to spawn the waypoints position
        if (ShouldMoveUpRight())
        {
            waypoint.position = transform.TransformPoint(-x, y, z);
            return;
        }

        if (ShouldMoveDownRight())
        {
            waypoint.position = transform.TransformPoint(x, y, z);
            return;
        }

        if (ShouldMoveUpLeft())
        {
            waypoint.position = transform.TransformPoint(-x, -y, z);
            return;
        }

        if (ShouldMoveDownLeft())
        {
            waypoint.position = transform.TransformPoint(x, -y, z);
            return;
        }

        if (ShouldMoveLeftFI())
        {
            waypoint.position = transform.TransformPoint(x, -y, z);
            run1 = false;
            return;
        }

        if (ShouldMoveLeftSI())
        {
            waypoint.position = transform.TransformPoint(-x, -y, z);
            run1 = true;
            return;
        }

        if (ShouldMoveRightFI())
        {
            waypoint.position = transform.TransformPoint(x, y, z);
            run1 = false;
            return;
        }

        if (ShouldMoveRightSI())
        {
            waypoint.position = transform.TransformPoint(-x, y, z);
            run1 = true;
            return;
        }
    }

    //Simple script telling our SE where to go
    void MoveToEWaypoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, waypoint.position, speed * Time.deltaTime);
    }
}
