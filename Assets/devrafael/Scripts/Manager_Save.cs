using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //You can change uint type if you think that the player will call Save() more than 2,147,483,647 times without deleting the save
    public uint saveVersion;
    
    //Another cool variables here
}

public class Manager_Save
{
    private static string savePath = "";
    private static bool isInitialized = false;

    public static PlayerData GetPlayerData { get; private set; } = new PlayerData();

    public static void Init()
    {
        if (isInitialized)
        {
            return;
        }

        //You can change to any name and format like: "/banana.fruit"
        savePath = Application.persistentDataPath + "/save.dat";
        
        isInitialized = true;
    }

    public static void LoadData(System.Action resultCallback = null)
    {
        Init();
        LocalLoad();
    }

    public static void SaveData(System.Action resultCallback = null)
    {
        //Uncomment Init() if will call SaveData() before LoadData() in the game timeline
        //Init();
        GetPlayerData.saveVersion++;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileToCreate = File.Create(savePath);
        binaryFormatter.Serialize(fileToCreate, GetPlayerData);
        fileToCreate.Close();
        
        #if UNITY_EDITOR
        Debug.Log("Saved: " + PlayerDataToJson(GetPlayerData));
        #endif
    }

    public static void DeleteSave(uint newVersion)
    {
        //This resets the saveVersion
        GetPlayerData = new PlayerData();
        SaveData();
    }

    private static void LocalLoad()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileToOpen = File.Open(savePath, FileMode.Open);
            GetPlayerData = (PlayerData)binaryFormatter.Deserialize(fileToOpen);
            fileToOpen.Close();
        }
        
        #if UNITY_EDITOR
        Debug.Log("Loaded: " + PlayerDataToJson(GetPlayerData));
        #endif
    }
    
    #if UNITY_EDITOR
    //Only for debug purposes
    private static string PlayerDataToJson(PlayerData playerData)
    {
        if (playerData == null)
        {
            return JsonUtility.ToJson(new PlayerData());
        }
        else
        {
            return JsonUtility.ToJson(playerData);
        }
    }
    #endif
}
