﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Dance()
    {
        anim.SetBool("dance", true);
    }
    public void Idle()
    {
        anim.SetBool("dance", false);
    }
}
