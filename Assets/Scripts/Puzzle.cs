using NaughtyAttributes;
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
    [Header("!!! Y = Z !!!")]
    [BoxGroup("Help Decors")] [SerializeField] private Vector2Int[] _targetPositions;
    [BoxGroup("Help Decors")] [SerializeField] private Transform[] _helpDecors;

    private AudioSource _audioSource;
    private CubeConfig _config;

    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        CheckDecorHelpers(false);
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

        ToolBox.CubeManager.ResetOutlinePuzzles();

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

        ToolBox.CubeManager.OnSwapPuzzle();
    }

    public void CheckDecorHelpers(bool anim = true)
    {
        float val = default;

        if (IsActive)
        {
            val = 0;
        }
        else if(IsOnTargetPosition()/*Cell.x == _firstPost.x && Cell.y == _firstPost.y*/)
        {
            val = 0.5f;
        }
        else
        {
            val = 0;
        }

        for (int i = 0; i < _helpDecors.Length; i++)
        {
            if(_helpDecors[i] != null)
            {
                _helpDecors[i].DOKill();
                if (anim)
                {
                    _helpDecors[i].DOScale(val, 0.5f);
                }
                else
                {
                    _helpDecors[i].localScale = new Vector3(val, val, val);
                }
            } 
        }

        bool IsOnTargetPosition()
        {
            for (int i = 0; i < _targetPositions.Length; i++)
            {
                if (Cell.x == _targetPositions[i].x && Cell.y == _targetPositions[i].y)
                {
                    return true;
                }
            }
            return false;
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
            SetOutline(true);
            SetOutlineNeighbors(true);
        }
        else
        {
            transform.DOLocalMoveY(0f, _config.UpSpeedPlatform);
            SetOutline(false);
            SetOutlineNeighbors(false);
        }
    }

    public void SetOutline(bool enable)
    {
        Material[] temp;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (enable)
        {
            temp = new Material[2];
            temp[0] = mr.materials[0];
            temp[1] = Resources.Load<Material>("Outline");      
        }
        else
        {
            temp = new Material[1];
            temp[0] = mr.materials[0];   
        }

        mr.materials = temp;
    }

    private void SetOutlineNeighbors(bool enable)
    {
        List<Puzzle> neighbors = ToolBox.CubeManager.GetNeighbors(this);

        for (int i = 0; i < neighbors.Count; i++)
        {
            neighbors[i].SetOutline(enable);
        }
    }

    public bool IsActive { get; private set; }
    public bool Interactable => _interactable;
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
