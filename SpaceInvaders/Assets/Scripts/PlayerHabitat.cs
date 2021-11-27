using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHabitat : MonoBehaviour
{
    //We create a public variable to hold our PlayerHabitat bool
    public bool playerHabitatLeft;

    private void Start()
    {
        flipHabitat();
    }

    void flipHabitat()
    {
        if (playerHabitatLeft == false)
        {
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
