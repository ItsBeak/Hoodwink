using UnityEngine;
using Cinemachine;

public class H_Spectating : MonoBehaviour
{
    public CinemachineFreeLook spectatorCam;

    public GameObject[] spectatorTargets;

    public H_PlayerUI playerUI;

    bool isSpectating;
    int targetIndex;

    void Update()
    {
        if (isSpectating)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                targetIndex++;

                if (targetIndex > spectatorTargets.Length - 1)
                {
                    targetIndex = 0;
                }

                SetSpectatingTarget(targetIndex);
            }

            if (Input.GetKeyDown(KeyCode.A))
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
        spectatorCam.LookAt = spectatorTargets[index].transform;

        //Debug.LogWarning(spectatorTargets[index].transform.GetComponentInParent<H_PlayerBrain>());
        playerUI.ChangeSpectator(spectatorTargets[index].transform.GetComponentInParent<H_PlayerBrain>().playerName, spectatorTargets[index].transform.GetComponentInParent<H_PlayerBrain>().coatColour);
    }

    public GameObject[] FindSpectatorTargets()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("SpectatorTarget");

        return targets;
    }
}
