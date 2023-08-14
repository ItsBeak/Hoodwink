using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class H_Bootstrap : MonoBehaviour
{
    [Scene] public string mainMenuScene;
    public float bootstrapTime;
    [HideInInspector] public bool hasPressedPlay;
    void Start()
    {
        DontDestroyOnLoad(this);
        Invoke("StartGame", bootstrapTime);
    }

    void StartGame()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
