using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int _version = 0;
    public PlayerData _playerData = null;
    
    private static SaveData _instance;
    public static SaveData Instance
    {
        get
        {
            if (null == _instance) 
            {
                _instance = new SaveData();
            }
            return _instance;
        }
    }

    public SaveData()
    {
        _playerData = new PlayerData();
    }

    public void Load(string dataName)
    {
        SaveData loadData = (SaveData)SerializationManager.Load(dataName);
        if(null != loadData)
        {
            _instance = loadData;
            //_instance = new SaveData();
        }
        else
        {
            _instance = new SaveData();
        }
    }

    public bool Save(string dataName)
    {
        return SerializationManager.Save(dataName, (object)_instance);
    }
}

[System.Serializable]
public class PlayerData
{
    public int _version = 0;
    public string _playerName = "";
    public int _score = 0;
    public int _playerModelIndex = 0;
    public int _tutorialCompleted = 0;
    public int _hp = 100;
}