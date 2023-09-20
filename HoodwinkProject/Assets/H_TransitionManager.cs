using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class H_TransitionManager : MonoBehaviour
{
    public static H_TransitionManager instance { get; private set; }

    public Image fadePanel;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void SetBlack()
    {
        fadePanel.color = Color.black;
    }

    public void SetClear()
    {
        fadePanel.color = Color.clear;
    }

    public void FadeIn(float speed)
    {
        StartCoroutine(FadeInPanel(speed));
    }

    public void FadeOut(float speed)
    {
        StartCoroutine(FadeOutPanel(speed));
    }

    public IEnumerator FadeInPanel(float fadeSpeed)
    {
        while (fadePanel.color.a < 1)
        {
            Color panelColor = fadePanel.color;
            float fadeAmount = fadePanel.color.a + (fadeSpeed * Time.deltaTime);

            panelColor = new Color(panelColor.r, panelColor.g, panelColor.b, fadeAmount);
            fadePanel.color = panelColor;
            yield return null;
        }
    }

    public IEnumerator FadeOutPanel(float fadeSpeed)
    {
        while (fadePanel.color.a > 0)
        {
            Color panelColor = fadePanel.color;
            float fadeAmount = fadePanel.color.a - (fadeSpeed * Time.deltaTime);

            panelColor = new Color(panelColor.r, panelColor.g, panelColor.b, fadeAmount);
            fadePanel.color = panelColor;
            yield return null;
        }
    }

}
