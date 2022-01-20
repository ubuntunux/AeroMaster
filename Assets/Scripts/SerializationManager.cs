using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager
{
    const string saveDir = "saves";

    public static string GetDataDirectory()
    {
        return Application.persistentDataPath + "/" + saveDir;
    }

    public static string GetDataFilePath(string dataName)
    {
        return GetDataDirectory() + "/" + dataName + ".dat";
    }


    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();
        string saveDirectory = GetDataDirectory();
        if(false == Directory.Exists(saveDirectory))
        {
            Debug.Log("CreateDirectory: " + saveDirectory);
            Directory.CreateDirectory(saveDirectory);            
        }

        string saveFilePath = GetDataFilePath(saveName);
        Debug.Log("Save: " + saveFilePath);

        FileStream file = File.Create(saveFilePath);
        formatter.Serialize(file, saveData);
        file.Close();
        return true;
    }

    public static object Load(string loadName)
    {
        string loadFilePath = GetDataFilePath(loadName);
        if(false == File.Exists(loadFilePath))
        {
            Debug.Log("File not exists: " + loadFilePath);
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(loadFilePath, FileMode.Open);
        try
        {
            Debug.Log("Load: " + loadFilePath);
            object loadData = formatter.Deserialize(file);
            file.Close();
            return loadData;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}.", loadFilePath);
            file.Close();
            return null;
        }
    }

    public static bool Delete(string dataName)
    {
        string dataFilePath = GetDataFilePath(dataName);
        if(File.Exists(dataFilePath))
        {
            Debug.Log("Delete: " + dataFilePath);
            File.Delete(dataFilePath);
            return true;
        }

        Debug.Log("Delete file not exists: " + dataFilePath);
        return false;
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }
}
