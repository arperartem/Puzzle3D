using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private const string LEVEL_KEY = "LAST_LEVEL";

    [SerializeField] private Transform[] _levels;

    public static System.Action ToNextLevel;
    private Transform _currentLevel;

    private void OnEnable()
    {
        ToNextLevel += LoadNextLevel;
    }

    private void Start()
    {
        CurrentLevel = LoadLevel();
        CreateLevel();
    }

    private void CreateLevel()
    {
        if(CurrentLevel < 0 || CurrentLevel >= _levels.Length)
        {
            Debug.LogError("Unknow level!");
            return;
        }

        if(_currentLevel != null)
        {
            Destroy(_currentLevel.gameObject);
        }

        _currentLevel = Instantiate(_levels[CurrentLevel]);
    }

    private void LoadNextLevel()
    {
        if(CurrentLevel + 1 >= _levels.Length)
        {
            CurrentLevel = 0;
        }
        else
        {
            CurrentLevel++;
        }
        
        SaveLevel(CurrentLevel);
        CreateLevel();
    }

    private int LoadLevel()
    {
        if (PlayerPrefs.HasKey(LEVEL_KEY))
        {
            return PlayerPrefs.GetInt(LEVEL_KEY);
        }
        else return 0;
    }

    private void SaveLevel(int level) => PlayerPrefs.SetInt(LEVEL_KEY, level);

    public static int CurrentLevel { get; private set; } = 0;

    private void OnDisable()
    {
        ToNextLevel -= LoadNextLevel;
    }
}
