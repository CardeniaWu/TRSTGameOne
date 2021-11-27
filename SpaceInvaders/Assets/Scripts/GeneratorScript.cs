using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    public bool playerGeneratorRight;
    
    // Start is called before the first frame update
    void Start()
    {
        flipGenerator();
    }

    void flipGenerator()
    {
        if (playerGeneratorRight == false)
        {
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
