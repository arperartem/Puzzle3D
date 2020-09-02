using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishWindow : MonoBehaviour
{
    public void LoadNextLevel()
    {
        LevelController.ToNextLevel?.Invoke();
        Destroy(gameObject);
    }
}
