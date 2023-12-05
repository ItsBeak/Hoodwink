using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Tutorial : NetworkBehaviour
{

    public bool isOpen;
    public Animator tutorialAnimator;
    public CanvasGroup tutorialCanvas;
    public GameObject taskPage, gadgetPage;

    enum TutorialState
    {
        Closed,
        TaskPage,
        GadgetPage
    }

    TutorialState state;

    private void Update()
    {
        tutorialCanvas.alpha = (state == TutorialState.Closed) ? 0 : 1;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (state == TutorialState.TaskPage)
            {
                NextPage();
            }
            else if (state == TutorialState.GadgetPage)
            {
                CloseTutorial();
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (state == TutorialState.Closed)
            {
                OpenTutorial();
            }
        }
    }

    [ClientRpc]
    public void RpcOpenTutorial()
    {
        if (state == TutorialState.Closed)
        {
            OpenTutorial();
        }
    }

    void OpenTutorial()
    {
        state = TutorialState.TaskPage;
        tutorialAnimator.SetTrigger("TutorialOpen");
        taskPage.SetActive(true);
        gadgetPage.SetActive(false);
    }

    void NextPage()
    {
        taskPage.SetActive(false);
        gadgetPage.SetActive(true);
        state = TutorialState.GadgetPage;

    }

    void CloseTutorial()
    {
        tutorialAnimator.SetTrigger("TutorialClose");
        Invoke(nameof(SetClosed), 1f);
    }

    void SetClosed()
    {
        state = TutorialState.Closed;
        Debug.Log("Closing tutorial");
    }

}
