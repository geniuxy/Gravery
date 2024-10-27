using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_Hotkey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private TextMeshProUGUI myText;
    private KeyCode myKeyCode;
    private Transform myEnemy;
    private Blackhole_Skill_Controller _myBlackhole;
    public void SetupHotkey(KeyCode _newHotkey,Transform _myEnemy,Blackhole_Skill_Controller myBlackhole)
    {
        sr = GetComponent<SpriteRenderer>();
        myKeyCode = _newHotkey;
        myEnemy = _myEnemy;
        _myBlackhole = myBlackhole;
        myText = GetComponentInChildren<TextMeshProUGUI>();
        myText.text = _newHotkey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(myKeyCode))
        {
            _myBlackhole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
