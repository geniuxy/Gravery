using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool needEncrypt;
    private GameData gameData;
    private List<ISaveManager> saveManagers; // 每一个用到ISaveManager接口的类
    private FileDataHandler fileDataHandler;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;
    }

    private void Start()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, needEncrypt);
        saveManagers = FindAllSaveManagers();

        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = fileDataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("game data is not existed");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        fileDataHandler.Save(gameData);
    }

    public void DeleteArchive()
    {
        fileDataHandler.Delete();
    }

    public bool IsArchiveExist() => fileDataHandler.Load() != null;

    // 找到每一个用到ISaveManager接口的类
    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> managers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();

        return new List<ISaveManager>(managers);
    }
}