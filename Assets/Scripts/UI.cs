using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Text _currScore;
    [SerializeField] private Text _bestScore;

    private void OnEnable()
    {
        GameEvents.OnSwap += UpdateCurrScore;
        GameEvents.OnNewBestScore += UpdateBestScore;
        GameEvents.OnLoadNewLevel += OnNewLevel;
    }

    private void Start()
    {
        _bestScore.text = LevelController.GetBestScore().ToString();
    }

    private void UpdateCurrScore(int countSwaps) => _currScore.text = countSwaps.ToString();
    private void UpdateBestScore(int countSwaps) => _bestScore.text = countSwaps.ToString();

    private void OnNewLevel()
    {
        _currScore.text = string.Empty;
        _bestScore.text = LevelController.GetBestScore().ToString();
    }
}
