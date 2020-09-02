using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Puzzle : MonoBehaviour
{

    public static Puzzle ActivePuzzle;
    public static bool IsInteractable = true;

    [SerializeField] private bool _interactable = true;
    [Space]
    [SerializeField] private Direction[] _dirs;
    [Space]
    [SerializeField] private Transform[] _helpDecors;

    private AudioSource _audioSource;

    private Vector2Int _firstPost;

    private CubeConfig _config;

    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {  
        _firstPost = Cell;
    }

    public void Init(CubeConfig config)
    {
        _config = config;
        if (_audioSource != null)
        {
            _audioSource.clip = _config.Audio.ClickPlatform;
        }   
    }

    private void OnMouseUp()
    {
        if (InputManager.IsDrag || !IsInteractable || !_interactable)
        {
            return;
        }

        if(ActivePuzzle == this)
        {
            Active(false);
            ActivePuzzle = null;
        }
        else if (ActivePuzzle != null)
        {
            if (IsNeighbor(ActivePuzzle))
            {
                Swap(this);
                Debug.Log("IsNeighbor");
                _audioSource.Play();
            }
            else
            {
                ActivePuzzle.Active(false);

                ActivePuzzle = this;
                Active(true);
            }
        }
        else if(ActivePuzzle == null)
        {
            ActivePuzzle = this;
            Active(true);
            _audioSource.Play();
        }
    }

    public void Swap(Puzzle targetPuzzle)
    {
        IsInteractable = false;

        Vector3 targetPos = targetPuzzle.transform.localPosition;
        Vector3 activePos = new Vector3(ActivePuzzle.transform.localPosition.x, 0f, ActivePuzzle.transform.localPosition.z);

        ActivePuzzle.DOKill();

        ActivePuzzle.transform.DOLocalMove(new Vector3(targetPos.x, ActivePuzzle.transform.localPosition.y, targetPos.z), _config.UpSpeedPlatform).OnComplete(() => 
        {
            //ActivePuzzle.CheckDecorHelpers();
            ActivePuzzle.transform.DOLocalMove(targetPos, _config.SwapSpeedPlatform);

            targetPuzzle.DOKill();
            //targetPuzzle.CheckDecorHelpers();
            targetPuzzle.transform.DOLocalMove(activePos, _config.SwapSpeedPlatform).OnComplete(OnSwap);
        });
    }

    private void OnSwap()
    {
        CheckDecorHelpers();

        ActivePuzzle.Active(false);
        ActivePuzzle.CheckDecorHelpers();
        ActivePuzzle = null;

        if (ToolBox.CubeManager.CheckPath())
        {
            ToolBox.CubeManager.OnCorrectPath();
        }
        else
        {
            IsInteractable = true;
        }
    }

    public void CheckDecorHelpers()
    {
        float val = default;

        if (IsActive)
        {
            val = 0;
        }
        else if(Cell.x == _firstPost.x && Cell.y == _firstPost.y)
        {
            val = 0.5f;
        }
        else
        {
            val = 0;
        }

        for (int i = 0; i < _helpDecors.Length; i++)
        {
            _helpDecors[i].DOKill();

            _helpDecors[i].DOScale(val, 0.5f);
        }
    }

    public bool IsNeighbor(Puzzle puzzle)
    {
        if (transform.localPosition.x == puzzle.transform.localPosition.x && transform.localPosition.z + 1 == puzzle.transform.localPosition.z)
        {
            return true;
        }
        else if (transform.localPosition.x + 1 == puzzle.transform.localPosition.x && transform.localPosition.z == puzzle.transform.localPosition.z)
        {
            return true;
        }
        else if (transform.localPosition.x - 1 == puzzle.transform.localPosition.x && transform.localPosition.z == puzzle.transform.localPosition.z)
        {
            return true;
        }
        else if (transform.localPosition.x == puzzle.transform.localPosition.x && transform.localPosition.z - 1 == puzzle.transform.localPosition.z)
        {
            return true;
        }
        else return false;
    }

    public void Active(bool status)
    {
        transform.DOKill();

        IsActive = status;

        if (IsActive)
        {
            transform.DOLocalMoveY(0.2f, _config.UpSpeedPlatform);
        }
        else
        {
            transform.DOLocalMoveY(0f, _config.UpSpeedPlatform);
        }
    }

    public bool IsActive { get; private set; }
    public Direction[] Directions => _dirs;
    public Vector2Int Cell => new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.z);

    public enum Direction
    {
        Up,
        Left,
        Right,
        Down
    }
}
