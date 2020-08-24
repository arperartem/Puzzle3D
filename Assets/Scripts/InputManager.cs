using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static bool IsDrag { get; private set; }

    [SerializeField] private float _speedRotate;
    [SerializeField] private float _minDistance = 5f;

    private bool _isPressed = false;

    private Vector3 _pressedPosition;
    private Vector3 _currentPosition;

    private Transform _cube;
    private float yAxis;

    private void Awake()
    {
        _cube = transform;
    }

    private void Update()
    {
        if (!_isPressed && Input.GetMouseButtonDown(0))
        {
            _isPressed = true;

            yAxis = _cube.localEulerAngles.y;

            _pressedPosition = Input.mousePosition;
        }

        if (_isPressed)
        {
            _currentPosition = Input.mousePosition;

            if(Vector3.Distance(_pressedPosition, _currentPosition) >= _minDistance || IsDrag)
            {
                IsDrag = true;
                int dir = _pressedPosition.x < _currentPosition.x ? -1 : 1;
                float diff = Mathf.Abs(_pressedPosition.x - _currentPosition.x);
                float targetY = diff * dir * _speedRotate;

                //_cube.localEulerAngles = Vector3.Lerp(_cube.localEulerAngles, new Vector3(_cube.localEulerAngles.x, yAxis + y, _cube.localEulerAngles.z), Time.deltaTime );
                
                _cube.localEulerAngles = new Vector3(_cube.localEulerAngles.x, yAxis + targetY, _cube.localEulerAngles.z);
            }
            /*else
            {
                IsDrag = false;
            }*/
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isPressed = false;
            IsDrag = false;
        }  
    }
}
