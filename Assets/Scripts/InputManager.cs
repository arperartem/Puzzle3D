using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static bool IsDrag { get; private set; }

    private bool _isPressed = false;

    private Vector3 _pressedPosition;
    private Vector3 _currentPosition;

    private Transform _cube;
    private float yAxis;

    private Camera _mainCamera;
    private float xAxis;
    private Transform _pivotCamera;
    private CubeConfig _config;

    private bool _isZooming = false;
    private bool _isHorizontalRotating = false;
    private bool _isVerticalRotating = false;

    private void Awake()
    {
        _cube = transform;
        _mainCamera = Camera.main;
        _pivotCamera = _mainCamera.transform.parent;
        _config = ToolBox.CubeManager.Config;
    }

    private void Update()
    {
        ZoomCube();

        if (!_isZooming)
        {
            RotateCube();
        }   
    }

    private void ZoomCube()
    {
        if(Input.touchCount == 2)
        {
            _isZooming = true;

            Touch tZero = Input.GetTouch(0);
            Touch tOne = Input.GetTouch(1);

            Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
            Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

            float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
            float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

            float deltaDistance = oldTouchDistance - currentTouchDistance;
            Zoom(deltaDistance, _config.Zoom.TouchZoomSpeed);

            /*else
            {

                float scroll = Input.GetAxis("Mouse ScrollWheel");
                Zoom(scroll, MouseZoomSpeed);
            }*/

            if (_mainCamera.fieldOfView < _config.Zoom.ZoomMinBound)
            {
                _mainCamera.fieldOfView = 0.1f;
            }
            else
            if (_mainCamera.fieldOfView > _config.Zoom.ZoomMaxBound)
            {
                _mainCamera.fieldOfView = 179.9f;
            }
        } 
        else
        {
            _isZooming = false;
        }
    }

    private void Zoom(float deltaMagnitudeDiff, float speed)
    {
        _mainCamera.fieldOfView += deltaMagnitudeDiff * speed;
        _mainCamera.fieldOfView = Mathf.Clamp(_mainCamera.fieldOfView, _config.Zoom.ZoomMinBound, _config.Zoom.ZoomMaxBound);
    }

    private void RotateCube()
    {
        if (!_isPressed && (Input.GetMouseButtonDown(0) || Input.touchCount == 1))
        {
            _isPressed = true;

            xAxis = _pivotCamera.localEulerAngles.x;
            yAxis = _cube.localEulerAngles.y;

            _pressedPosition = Input.mousePosition;
        }

        if (_isPressed)
        {
            _currentPosition = Input.mousePosition;


            if (!_isVerticalRotating && Mathf.Abs(_pressedPosition.x - _currentPosition.x) >= _config.Rotate.MinDistance)
            {
                IsDrag = true;
                _isHorizontalRotating = true;
                int dir = _pressedPosition.x < _currentPosition.x ? -1 : 1;
                float diff = Mathf.Abs(_pressedPosition.x - _currentPosition.x);
                float targetY = diff * dir * _config.Rotate.SpeedRotate;
                _cube.localEulerAngles = new Vector3(_cube.localEulerAngles.x, yAxis + targetY, _cube.localEulerAngles.z);
            }

            if (!_isHorizontalRotating && Mathf.Abs(_pressedPosition.y - _currentPosition.y) >= _config.Rotate.MinDistance)
            {
                IsDrag = true;
                _isVerticalRotating = true;
                int dir = _pressedPosition.y < _currentPosition.y ? -1 : 1;
                float diff = Mathf.Abs(_pressedPosition.y - _currentPosition.y);
                float targetX = diff * dir * _config.Rotate.SpeedRotate;
                float temp = Mathf.Min(35f, xAxis + targetX);
                _pivotCamera.localEulerAngles = new Vector3(Mathf.Max(temp, 0f), _pivotCamera.localEulerAngles.y, _pivotCamera.localEulerAngles.z);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isVerticalRotating = false;
            _isHorizontalRotating = false;

            _isPressed = false;
            IsDrag = false;
        }
    }
}
