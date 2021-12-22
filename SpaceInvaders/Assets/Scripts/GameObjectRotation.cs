using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectRotation : MonoBehaviour
{
    /*
    1. Create a variable as our rotation goal (quaternion) **done**
    2. Create a variable to hold our forward point (transform of an empty child object set at the head of our parent game object) **done**
    3. Create a variable to hold our back point (transform of an empty child object set at the butt of our parent game object) **done**
    4. Create variable to hold our point of interest (transform) **done**
    5. Create a variable to hold the value of our vector from fwd to back point **bToFVector** (Vector2) **done**
    6. Create a variable to hold our bToFAngle (float) **done**
    7. Create a variable to hold our turnRate speed (float) **done**
    8. Create a variable to hold our newRotation (Quaternion) **done**
    9. Convert the bToFDistance into an angle (use code example below): **done**
        **movement will be replaced by bToFDistance**
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
    10. Create a variable to hold the vector between the parents transform and the point of interest **parentToPoIVector** (Vector2) **done**
    11. Create a variable to hold our parentToPoIAngle (float) **done**
    12. Convert the parentToPoIDistance to an angle (use code example below): **done**
        **movement will be replaced by parentToPoIDistance
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
    13. Check to see whether the bToFAngle and parentToPoIAngle align **done**
    14. If not, set the gameobject to have a rotationGoal (using Quaternion.LookRotation) of the parentToPoIAngle
    15. Use the rotationGoal to set a newRotation using Quaternion.RotateTowards & the turn rate speed variable
    16. Use this.localRotation = newRotation; to set the rotation of the parent object in motion 

    */
    
    
    [Header("Rotation Systems")]
    //We create a variable to hold our rotation goal
    private Quaternion rotationGoal;
    //We create a variable to hold our forward point
    [SerializeField]
    private Transform fPoint;
    //We create a variable to hold our back point
    [SerializeField]
    private Transform bPoint;
    //We create a variable to hold our point of interest
    [SerializeField]
    private Transform poi;
    //We create a variable to hold the vector from fwd to back point
    private Vector2 bToFVector;
    //We create a variable to hold our back to forward angle
    private float bToFAngle;
    //We create a variable to hold our turnRate speed
    private float turnRate;
    //We create a variable to hold our newRotation
    private Quaternion newRotation;
    //We create a variable to hold the vector between the parents transform and the point of interest
    private Vector2 parentToPoIVector;
    //We create a variable to hold our parentToPoIAngle
    private float parentToPoIAngle;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //We set bToFVector equal to the fPoint position minus the bPoint position
        bToFVector = fPoint.position - bPoint.position;
        //We convert this bToFvector into an angle
        bToFAngle = Mathf.Atan2(bToFVector.y, bToFVector.x) * Mathf.Rad2Deg;

        //We set parentToPoIVector equal to the PoI's position minus the parent transforms position
        parentToPoIVector = poi.position - this.transform.position;
        //We convert this parentToPoIVector into an angle
        parentToPoIAngle = Mathf.Atan2(parentToPoIVector.y, parentToPoIVector.x) * Mathf.Rad2Deg;

        if (bToFAngle != parentToPoIAngle)
        {
            
            
            //rotationGoal = Quaternion.LookRotation();
        }
    }
}
