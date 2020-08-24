using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ToolBox
{
    public static CubeManager CubeManager;
}

public class CubeManager : MonoBehaviour
{
    [SerializeField] private Transform _ball;
    [SerializeField] private float _speed;
    [Space]
    [SerializeField] private Puzzle _startPoint;
    [SerializeField] private Puzzle _finishPoint;

    [SerializeField] private Transform _puzzlePlainContainer;
    private Puzzle[] _puzzles;

    private PathFinder _pathFinder = new PathFinder();

    private void OnEnable()
    {
        ToolBox.CubeManager = this;
    }

    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            _puzzles = GetComponentsInChildren<Puzzle>();
        }

        Puzzle.IsInteractable = true;
    }

    public void OnCorrectPath()
    {
        List<Puzzle> path = _pathFinder.Find(_startPoint, _finishPoint, _puzzles);
        StartCoroutine(MoveBall(path, 0f));
    }

    private IEnumerator MoveBall(List<Puzzle> targetTiles, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        int i = 0;
        Vector3 targetPos = new Vector3(targetTiles[i].transform.localPosition.x, _ball.transform.localPosition.y ,targetTiles[i].transform.localPosition.z);

        while (true)
        {
            float step = _speed * Time.fixedDeltaTime;
            _ball.transform.localPosition = Vector3.MoveTowards(_ball.transform.localPosition, targetPos, step);

            if (Mathf.Approximately(_ball.transform.localPosition.x, targetPos.x) && Mathf.Approximately(_ball.transform.localPosition.z, targetPos.z))
            {
                if (i + 1 == targetTiles.Count)
                {
                    break;
                }
                else
                {
                    i++;
                    targetPos = new Vector3(targetTiles[i].transform.localPosition.x, _ball.transform.localPosition.y, targetTiles[i].transform.localPosition.z);
                }
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }

        Finish();
    }

    private void Finish()
    {
        LevelController.ToNextLevel?.Invoke();
    }

    public bool CheckPath() => _pathFinder.Find(_startPoint, _finishPoint, _puzzles).Count > 0;
}

