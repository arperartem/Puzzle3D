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
    [SerializeField] private CubeConfig _cubeConfig;
    [Space]
    [SerializeField] private Transform _ball;
    [Space]
    [SerializeField] private Puzzle _startPoint;
    [SerializeField] private Puzzle _finishPoint;
    [Space]
    [SerializeField] private ParticleSystem[] _finishParticles;
    [Space]
    [SerializeField] private Transform _puzzlePlainContainer;
    private Puzzle[] _puzzles;

    private PathFinder _pathFinder = new PathFinder();

    private void OnEnable()
    {
        ToolBox.CubeManager = this;
    }

    private void Start()
    {
        _puzzles = _puzzlePlainContainer.GetComponentsInChildren<Puzzle>();

        for (int i = 0; i < _puzzles.Length; i++)
        {
            _puzzles[i].Init(_cubeConfig);
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
            float step = Config.SpeedBall * Time.fixedDeltaTime;
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

        StartCoroutine(Finish());
    }

    private IEnumerator Finish()
    {
        PlayFinishParticles();

        GetComponent<AudioSource>().clip = _cubeConfig.Audio.Finish;
        GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(_cubeConfig.DelayNextCube);

        Instantiate(Resources.Load<FinishWindow>("FinishWindow"), GameObject.FindGameObjectWithTag("UI").transform);
    }

    private void PlayFinishParticles()
    {
        for(int i = 0; i < _finishParticles.Length; i++)
        {
            _finishParticles[i].Play();
        }
    }

    public List<Puzzle> GetNeighbors(Puzzle centerPuzzle)
    {
        List<Puzzle> result = new List<Puzzle>();
        for (int i = 0; i < _puzzles.Length; i++)
        {
            if (!_puzzles[i].Interactable) continue;

            if(centerPuzzle.Cell.x + 1 == _puzzles[i].Cell.x && centerPuzzle.Cell.y == _puzzles[i].Cell.y)
            {
                result.Add(_puzzles[i]);
            }
            if (centerPuzzle.Cell.x - 1 == _puzzles[i].Cell.x && centerPuzzle.Cell.y == _puzzles[i].Cell.y)
            {
                result.Add(_puzzles[i]);
            }
            if (centerPuzzle.Cell.x == _puzzles[i].Cell.x && centerPuzzle.Cell.y + 1 == _puzzles[i].Cell.y)
            {
                result.Add(_puzzles[i]);
            }
            if (centerPuzzle.Cell.x == _puzzles[i].Cell.x && centerPuzzle.Cell.y - 1 == _puzzles[i].Cell.y)
            {
                result.Add(_puzzles[i]);
            }
        }

        return result;
    }

    public void ResetOutlinePuzzles()
    {
        for (int i = 0; i < _puzzles.Length; i++)
        {
            _puzzles[i].SetOutline(false);
        }
    }

    public bool CheckPath() => _pathFinder.Find(_startPoint, _finishPoint, _puzzles).Count > 0;
    public CubeConfig Config => _cubeConfig;
}

