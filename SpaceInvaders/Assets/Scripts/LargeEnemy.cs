using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeEnemy : MonoBehaviour
{
    [Header("Movement")]
    //We create a variable to hold our player base transform
    private Transform PBase;
    //We create a variable to hold our desired waypoint
    [SerializeField]
    private Transform targetWPTransform;
    //We create a variable to hold our speed variable
    [SerializeField]
    private float speed;
    //We create a variable to hold a rigidbody2D
    private Rigidbody2D le_Rigidbody2D;
    //We create a variable to hold the collider that will serve as our barrier
    private CircleCollider2D leBarrier;
    //We create a bool to tell us whether to move forward or not
    private bool shouldMove = true;
    //We create a Vector2 to hold our movement value
    private Vector2 movement;
    //We create two animator variables to hold our thrust animations
    [SerializeField]
    private Animator rightThrust;
    [SerializeField]
    private Animator leftThrust;
    //We create a variable to hold our velocity
    private Vector2 leVelocity;
    //We create a variable to hold our turnRate
    private float turnRate = 1.0f;

    [Header("Round End Logic")]
    //We create a bool to hold the value to tell us whether the round is ending or not
    private bool roundEnd = false;
    //We create a variable to hold our location that the LE spawns in so that we may return to it when the round is over
    private Transform startPositionTransform;

    
    [Header("Shooting System")]
    //We create a two variables to hold our bay door animators 
    [SerializeField]
    private Animator leBayDoorT;
    [SerializeField]
    private Animator leBayDoorB;
    //We create a variable to hold our fire animator which comes after the bay doors are open
    [SerializeField]
    private Animator LEFire;
    //We create a transform variable to hold our prefabbed laser
    [SerializeField]
    private Transform prefabLaser;
    //And one to hold the instantiated laser
    private Transform instantiatedLaser;
    //We create a transform variable to hold the position of the gun from which we will shoot the laser 
    [SerializeField]
    private Transform laserSpawn;
    //And a bool to tell us whether we should shoot the laser or not
    private bool shouldFire = true;
    


    [Header("Debug Assistance")]
    //We create a variable to allow us to turn debug assistance on or off
    [SerializeField]
    private bool debugAssist;
    [SerializeField]
    private bool turnOnGizmo;

    // Start is called before the first frame update
    void Start()
    {
        //We will grab our variables for the PBase and leBarrier
        PBase = GameObject.Find("NuclearReactor").GetComponent<Transform>();
        leBarrier = GameObject.Find("LEBarrier").GetComponent<CircleCollider2D>();
        
        //We set our rigidbody variable
        le_Rigidbody2D = GetComponent<Rigidbody2D>();

        //We set the current position to our startPosition
        startPositionTransform = this.transform;
    }

    void Update()
    {
        //We pull the bool from our Gamehandler script which tells us whether the round is active or not and set roundEnd to the opposite of that
        if (GameObject.Find("GameHandler").GetComponent<GameHandler>().roundActive == false)
        {
            roundEnd = true;
        } else
        {
            roundEnd = false;
        }

        ActionsDuringRoundUpdate();
        ActionsAtEndOfRoundUpdate();
    }
    
    void FixedUpdate()
    {
        //We calculate our movement value
        if (roundEnd == false)
        {
            targetWPTransform = PBase;
        } 
        else
        {
            targetWPTransform = startPositionTransform;
        }

        movement = targetWPTransform.position - this.transform.position;
        
        //We set leVelocity equal to movement normalized * speed
        leVelocity = (movement).normalized * speed;

        //We check if shouldMove is true or false and call our function MoveToLEWaypoint if it is true
        if (shouldMove)
        {
            MoveToLEWaypoint();
            rightThrust.SetBool("Moving", true);
            leftThrust.SetBool("Moving", true);
        }

        //If we are moving, we align our rotation with the direction we are heading
        if (le_Rigidbody2D.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        //Debug assistance message that tells us the target rotation in euler angles
        if (debugAssist)
        {
            Debug.Log($"LE target rotation is {this.transform.eulerAngles}");
        }

        //We check to see whether the round is at its end and whether the bay doors have shut
        //If so, we enact our rotation logic
        if (roundEnd && leBayDoorT.GetCurrentAnimatorStateInfo(0).IsName("Waiting"))
        {
            //step size is equal to speed times frame time
            float singleStep = turnRate * Time.deltaTime;

            //Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, movement, singleStep, 0.0f);

            //Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void ActionsDuringRoundUpdate()
    {
        //We check to see whether we should continue moving. If we are close enough to the base, we stop and initiate our bay door anims
        if (leBarrier.OverlapPoint(transform.position) && roundEnd == false)
        {
            shouldMove = false;
            leBayDoorT.SetBool("InRange", true);
            leBayDoorB.SetBool("InRange", true);
            rightThrust.SetBool("Moving", false);
            leftThrust.SetBool("Moving", false);

            //We reset velocity to be 0 to stop all movement
            le_Rigidbody2D.velocity = this.transform.position - this.transform.position;
        } else
        {
            leBayDoorB.SetBool("InRange", false);
            leBayDoorT.SetBool("InRange", false);
        }

        //Once the bay doors are in firing position, we heat up our laser gun
        if (leBayDoorB.GetCurrentAnimatorStateInfo(0).IsName("LEBDBFiring") && roundEnd == false)
        {
            LEFire.SetBool("FireReady", true);
        } else
        {
            LEFire.SetBool("FireReady", false);
        }

        //Once the laser gun is heated, we fire our laser once
        if (LEFire.GetCurrentAnimatorStateInfo(0).IsName("LEBOOM") && shouldFire)
        {
            FireLaser();
            shouldFire = false;
        }

        //Once the laser gun fires once and returns to heating up, we return our shouldFire variable to true to fire once again
        if (LEFire.GetCurrentAnimatorStateInfo(0).IsName("LEFire"))
        {
            shouldFire = true;
        }
    }

    private void ActionsAtEndOfRoundUpdate()
    {
        //We test if the round has ended. If it has, we reverse the direction of the bay door anims and start the cool down anim of the LE laser
        if (roundEnd)
        {
            LEFire.SetBool("RoundEnd", true);
            leBayDoorB.SetFloat("Direction", -1);
            leBayDoorT.SetFloat("Direction", -1);
        }

        //If the round ends in the middle of the laser charging up, we reverse the direction of the anim
        if (roundEnd && LEFire.GetCurrentAnimatorStateInfo(0).IsName("LEFire"))
        {
            LEFire.SetFloat("Direction", -1);
        }

        //Once the LEFire anim has returned to it's waiting state, we begin our leBayDoor closing anims
        if (roundEnd && LEFire.GetCurrentAnimatorStateInfo(0).IsName("Waiting"))
        {
            leBayDoorT.SetBool("RoundEnd", true);
            leBayDoorB.SetBool("RoundEnd", true);
        }
    }

    /*
    1. Rotate 180 degrees to face in the opposite direction
    2. Move Forward till off screen
    3. Delete game object once it reaches the enemyOperatingArea barrier

    */


    //Here we tell our ship to move towards the player base or leave the player view at the end of the round
    void MoveToLEWaypoint()
    {
        le_Rigidbody2D.velocity = leVelocity;
    }

    //Pew Pew
    void FireLaser()
    {
        //Convert the quaternion to euler angles, then modify the Z to get the rotation we want
        Vector3 myEulerAngles = this.transform.rotation.eulerAngles;
        Quaternion laserRotation = Quaternion.Euler(myEulerAngles.x, myEulerAngles.y, myEulerAngles.z - 90);

        //Instantiate the laser
        instantiatedLaser = Instantiate(prefabLaser, laserSpawn.position, laserRotation);
    }

    // Here we draw Gizmo rays from the turrets to enable us to see the prospective missile trajectory
    private void OnDrawGizmos()
    {
        if (turnOnGizmo)
        {
            Gizmos.color = Color.red;
            Vector3 direction = transform.TransformDirection(Vector3.right) * 25;
            Gizmos.DrawRay(transform.position, direction);
        }
    }
}
