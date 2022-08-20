using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    // public void Start() {
    //     Cursor.visible = false;
    // }
    // public void Update() {
    //     Cursor.visible = false;
    // }

    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() {
        Debug.Log("Quitted game..");
        Application.Quit();
    }
}
