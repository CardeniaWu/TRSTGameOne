using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientWithGround : MonoBehaviour
{
    //We create a variable to hold the transform of the ground
    private Transform groundVar;

    // Start is called before the first frame update
    void Start()
    {
        groundVar = GameObject.Find("Background").GetComponent<Transform>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 myEulerAngles = groundVar.rotation.eulerAngles;
        Quaternion HBarRotation = Quaternion.Euler(myEulerAngles.x, myEulerAngles.y, myEulerAngles.z);

        this.transform.rotation = HBarRotation;
    }
}
