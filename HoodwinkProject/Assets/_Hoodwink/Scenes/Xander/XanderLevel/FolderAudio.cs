using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderAudio : MonoBehaviour
{
    public AudioSource folderOpen;
    public AudioSource folderClose;

    void OpenFolder()
    {
        folderOpen.Play();
    }
    void CloseFolder()
    {
        folderClose.Play();
    }
}
