using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool needEncrypt;
    private string encryptKey = "xygod";

    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _needEncrypt)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        needEncrypt = _needEncrypt;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
            string dataTostore = JsonUtility.ToJson(_data, true);
            if (needEncrypt)
                EncryptData(ref dataTostore);
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(dataTostore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("error to save game data in the path of: " + fullPath + "\n" + e);
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if (needEncrypt)
                    EncryptData(ref dataToLoad);
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("error to load game data from the path of: " + fullPath + "\n" + e);
            }
        }

        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public void EncryptData(ref string _data)
    {
        char[] data = _data.ToCharArray();
        for (int i = 0; i < data.Length; i++)
            data[i] = (char)(data[i] ^ encryptKey[i % encryptKey.Length]);
        _data = new string(data);
    }
}