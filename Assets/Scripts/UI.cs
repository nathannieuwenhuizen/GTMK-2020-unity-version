using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public CanvasGroup scores;
    public CanvasGroup menu;
    public Text scoreText;
    public Text highScoreText;
    public Text introText;

    public GameObject world;
    private void Start()
    {
        introText.gameObject.SetActive(true);
        UpdateHighScoreText(Settings.HighScore);
        scores.alpha = 0;
        world.gameObject.SetActive(false);
        UpdateScoreText(0);
    }
    public void FirstMove()
    {
        introText.gameObject.SetActive(false);
        scores.gameObject.SetActive(true);
    }

    public void Play()
    {
        world.SetActive(true);
        Debug.Log("play");
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
