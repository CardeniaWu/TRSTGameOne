using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //We create an exit function for our exit button
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }

    //We create a start function to start the game
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
