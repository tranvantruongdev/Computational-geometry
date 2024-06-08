using UnityEngine;

public class Graph : MonoBehaviour
{
    private LineRenderer[] _lineX;
    private LineRenderer[] _lineY;
    private Vector3 _lastPosition;

    [SerializeField]
    private LineRenderer _linePrefab;
    [Space]
    [Header("__SETTINGS__")]
    [SerializeField]
    private Vector2Int _line = new Vector2Int(7, 7);
    [SerializeField]
    private Vector2 _max = new Vector2(100f, 100f);
    [SerializeField]
    private Vector2 _min = new Vector2(-100f, -100f);
    [Space]
    [Header("__COLOR__")]
    [SerializeField]
    private Color _yColor;
    [SerializeField]
    private Color _xColor;
    [SerializeField]
    private Color _lineColor;

    private GameObject g;

    private void Start()
    {
        CreateLinesX();
        CreateLinesY();
        _lastPosition = Camera.main.transform.position;
        g = new GameObject("l");
    }

    private void Update()
    {

        if (Camera.main.transform.position.x > _lastPosition.x)
        {
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(Camera.main, transform);
            OnCameraMoveRight(min);
        }
        else if (Camera.main.transform.position.x < _lastPosition.x)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(Camera.main, transform);
            OnCameraMoveLeft(max);
        }

        if (Camera.main.transform.position.y > _lastPosition.y)
        {
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(Camera.main, transform);
            OnCameraMoveUp(min);
        }
        else if (Camera.main.transform.position.y < _lastPosition.y)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(Camera.main, transform);
            OnCameraMoveDown(max);
        }

        _lastPosition = Camera.main.transform.position;
    }

    private void OnDrawGizmos()
    {
        Vector3 fieldOfViewSize = CameraHelper.GetFieldOfViewSizeFromCameraTo(Camera.main, transform, out Vector3 fieldOfView);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fieldOfView, fieldOfViewSize);
    }

    private void OnCameraMoveRight(Vector3 min)
    {
        for (int i = 0; i < _lineX.Length; i++)
        {
            if (_lineX[i].GetPosition(1).x < min.x)
            {
                LineRenderer largestLine = GetLargestLineX(_lineX);

                Vector3[] positions = GetLineTo(largestLine, Vector3.right);
                SetLineToPosition(_lineX[i], positions);
                AutoSetLineColorX(_lineX[i]);
            }
        }
    }

    private void OnCameraMoveUp(Vector3 min)
    {
        for (int i = 0; i < _lineY.Length; i++)
        {
            if (_lineY[i].GetPosition(1).y < min.y)
            {
                LineRenderer largestLine = GetLargestLineY(_lineY);

                Vector3[] positions = GetLineTo(largestLine, Vector3.up);
                SetLineToPosition(_lineY[i], positions);
                AutoSetLineColorY(_lineY[i]);
            }
        }
    }

    private void OnCameraMoveLeft(Vector3 max)
    {
        for (int i = _lineX.Length - 1; i >= 0; i--)
        {
            if (_lineX[i].GetPosition(1).x > max.x)
            {
                LineRenderer smallestLine = GetSmallestLineX(_lineX);

                Vector3[] positions = GetLineTo(smallestLine, Vector3.left);
                SetLineToPosition(_lineX[i], positions);
                AutoSetLineColorX(_lineX[i]);
            }
        }
    }

    private void OnCameraMoveDown(Vector3 max)
    {
        for (int i = _lineY.Length - 1; i >= 0; i--)
        {
            if (_lineY[i].GetPosition(1).y > max.y)
            {
                LineRenderer smallestLine = GetSmallestLineY(_lineY);

                Vector3[] positions = GetLineTo(smallestLine, Vector3.down);
                SetLineToPosition(_lineY[i], positions);
                AutoSetLineColorY(_lineY[i]);
            }
        }
    }

    private Vector3[] GetLineTo(LineRenderer line, Vector3 add)
    {
        Vector3[] positions = new Vector3[2];

        positions[0] = line.GetPosition(0);
        positions[0] += add;

        positions[1] = line.GetPosition(1);
        positions[1] += add;
        return positions;
    }

    private void SetLineToPosition(LineRenderer line, Vector3[] linePositions)
    {
        for (int i = 0; i < 2; i++)
        {
            line.SetPosition(i, linePositions[i]);
        }
    }

    private LineRenderer GetLargestLineX(LineRenderer[] lines)
    {
        float x = float.NegativeInfinity;
        int index = 0;
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            if (x < lines[i].GetPosition(1).x)
            {
                x = lines[i].GetPosition(1).x;
                index = i;
            }
        }
        return lines[index];
    }

    private LineRenderer GetLargestLineY(LineRenderer[] lines)
    {
        float y = float.NegativeInfinity;
        int index = 0;
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            if (y < lines[i].GetPosition(1).y)
            {
                y = lines[i].GetPosition(1).y;
                index = i;
            }
        }
        return lines[index];
    }

    private LineRenderer GetSmallestLineX(LineRenderer[] lines)
    {
        float x = float.PositiveInfinity;
        int index = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (x > lines[i].GetPosition(1).x)
            {
                x = lines[i].GetPosition(1).x;
                index = i;
            }
        }
        return lines[index];
    }

    private LineRenderer GetSmallestLineY(LineRenderer[] lines)
    {
        float y = float.PositiveInfinity;
        int index = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (y > lines[i].GetPosition(1).y)
            {
                y = lines[i].GetPosition(1).y;
                index = i;
            }
        }
        return lines[index];
    }

    private void CreateLinesY()
    {
        _lineY = new LineRenderer[_line.y * 2];
        int y = -_line.y;
        for (int i = 0; i < _line.y * 2; i++)
        {
            _lineY[i] = Instantiate(_linePrefab, transform);
            SetLineY(_lineY[i], y);

            AutoSetLineColorY(_lineY[i]);
            y++;
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
            SetLineColors(line, _yColor, _yColor);
        }
    }
    
    private void CreateLinesX()
    {
        _lineX = new LineRenderer[_line.x * 2];
        int x = -_line.x;
        for (int i = 0; i < _line.x * 2; i++)
        {
            _lineX[i] = Instantiate(_linePrefab, transform);
            SetLineX(_lineX[i], x);
            AutoSetLineColorX(_lineX[i]);
            x++;
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
            SetLineColors(line, _xColor, _xColor);
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
}
