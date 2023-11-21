using UnityEngine;
using Cinemachine;
using JetBrains.Annotations;
using System.Collections;

public class H_Spectating : MonoBehaviour
{
    public CinemachineVirtualCamera spectatorCam;

    public GameObject[] spectatorTargets;

    public GameObject transitionCam;

    public H_PlayerUI playerUI;

    public AudioSource camChange;

    bool isSpectating;
    int targetIndex;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            isSpectating = !isSpectating;
        }

        if (isSpectating)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                targetIndex++;

                if (targetIndex > spectatorTargets.Length - 1)
                {
                    targetIndex = 0;
                }

                SetSpectatingTarget(targetIndex);
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                targetIndex--;

                if (targetIndex < 0)
                {
                    targetIndex = spectatorTargets.Length - 1;
                }

                SetSpectatingTarget(targetIndex);
            }
        }
    }

    public void EnableSpectating()
    {
        StartCoroutine(SpectatingTransition());
    }

    IEnumerator SpectatingTransition()
    {
        transitionCam.SetActive(true);

        foreach (var ob in playerUI.brain.hideWhenSpectating)
        {
            ob.layer = LayerMask.NameToLayer("Hidden");
        }

        yield return new WaitForSeconds(1f);

        H_TransitionManager.instance.FadeIn(0.5f);

        yield return new WaitForSeconds(2.25f);

        H_TransitionManager.instance.FadeOut(1f);

        playerUI.brain.cosmetics.ShowPlayer();
        playerUI.brain.playerUI.ShowSpectatorUI();

        transitionCam.SetActive(false);

        isSpectating = true;

        spectatorCam.gameObject.SetActive(true);

        SetSpectatingTarget(0);
    }

    public void DisableSpectating()
    {
        isSpectating = false;

        spectatorCam.gameObject.SetActive(false);

        foreach (var ob in playerUI.brain.hideWhenSpectating)
        {
            if (playerUI.brain.hideForLocalPlayer.Contains(ob))
            {
                ob.layer = LayerMask.NameToLayer("LocalPlayer");
            }
            else
            {
                ob.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public void SetSpectatingTarget(int index)
    {
        spectatorTargets = FindSpectatorTargets();

        spectatorCam.Follow = spectatorTargets[index].transform;
        spectatorCam.LookAt = spectatorTargets[index].transform.GetChild(0);

        camChange.Play();

        //Debug.LogWarning(spectatorTargets[index].transform.GetComponentInParent<H_PlayerBrain>());
        playerUI.ChangeSpectator(spectatorTargets[index].transform.GetComponent<H_SpectatorInfo>().spectatorName);
    }

    public GameObject[] FindSpectatorTargets()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("SpectatorTarget");

        return targets;
    }
}
