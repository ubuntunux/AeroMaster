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
    public GameObject[] _meshObjects;
    public GameObject _prefabPlayer;
    public GameObject _prefabEnemy;

    GameObject _player;
    GameObject _enemy;

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

    void Awake()
    {
        _player = Instantiate(_prefabPlayer, Vector3.zero, Quaternion.identity);
        _player.transform.SetParent(transform, false);

        _enemy = Instantiate(_prefabEnemy, Vector3.zero, Quaternion.identity);
        _enemy.transform.SetParent(transform, false);
    }

    public Player GetPlayer()
    {
        return _player.GetComponent<Player>();
    }

    public int GetCharacterModelCount()
    {
        return _meshObjects.Length;
    }

    public GameObject CreateCharacterModel(int index)
    {
        return (index < _meshObjects.Length) ? Instantiate(_meshObjects[index]) : null;
    }
}
