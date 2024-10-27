using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashImageFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private float colorLooseRate;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float alpha = sr.color.a - colorLooseRate * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (alpha <= 0)
            Destroy(gameObject);
    }

    public void SetupDashImage(float _colorLooseRate, Sprite _sprite)
    {
        sr.sprite = _sprite;
        colorLooseRate = _colorLooseRate;
    }
}