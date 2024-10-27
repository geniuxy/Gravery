using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerFX : EntityFX
{
    public bool showPopUpText;

    [Header("Dash Image fx")]
    [SerializeField] private GameObject dashImagePrefab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float fxCreateCooldown;
    private float fxCreateCoolDownTimer;

    [Header("Screen shock")]
    [SerializeField] private float shockModifier;
    public Vector3 catchSwordShockPower;
    public Vector3 highDamageShockPower;
    private CinemachineImpulseSource screenShock;
    public bool canShock;

    protected override void Start()
    {
        base.Start();

        screenShock = GetComponent<CinemachineImpulseSource>();
    }

    private void Update() =>
        fxCreateCoolDownTimer -= Time.deltaTime;

    public void CreateDashImageFX()
    {
        if (fxCreateCoolDownTimer < 0)
        {
            fxCreateCoolDownTimer = fxCreateCooldown;
            GameObject newImageFX = Instantiate(dashImagePrefab, transform.position, transform.rotation);
            newImageFX.GetComponent<DashImageFX>().SetupDashImage(colorLooseRate, sr.sprite);
        }
    }

    public void CanShowPopUpText() => showPopUpText = !showPopUpText;

    public void CanScreenShock() => canShock = !canShock;

    public void ScreenShock(Vector3 _shockPower)
    {
        if (!canShock)
            return;
        screenShock.m_DefaultVelocity = new Vector3(_shockPower.x * player.facingDir, _shockPower.y) * shockModifier;
        screenShock.GenerateImpulse();
    }
}