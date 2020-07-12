using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public CanvasGroup scores;
    public Text scoreText;
    public Text highScoreText;
    public Text introText;

    private void Start()
    {
        introText.gameObject.SetActive(true);
        UpdateHighScoreText(Settings.HighScore);
        UpdateScoreText(0);
    }
    public void Play()
    {
        introText.gameObject.SetActive(false);
        scores.gameObject.SetActive(true);
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
