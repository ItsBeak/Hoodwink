using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class H_DocumentFax : NetworkBehaviour
{
    [Header("Score Settings")]
    public int scoreChange;

    [Header("Sequence Settings")]
    [Range(0, 20)] public int sequenceLength;
    public H_DocumentFaxButton[] buttons;

    [Header("Fax Settings")]
    public float faxCompleteCooldown;
    [HideInInspector] public bool isOnCooldown;

    [Header("Document Settings")]
    public GameObject documentPrefab;
    public Transform rejectionOutputPosition;

    [Header("Audio")]
    public AudioClip insertDocumentClip;
    public AudioClip rejectDocumentClip;
    public AudioClip faxingClip;
    public AudioClip buttonClip;
    AudioSource source;

    [Header("SyncVars")]
    [SyncVar(hook = nameof(OnDocumentChanged))] public bool containsDocument;
    [SyncVar] bool isCompleted = false;
    [SyncVar] int sequenceCount = 0;
    [SyncVar(hook = nameof(OnSentChanged))] public int documentsSent;

    public Renderer[] sentLights;
    public Material lightOn, lightOff;

    [Header("Debugging")]
    public bool enableDebugLogs;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    [Command(requiresAuthority = false)]
    public void CmdAddDocument()
    {
        containsDocument = true;
    }

    void OnDocumentChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            source.PlayOneShot(insertDocumentClip);

            if (isServer)
            {
                ShuffleButtons();
            }
        }
        else
        {
            RpcClearButtons();

            if (isServer)
            {
                ClearButtons();
            }
        }
    }

    void OnSentChanged(int oldAmount, int newAmount)
    {
        if (newAmount == 1)
        {
            sentLights[0].material = lightOn;
            sentLights[1].material = lightOff;
            sentLights[2].material = lightOff;
            sentLights[3].material = lightOff;

        }

        if (newAmount == 2)
        {
            sentLights[1].material = lightOn;
            sentLights[2].material = lightOff;
            sentLights[3].material = lightOff;
        }

        if (newAmount == 3)
        {
            sentLights[2].material = lightOn;
            sentLights[3].material = lightOff;
        }

        if (newAmount == 4)
        {
            sentLights[3].material = lightOn;
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdButtonPressed(bool isPressedButtonActive)
    {
        RpcButtonPressed();

        if (isCompleted || !containsDocument)
        {
            return;
        }

        if (isPressedButtonActive)
        {
            sequenceCount++;

            if (sequenceCount >= sequenceLength)
            {
                Complete();
                ClearButtons();
            }
            else
            {
                ShuffleButtons();
            }

        }
        else
        {
            Reject();
        }
    }

    [ClientRpc]
    void RpcButtonPressed()
    {
        source.PlayOneShot(buttonClip);
    }

    void ShuffleButtons()
    {
        int newActiveButton = Random.Range(0, buttons.Length);

        RpcShuffleButtons(newActiveButton);
    }

    [ClientRpc]
    void RpcShuffleButtons(int buttonIndex)
    {
        foreach (H_DocumentFaxButton button in buttons)
        {
            button.Deactivate();
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == buttonIndex)
            {
                buttons[i].Activate();
            }
        }
    }

    void ClearButtons()
    {
        RpcClearButtons();
    }

    [ClientRpc]
    void RpcClearButtons()
    {
        foreach (H_DocumentFaxButton button in buttons)
        {
            button.Deactivate();
        }
    }

    void Complete()
    {
        if (!containsDocument)
            return;

        //H_GameManager.instance.CmdUpdateEvidence(scoreChange);
        StartCoroutine(FaxCompleting());

        sequenceCount = 0;

        containsDocument = false;

        ClearButtons();

        RpcComplete();

        documentsSent++;
    }

    IEnumerator FaxCompleting()
    {
        int scoreLeft = scoreChange;

        if (enableDebugLogs)
            Debug.Log("Fax started");

        while (scoreLeft > 0)
        {
            yield return new WaitForSeconds(0.35f);

            H_GameManager.instance.CmdUpdateEvidence(1);
            scoreLeft--;

            if (enableDebugLogs)
                Debug.Log("Removed evidence");
        }
    }

    [ClientRpc]
    void RpcComplete()
    {
        StartCoroutine(FaxCooldown());
    }

    IEnumerator FaxCooldown()
    {
        isOnCooldown = true;

        source.PlayOneShot(faxingClip);

        yield return new WaitForSeconds(faxCompleteCooldown);

        isOnCooldown = false;
    }

    void Reject()
    {
        Vector3 position = rejectionOutputPosition.position;
        Quaternion rotation = rejectionOutputPosition.rotation;
        GameObject document = Instantiate(documentPrefab, position, rotation);

        document.GetComponent<Rigidbody>().AddTorque(Random.Range(-2.5f, 2.5f), Random.Range(-2.5f, 2.5f), Random.Range(-2.5f, 2.5f));
        document.GetComponent<Rigidbody>().AddForce(rejectionOutputPosition.forward * 4, ForceMode.Impulse);

        NetworkServer.Spawn(document);

        containsDocument = false;
        sequenceCount = 0;

        RpcReject();
    }

    [ClientRpc]
    void RpcReject()
    {
        source.PlayOneShot(rejectDocumentClip);
    }

}
