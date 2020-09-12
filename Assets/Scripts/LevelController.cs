using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private const string LEVEL_KEY = "LAST_LEVEL";
    private const string SCORE_KEY = "_SCORE_KEY";

    [SerializeField] private Transform[] _levels;

    private Transform _currentLevel;

    private void OnEnable()
    {
        GameEvents.LoadNewLevel += LoadNextLevel;
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
            CurrentLevel = 0;
            Debug.LogError("Unknow level!");
            //return;
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

        GameEvents.OnLoadNewLevel?.Invoke();
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

    public static int GetBestScore()
    {
        if (PlayerPrefs.HasKey(CurrentLevel + SCORE_KEY))
        {
            return PlayerPrefs.GetInt(CurrentLevel + SCORE_KEY);
        }
        else
        {
            return 0;
        }
    }
    public static void SaveBestScore(int score)
    {
        PlayerPrefs.SetInt(CurrentLevel + SCORE_KEY, score);
    }

    public static int CurrentLevel { get; private set; } = 0;

    private void OnDisable()
    {
        GameEvents.LoadNewLevel -= LoadNextLevel;
    }
}
