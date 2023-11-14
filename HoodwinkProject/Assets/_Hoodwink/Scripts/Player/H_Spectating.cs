using UnityEngine;
using Cinemachine;
using JetBrains.Annotations;

public class H_Spectating : MonoBehaviour
{
    public CinemachineVirtualCamera spectatorCam;

    public GameObject[] spectatorTargets;

    public H_PlayerUI playerUI;

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
        isSpectating = true;

        spectatorCam.gameObject.SetActive(true);

        SetSpectatingTarget(0);
    }

    public void DisableSpectating()
    {
        isSpectating = false;

        spectatorCam.gameObject.SetActive(false);
    }

    public void SetSpectatingTarget(int index)
    {
        spectatorTargets = FindSpectatorTargets();

        spectatorCam.Follow = spectatorTargets[index].transform;
        spectatorCam.LookAt = spectatorTargets[index].transform.GetChild(0);



        //Debug.LogWarning(spectatorTargets[index].transform.GetComponentInParent<H_PlayerBrain>());
        playerUI.ChangeSpectator(spectatorTargets[index].transform.GetComponent<H_SpectatorInfo>().spectatorName);
    }

    public GameObject[] FindSpectatorTargets()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("SpectatorTarget");

        return targets;
    }
}
