using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repository : MonoBehaviour
{
    private Vector2 movement;
    private Transform PBase;
    private Vector3 diff;
    private bool lSpawn;
    private float rotationSpeed;
    private Rigidbody2D se_Rigidbody2D;

    //All the scripts below were various methods we used to unsuccessfully rotate a 2D sprite to face the direction it was moving in 

    void FixedUpdate()
    {
        //This first method came from the following: https://youtu.be/gs7y2b0xthU?t=306
        //We haven't discovered why but it was offsetting our sprite rotation by roughly 90 degrees (I believe this 90 degree inaccuracy is due to the use of Vector3.forward when the toRotation variable is set
        //Our second method does the same thing without this and it works for the left spawns and flips the right spawns by 180 degrees

        movement = this.transform.position - PBase.position;

        if (movement != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }








        //This method works for the left but does not work on the right. The direction on the right is 180 degrees off so to make it work, we'd need to learn how to edit a Quaternion to amend the z rotation by 180 degrees
        //We'd need to learn how to modify Quaternions properly to apply a 180 amendment to the outcome on right spawns to make this work
        //I could not find the documentation on where I found this one:/

        //We create a variable to hold our movement vector
        movement = this.transform.position - PBase.position;

        //We create a new Quaternion named targetRotation and use the movement to set its LookRotation
        Quaternion targetRotation = Quaternion.LookRotation(movement);

        //We modify the new Quaternion to set a rotate towards value
        targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.fixedDeltaTime);

        //We use MoveRotation to adjust our rotation based on the values formulated by targetRotation
        se_Rigidbody2D.MoveRotation(targetRotation);








        //This method was math heavy but easier to manipulate. It would have been a good solution had it not been offsetting the sprite rotation by 20 degrees or so.
        //Originally it was working on the left and flipping the right by 180 degrees just like the first script. Something happened after we implemented the if statement and it stopped working altogether giving all the spawns a 20 degree offset
        //making identifying and resolving the issue challenging
        //This comes from: https://forum.unity.com/threads/quaternion-lookrotation-sometimes-backwards.338681/

        diff = this.transform.position - PBase.position;
        diff.Normalize();
        float rotZ = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;

        this.transform.rotation = Quaternion.Euler(0, 0, rotZ);

        //We were getting the above script to work at first for the left two spawn points before we broke it somehow
        //However it would be 180 degrees off from what we wanted on the right, so the below script checked the spawn and applied to math accordingly
        if (lSpawn)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, rotZ);

        } else
        {
            this.transform.rotation = Quaternion.Euler(0, 0, rotZ - 180);
        }



        //Forum post: https://forum.unity.com/threads/weird-rotation-behaviour-with-quaternion-lookrotation.504726/
        //Has to be commented out to prevent compiler errors
        /*
        float distanceThreshold = 0.2f;
        vector3 upDirection = PathToFollow.path_objs[currentWaypointID].position - transform.position;

        //only turn towards the current waypoint when far enough away (prevents the snapping)
        if (lookDirection.sqrMagnitude >= distThreshold * disThreshold)
        {
            // this will force the object to always look forward, but orient itself so its pivoting to the current waypoint (i.e. locking to the z axis)
            rotation = Quaternion.LookRotation(Vector3.forward, upDirection);

            //use rotateTowards for a smooth rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * maxDegressPerSecond);

            // or continue to use Slerp for that elastic rotation
            //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        */

        //How to move a gameobject the incorrect way, not using rigidbodies: Place in FixedUpdate
        //transform.position = Vector2.MoveTowards(transform.position, PBase.position, speed * Time.deltaTime);
    }
}
