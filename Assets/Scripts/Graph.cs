using System.Collections.Generic;

using UnityEngine;

public class Graph : MonoBehaviour
{
    private readonly List<LineRenderer> _lineX = new();
    private readonly List<LineRenderer> _lineY = new();
    private readonly Stack<LineRenderer> _linePool = new();
    private Vector3 _lastPosition;

    [SerializeField]
    private LineRenderer _linePrefab;
    [Space]
    [Header("__SETTINGS__")]
    [SerializeField]
    private Vector2Int _line = new(7, 7);
    [SerializeField]
    private Vector2 _max = new(100f, 100f);
    [SerializeField]
    private Vector2 _min = new(-100f, -100f);
    [Space]
    [Header("__COLOR__")]
    [SerializeField]
    private Color _yColor;
    [SerializeField]
    private Color _xColor;
    [SerializeField]
    private Color _lineColor;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please tag your main camera as 'MainCamera'.");
            return;
        }

        CreateLines(_lineX, SetLineX, AutoSetLineColorX);
        CreateLines(_lineY, SetLineY, AutoSetLineColorY);
        _lastPosition = _mainCamera.transform.position;
    }

    private void Update()
    {
        if (_mainCamera == null)
        {
            return;
        }

        Vector3 cameraPosition = _mainCamera.transform.position;

        if (cameraPosition != _lastPosition)
        {
            Vector3 maxView = CameraHelper.GetMaxViewFromCameraTo(_mainCamera, transform);
            Vector3 minView = CameraHelper.GetMinViewFromCameraTo(_mainCamera, transform);

            if (cameraPosition.x != _lastPosition.x)
            {
                if (cameraPosition.x > _lastPosition.x)
                {
                    OnCameraMoveRight(maxView);
                }
                else
                {
                    OnCameraMoveLeft(minView);
                }
            }

            if (cameraPosition.y != _lastPosition.y)
            {
                if (cameraPosition.y > _lastPosition.y)
                {
                    OnCameraMoveUp(maxView);
                }
                else
                {
                    OnCameraMoveDown(minView);
                }
            }

            if (cameraPosition.z != _lastPosition.z)
            {
                if (cameraPosition.z < _lastPosition.z)
                {
                    OnCameraMoveForward(maxView, minView);
                }
                else
                {
                    OnCameraMoveBack(minView, maxView);
                }
            }

            _lastPosition = cameraPosition;
        }
    }

    private void CreateLines(List<LineRenderer> lines, System.Action<LineRenderer, int> setLinePosition, System.Action<LineRenderer> setLineColor)
    {
        int count = _line.x * 2;
        int startIndex = -_line.x;

        for (int i = 0; i < count; i++)
        {
            LineRenderer line = Instantiate(_linePrefab, transform);
            setLinePosition(line, startIndex + i);
            setLineColor(line);
            lines.Add(line);
        }
    }

    private void AutoSetLineColorY(LineRenderer line)
    {
        if (line.GetPosition(0).y != 0)
        {
            SetLineColors(line, _lineColor, _lineColor);
        }
        else
        {
            SetLineColors(line, _xColor, _xColor);
        }
    }

    private void AutoSetLineColorX(LineRenderer line)
    {
        if (line.GetPosition(0).x != 0)
        {
            SetLineColors(line, _lineColor, _lineColor);
        }
        else
        {
            SetLineColors(line, _yColor, _yColor);
        }
    }

    private void SetLineY(LineRenderer line, int index)
    {
        line.SetPosition(0, new Vector3(_min.x, index));
        line.SetPosition(1, new Vector3(_max.x, index));
    }

    private void SetLineX(LineRenderer line, int index)
    {
        line.SetPosition(0, new Vector3(index, _min.y));
        line.SetPosition(1, new Vector3(index, _max.y));
    }

    private void SetLineColors(LineRenderer lineRenderer, Color c1, Color c2)
    {
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c2;
    }

    private void OnCameraMoveRight(Vector3 max)
    {
        foreach (LineRenderer line in _lineY)
        {
            if (line.GetPosition(1).x < max.x)
            {
                float newX = line.GetPosition(1).x + (_line.x * 2);
                SetLineY(line, Mathf.RoundToInt(newX));
            }
        }
    }

    private void OnCameraMoveLeft(Vector3 min)
    {
        foreach (LineRenderer line in _lineY)
        {
            if (line.GetPosition(0).x > min.x)
            {
                float newX = line.GetPosition(0).x - (_line.x * 2);
                SetLineY(line, Mathf.RoundToInt(newX));
            }
        }
    }

    private void OnCameraMoveUp(Vector3 max)
    {
        foreach (LineRenderer line in _lineX)
        {
            if (line.GetPosition(1).y < max.y)
            {
                float newY = line.GetPosition(1).y + (_line.y * 2);
                SetLineX(line, Mathf.RoundToInt(newY));
            }
        }
    }

    private void OnCameraMoveDown(Vector3 min)
    {
        foreach (LineRenderer line in _lineX)
        {
            if (line.GetPosition(0).y > min.y)
            {
                float newY = line.GetPosition(0).y - (_line.y * 2);
                SetLineX(line, Mathf.RoundToInt(newY));
            }
        }
    }

    private void OnCameraMoveForward(Vector3 max, Vector3 min)
    {
        OnCameraMoveRight(max);
        OnCameraMoveLeft(min);
    }

    private void OnCameraMoveBack(Vector3 min, Vector3 max)
    {
        OnCameraMoveRight(max);
        OnCameraMoveLeft(min);
    }
}
