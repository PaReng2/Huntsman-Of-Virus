using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int playerHP;
    public float playerAttackPower;
    public float playerAttackRate;
    public float playerMoveSpeed;
    public float playerAttackRange;


}

public class PlayerStatusManager : MonoBehaviour
{
    public static PlayerStatusManager instance;

    public PlayerData playerData = new();

    private string path;
    private string fileName = "/save";

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        path = Application.persistentDataPath + fileName;
        Debug.Log(path);

        LoadData();
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(playerData);

        File.WriteAllText(path, data);
    }

    public void LoadData()
    {
        if (!File.Exists(path))
        {
            SaveData();
        }

        string data = File.ReadAllText(path);

        playerData = JsonUtility.FromJson<PlayerData>(data);    
    }
    
}
