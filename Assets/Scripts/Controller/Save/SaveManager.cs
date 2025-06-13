using System.Collections.Generic;
using System.IO; 
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private Dictionary<int, bool> _dictionaryState = new();
    private string _filePath;
    [SerializeField] private bool _persistence;

    private void Awake()
    {
        _filePath = Path.Combine(Application.persistentDataPath, "environment_state.json"); 
        LoadEnvironmentState();
    }

    private void LoadEnvironmentState()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                string json = File.ReadAllText(_filePath);
                _dictionaryState = JsonConvert.DeserializeObject<Dictionary<int, bool>>(json);
                Debug.Log("Environment state loaded from file:\n" + json);
            }
            catch (IOException ex)
            {
                Debug.LogError("Failed to load environment state: " + ex.Message);
            }
        }
        else
        {
            Debug.Log("Environment state file not found.");
        }
    }
    public void SaveEnvironmentState()
    {
        if (!_persistence)
            return;

        string json = JsonConvert.SerializeObject(_dictionaryState);

        try
        {
            File.WriteAllText(_filePath, json);
            Debug.Log("Environment state saved to file:\n" + json);
        }
        catch (IOException ex)
        {
            Debug.LogError("Failed to save environment state: " + ex.Message);
        }
    }

    public void CheckState(IEnvironmentState environmentState)
    {
        if (_dictionaryState.TryGetValue(environmentState.Hash, out var value))
        {
            environmentState.ChangeState(value);
        }
        else
        {
            _dictionaryState.Add(environmentState.Hash, false);
            environmentState.ChangeState(false);
        }
    } 
    public void ChangeState(int hash, bool state)
    {
        if (_dictionaryState.ContainsKey(hash))
        {
            _dictionaryState[hash] = state;
        }
        else
        {
            _dictionaryState.Add(hash, false);
        }
    }
} 