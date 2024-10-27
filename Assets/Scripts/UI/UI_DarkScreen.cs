using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DarkScreen : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void FadeIn() => anim.SetTrigger("FadeIn");

    public void FadeOut() => anim.SetTrigger("FadeOut");
}