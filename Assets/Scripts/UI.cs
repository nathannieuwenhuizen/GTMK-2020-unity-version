using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{

    public CanvasGroup scores;
    public CanvasGroup menu;
    public CanvasGroup controls;
    public Text scoreText;
    public Text highScoreText;
    public Text introText;

    public GameObject world;

    public GameObject singleButton;
    public GameObject pairButtons;
    private void Start()
    {
        introText.gameObject.SetActive(true);
        UpdateHighScoreText(Settings.HighScore);
        scores.alpha = 0;
        scores.interactable = false;
        world.gameObject.SetActive(false);
        UpdateScoreText(0);
    }

    public void ShowSingleButton()
    {
        singleButton.SetActive(true);
        pairButtons.SetActive(false);
    }
    public void ShowPairButtons()
    {
        singleButton.SetActive(false);
        pairButtons.SetActive(true);
    }
    public void FirstMove()
    {
        introText.gameObject.SetActive(false);
        scores.gameObject.SetActive(true);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowControls()
    {
        controls.interactable = true;
        menu.interactable = false;

        StartCoroutine(FadeGroup(menu, 0));
        StartCoroutine(FadeGroup(controls, 1));
    }

    public void ShowMenu()
    {
        controls.interactable = false;
        menu.interactable = true;

        StartCoroutine(FadeGroup(menu, 1));
        StartCoroutine(FadeGroup(controls, 0));
    }
    public void Play()
    {
        ShowSingleButton();
        world.SetActive(true);
        scores.interactable = true;
        menu.interactable = false;

        StartCoroutine(FadeGroup(menu, 0));
        StartCoroutine(FadeGroup(scores, 1));
        menu.interactable = false;

    }
    IEnumerator FadeGroup(CanvasGroup group, float val)
    {
        float duration = .5f;
        float index = 0;
        float start = group.alpha;
        while (index < duration)
        {
            index += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, val, index / duration);
            yield return new WaitForFixedUpdate();
        }
        group.alpha = val;
    }
    public void UpdateScoreText(int val)
    {
        scoreText.text = val + "";
    }
    public void UpdateHighScoreText(int val)
    {
        highScoreText.text = val + "";
    }
}
