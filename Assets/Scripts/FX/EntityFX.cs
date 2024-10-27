using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Player player;
    protected GameObject myHealthBar;

    [Header("Flash info")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originMat;

    [Header("Ailment colors")]
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailment particles")]
    [SerializeField] private ParticleSystem igniteParticle;
    [SerializeField] private ParticleSystem chillParticle;
    [SerializeField] private ParticleSystem shockParticle;

    [Header("Hit fx")]
    [SerializeField] private GameObject hitFXPrefab;
    [SerializeField] private GameObject critHitFXPrefab;
    [SerializeField] private ParticleSystem dustFX;

    [Header("Pop up text")]
    [SerializeField] private GameObject popUpTextPrefab;

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        player = PlayerManager.instance.player;

        originMat = sr.material;
    }

    protected IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        sr.color = currentColor;
        sr.material = originMat;
    }

    public virtual void MakeTransparent(bool _isTransparent)
    {
        if (myHealthBar == null)
            myHealthBar = GetComponentInChildren<UI_HealthBar>()?.gameObject;
        myHealthBar?.SetActive(!_isTransparent);
        sr.color = _isTransparent ? Color.clear : Color.white;
    }

    private void RedColorBlink()
    {
        sr.color = sr.color == Color.white ? Color.red : Color.white;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        igniteParticle.Stop();
        chillParticle.Stop();
        shockParticle.Stop();
    }

    public void CreateHitFX(Transform _target, bool _isCrit)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosOffset = Random.Range(-.5f, .5f);
        float yPosOffset = Random.Range(-.5f, .5f);

        GameObject hitPrefab = hitFXPrefab;
        Vector3 hitRotation = new Vector3(0, 0, zRotation);

        if (_isCrit)
        {
            hitPrefab = critHitFXPrefab;

            float yRotation = GetComponent<Entity>().facingDir == -1 ? 180 : 0;
            zRotation = Random.Range(-45, 45);

            hitRotation = new Vector3(0, yRotation, zRotation);
        }

        GameObject newHitFX = Instantiate(hitPrefab,
            _target.position + new Vector3(xPosOffset, yPosOffset), Quaternion.identity);
        newHitFX.transform.Rotate(hitRotation);

        Destroy(newHitFX, .5f);
    }

    public void PlayDustFX() => dustFX?.Play();

    public void CreatePopUpTextFX(string _text, Color _color = default)
    {
        if (!player.fx.showPopUpText)
            return;
        float xOffset = Random.Range(-1f, 1f);
        float yOffset = Random.Range(1f, 3f);
        Vector3 posOffset = new Vector3(xOffset, yOffset, 0);
        GameObject newTextFX = Instantiate(popUpTextPrefab, transform.position + posOffset, Quaternion.identity);
        newTextFX.GetComponent<TextMeshPro>().text = _text;
        newTextFX.GetComponent<TextMeshPro>().color = _color == default ? Color.white : _color;
    }

    # region Ailments FX

    public void IgniteFxFor(float _seconds)
    {
        igniteParticle.Play();

        InvokeRepeating(nameof(IgniteColorFx), .1f, .3f);
        Invoke(nameof(CancelColorChange), _seconds);
    }

    private void IgniteColorFx() => sr.color = sr.color == igniteColor[0] ? igniteColor[1] : igniteColor[0];

    public void ChillFxFor(float _seconds)
    {
        chillParticle.Play();

        InvokeRepeating(nameof(ChillColorFx), .1f, .3f);
        Invoke(nameof(CancelColorChange), _seconds);
    }

    private void ChillColorFx() => sr.color = sr.color == chillColor[0] ? chillColor[1] : chillColor[0];

    public void ShockFxFor(float _seconds)
    {
        shockParticle.Play();

        InvokeRepeating(nameof(ShockColorFx), .1f, .3f);
        Invoke(nameof(CancelColorChange), _seconds);
    }

    private void ShockColorFx() => sr.color = sr.color == shockColor[0] ? shockColor[1] : shockColor[0];

    #endregion
}