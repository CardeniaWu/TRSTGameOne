using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SETurretLogic : MonoBehaviour
{
    [Header("Fire Variables")]
    //This variable is our time to fire
    [SerializeField]
    private float timeToFire = 2.0f;
    //This variable is our time
    [SerializeField]
    private float timeLoop = 0.0f;
    //This variable holds the prefabed missile
    [SerializeField]
    private Transform prefabedmissile;
    //This variable holds the missile after instantiation
    private Transform instantiatedMissile;
    [SerializeField]
    private bool turnOnTurretRay = false;
    [SerializeField]
    public Transform missileSpawnPos;
    //isFiring bool for testing purposes
    [SerializeField]
    private bool isFiring = false;

    private void FixedUpdate()
    {
        //We set timeLoop to Time.fixedDeltaTime
        timeLoop += Time.fixedDeltaTime;
    }

    private void Update()
    {
        //We check to see if the timeloop meets or exceeds timeToFire and run the FireTurret function if it does 
        if (timeLoop >= timeToFire && isFiring)
        {
            FireTurret();
        }
    }

    private void FireTurret()
    {
        //Here we instantiate the missile and set its position and rotation according to the turret that spawned it
        instantiatedMissile = Instantiate(this.prefabedmissile, missileSpawnPos.position, transform.rotation);
        //We set the timeloop back to 0 to allow it to fire again
        timeLoop = 0.0f;
    }

    
    // Here we draw Gizmo rays from the turrets to enable us to see the prospective missile trajectory
    private void OnDrawGizmos()
    {
        if (turnOnTurretRay)
        {
            Gizmos.color = Color.red;
            Vector3 direction = transform.TransformDirection(Vector3.right) * 25;
            Gizmos.DrawRay(transform.position, direction);
        }
    }

    
}
