using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextFX : MonoBehaviour
{
    [SerializeField] private float upSpeed;
    [SerializeField] private float disappearSpeed;
    [SerializeField] private float colorLooseSpeed;
    [SerializeField] private float lifeTime;
    private float lifeTimer;
    private TextMeshPro text;

    void Start()
    {
        text = GetComponent<TextMeshPro>();
        lifeTimer = lifeTime;
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position,
            new Vector2(transform.position.x, transform.position.y + 1), upSpeed * Time.deltaTime);

        if (lifeTimer < 0)
        {
            float alpha = text.color.a - colorLooseSpeed * Time.deltaTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

            if (text.color.a < 50)
                upSpeed = disappearSpeed;

            if (text.color.a <= 0)
                Destroy(gameObject);
        }
    }
}