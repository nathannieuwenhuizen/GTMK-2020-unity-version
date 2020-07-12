using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{

    public static int HighScore
    {
        set
        {
            PlayerPrefs.SetInt("highscore", value);
        }
        get
        {
            return PlayerPrefs.GetInt("highscore", 0);
        }
    }


    public static bool Vibration
    {
        set
        {
            PlayerPrefs.SetInt("Vibration", value == true ? 1 : 0);
            if (value)
            {
                Handheld.Vibrate();
            }

        }
        get
        {
            return PlayerPrefs.GetInt("Vibration", 1) == 1;
        }
    }

}
