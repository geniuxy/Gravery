using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string id;
    public bool isActive;
    [SerializeField] private float inactiveRadius;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
            CheckIfDeactivateCheckpoint();
    }

    private void CheckIfDeactivateCheckpoint()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, inactiveRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
                DeactivateCheckpoint();
        }
    }

    // 这个注释是为了一次性的生成检查点的Id
    [ContextMenu("Generate checkpoint Id")]
    public void GenerateId()
    {
        id = Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            ActivateCheckpoint();
    }

    public void ActivateCheckpoint()
    {
        if (isActive)
            return;
        AudioManager.instance.PlaySFX(4);
        isActive = true;
        anim.SetBool("Active", true);
    }

    public void DeactivateCheckpoint()
    {
        isActive = false;
        anim.SetBool("Active", false);
    }
}