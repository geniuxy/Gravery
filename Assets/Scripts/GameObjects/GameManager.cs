using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    private Player player;

    [Header("checkpoint info")]
    [SerializeField] private Checkpoint[] checkpoints;
    private string closestCheckpointId;

    [Header("lost currency info")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    private float lostCurrencyPosX;
    private float lostCurrencyPosY;
    public float lostPercentage { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;

        checkpoints = FindObjectsOfType<Checkpoint>();
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
        lostPercentage = 0.1f;
    }

    private void Update()
    {
        if (UI.instance.darkScreen.GetComponent<Image>().color.a * 255 > 1)
            return;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            RestartScene();
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyPosX = player.transform.position.x;
        _data.lostCurrencyPosY = player.transform.position.y;

        if (FindClosestCheckpoint(player.transform.position) != null)
            _data.closestCheckpointId = FindClosestCheckpoint(player.transform.position).id;

        _data.checkpointDict.Clear();
        foreach (var checkpoint in checkpoints)
            _data.checkpointDict.Add(checkpoint.id, checkpoint.isActive);
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);
        LoadLostCurrency(_data);
        LoadActiveCheckpoint(_data);
        LoadPlayerPosition(_data);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyPosX = _data.lostCurrencyPosX;
        lostCurrencyPosY = _data.lostCurrencyPosY;

        if (lostCurrencyAmount != 0)
        {
            GameObject lostCurrency = Instantiate(lostCurrencyPrefab, new Vector2(lostCurrencyPosX, lostCurrencyPosY),
                Quaternion.identity);
            lostCurrency.GetComponent<LostCurrencyController>().lostCurrencyAmount = lostCurrencyAmount;
        }

        // 这一步非常关键，需要清空lostCurrencyAmount，否则会一直保留
        lostCurrencyAmount = 0;
    }

    private void LoadActiveCheckpoint(GameData _data)
    {
        foreach (var pair in _data.checkpointDict)
        {
            foreach (var checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value)
                    checkpoint.ActivateCheckpoint();
            }
        }
    }

    private void LoadPlayerPosition(GameData _data)
    {
        if (_data.closestCheckpointId != null)
            closestCheckpointId = _data.closestCheckpointId;

        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.id == closestCheckpointId && checkpoint.isActive)
            {
                player.transform.position = checkpoint.transform.position;
                break;
            }
        }
    }

    private Checkpoint FindClosestCheckpoint(Vector2 _position)
    {
        Checkpoint closestCheckpoint = null;
        float minDistance = float.MaxValue;

        foreach (var checkpoint in checkpoints)
        {
            float distance = Vector2.Distance(_position, checkpoint.transform.position);
            if (distance < minDistance && checkpoint.isActive)
            {
                minDistance = distance;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }

    public void PauseGame(bool _isPaused) => Time.timeScale = _isPaused ? 0 : 1;
}