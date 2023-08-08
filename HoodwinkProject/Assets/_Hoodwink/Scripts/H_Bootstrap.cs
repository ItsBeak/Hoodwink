using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class H_Bootstrap : MonoBehaviour
{
    [Scene] public string mainMenuScene;
    [HideInInspector] public bool hasPressedPlay;
    void Start()
    {
        DontDestroyOnLoad(this);
        Invoke("StartGame", 1f);
    }

    void StartGame()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
