using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    None,
    AirCraft
}

public enum DestroyType
{
    Explosion,
    ImpactWater
}

public class CharacterManager : MonoBehaviour
{
    public GameObject[] _modelObjects;
    public GameObject _prefabPlayer;

    GameObject _player;

    // Singleton instantiation
    static CharacterManager _instance;
    public static CharacterManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.FindObjectOfType<CharacterManager>();
            }
            return _instance;
        }
    }

    public void InitializeCharacterManager()
    {
        _player = Instantiate(_prefabPlayer, Vector3.zero, Quaternion.identity);
        _player.transform.SetParent(transform, false);
    }

    public Player GetPlayer()
    {
        return _player.GetComponent<Player>();
    }

    public int GetCharacterModelCount()
    {
        return _modelObjects.Length;
    }

    public GameObject CreateCharacterModel(int index)
    {
        return (index < _modelObjects.Length) ? _modelObjects[index] : null;
    }
}
