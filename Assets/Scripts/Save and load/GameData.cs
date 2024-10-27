using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class GameData
{
    public int currency;

    public SerializableDictionary<string, bool> skillTree;

    public SerializableDictionary<string, int> inventory;
    public List<string> deviceIds;

    public SerializableDictionary<string, bool> checkpointDict;
    public string closestCheckpointId;

    public int lostCurrencyAmount;
    public float lostCurrencyPosX;
    public float lostCurrencyPosY;

    public SerializableDictionary<string, float > volumeSettings;

    public SerializableDictionary<string, bool> toggles;

    public GameData()
    {
        currency = 10000;

        skillTree = new SerializableDictionary<string, bool>();

        inventory = new SerializableDictionary<string, int>();
        deviceIds = new List<string>();

        checkpointDict = new SerializableDictionary<string, bool>();
        closestCheckpointId = String.Empty;

        lostCurrencyAmount = 0;
        lostCurrencyPosX = 0;
        lostCurrencyPosY = 0;

        volumeSettings = new SerializableDictionary<string, float>();

        toggles = new SerializableDictionary<string, bool>();
    }
}