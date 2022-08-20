using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
    //PauseAction action;
    public static bool isPaused;
    public GameObject menu;
    [Header("Keybind")]
    public KeyCode pauseKey = KeyCode.Escape;

    private void Awake() {
        //action = new PauseAction();
    }
    private void Update() {
        if (Input.GetKeyDown(pauseKey)) {
            DeterminePause();
        }
        //action.Pause.PauseGame.performed 
        //+= _ => DeterminePause();
    }

    private void DeterminePause() {
        if (isPaused) {
            ResumeGame();
        } else {
            PauseGame();
        }
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;
        menu.SetActive(false);
        Cursor.visible = false;
    }  
    public void PauseGame() {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        isPaused = true;
        menu.SetActive(true);
        Cursor.visible = false;
    }  
    // private void OnEnable() {
    //     action.Enable();
    // }
    // private void OnDisable() {
    //     action.Disable();
    // }

    public void LoadMenu() {
        Debug.Log("Loading menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame() {
        Debug.Log("Quiting game...");
        Application.Quit();
    }
}
