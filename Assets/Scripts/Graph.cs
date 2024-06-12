using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    private List<LineRenderer> _lineX;
    private List<LineRenderer> _lineY;
    private Stack<LineRenderer> _linePool = new Stack<LineRenderer>();
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

    private void Start()
    {
        CreateLinesX();
        CreateLinesY();
        _lastPosition = Camera.main.transform.position;
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

        if (Camera.main.transform.position.z < _lastPosition.z)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(Camera.main, transform);
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(Camera.main, transform);
            OnCameraZoomOut(max, min);
        }
        else if (Camera.main.transform.position.z > _lastPosition.z)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(Camera.main, transform);
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(Camera.main, transform);
            OnCameraZoomIn(max, min);
        }

        _lastPosition = Camera.main.transform.position;
    }

    private void OnDrawGizmos()
    {
        Vector3 fieldOfViewSize = CameraHelper.GetFieldOfViewSizeFromCameraTo(Camera.main, transform, out Vector3 fieldOfView);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fieldOfView, fieldOfViewSize);
    }

    private void OnCameraZoomOut(Vector3 max, Vector3 min)
    {
        LineRenderer largestLineX = GetLargestLineX(_lineX);
        if (largestLineX.GetPosition(1).x < max.x)
        {
            LineRenderer newLine = GetLineFromPool(_linePrefab, transform);
            _lineX.Add(newLine);
            Vector3[] positions = GetLineTo(largestLineX, Vector3.right);
            SetLineToPosition(newLine, positions);
            AutoSetLineColorX(newLine);
        }

        LineRenderer smallestLineX = GetSmallestLineX(_lineX);
        if (smallestLineX.GetPosition(1).x < min.x)
        {
            LineRenderer newLine = GetLineFromPool(_linePrefab, transform);
            _lineX.Add(newLine);
            Vector3[] positions = GetLineTo(smallestLineX, Vector3.left);
            SetLineToPosition(newLine, positions);
            AutoSetLineColorX(newLine);
        }

        // LineRenderer largestLineY = GetLargestLineY(_lineY);
        // if (largestLineY.GetPosition(1).y < max.y)
        // {
        //     LineRenderer newLine = GetLineFromPool(_linePrefab, transform);
        //     _lineY.Add(newLine);
        //     Vector3[] positions = GetLineTo(largestLineY, Vector3.up);
        //     SetLineToPosition(newLine, positions);
        //     AutoSetLineColorX(newLine);
        // }

        // LineRenderer smallestLineY = GetSmallestLineY(_lineY);
        // if (smallestLineY.GetPosition(1).y < min.y)
        // {
        //     LineRenderer newLine = GetLineFromPool(_linePrefab, transform);
        //     _lineY.Add(newLine);
        //     Vector3[] positions = GetLineTo(smallestLineY, Vector3.down);
        //     SetLineToPosition(newLine, positions);
        //     AutoSetLineColorX(newLine);
        // }
    }

    private void OnCameraZoomIn(Vector3 max, Vector3 min)
    {
        LineRenderer largestLineX = GetLargestLineX(_lineX);
        if (largestLineX.GetPosition(1).x > max.x)
        {
            _lineX.Remove(largestLineX);
            largestLineX.gameObject.SetActive(false);
            _linePool.Push(largestLineX);
        }

        LineRenderer smallestLineX = GetSmallestLineX(_lineX);
        if (smallestLineX.GetPosition(1).x > min.x)
        {
            _lineX.Remove(smallestLineX);
            smallestLineX.gameObject.SetActive(false);
            _linePool.Push(smallestLineX);
        }

        LineRenderer largestLineY = GetLargestLineY(_lineY);
        if (largestLineY.GetPosition(1).y > max.y)
        {
            _lineY.Remove(largestLineY);
            largestLineY.gameObject.SetActive(false);
            _linePool.Push(largestLineY);
        }

        LineRenderer smallestLineY = GetSmallestLineY(_lineY);
        if (smallestLineY.GetPosition(1).y > min.y)
        {
            _lineY.Remove(smallestLineY);
            smallestLineY.gameObject.SetActive(false);
            _linePool.Push(smallestLineY);
        }
    }

    private LineRenderer GetLineFromPool(LineRenderer prefab, Transform parent)
    {
        LineRenderer newLine = null;
        if (_linePool.Count == 0)
        {
            newLine = Instantiate(prefab, parent);
        }
        else
        {
            newLine = _linePool.Pop();
        }
        return newLine;
    }

    private void OnCameraMoveRight(Vector3 min)
    {
        for (int i = 0; i < _lineX.Count; i++)
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
        for (int i = 0; i < _lineY.Count; i++)
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
        for (int i = _lineX.Count - 1; i >= 0; i--)
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
        for (int i = _lineY.Count - 1; i >= 0; i--)
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

    private LineRenderer GetLargestLineX(List<LineRenderer> lines)
    {
        float x = float.NegativeInfinity;
        int index = 0;
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (x < lines[i].GetPosition(1).x)
            {
                x = lines[i].GetPosition(1).x;
                index = i;
            }
        }
        return lines[index];
    }

    private LineRenderer GetLargestLineY(List<LineRenderer> lines)
    {
        float y = float.NegativeInfinity;
        int index = 0;
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (y < lines[i].GetPosition(1).y)
            {
                y = lines[i].GetPosition(1).y;
                index = i;
            }
        }
        return lines[index];
    }

    private LineRenderer GetSmallestLineX(List<LineRenderer> lines)
    {
        float x = float.PositiveInfinity;
        int index = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            if (x > lines[i].GetPosition(1).x)
            {
                x = lines[i].GetPosition(1).x;
                index = i;
            }
        }
        return lines[index];
    }

    private LineRenderer GetSmallestLineY(List<LineRenderer> lines)
    {
        float y = float.PositiveInfinity;
        int index = 0;
        for (int i = 0; i < lines.Count; i++)
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
        _lineY = new List<LineRenderer>();
        int y = -_line.y;
        for (int i = 0; i < _line.y * 2; i++)
        {
            _lineY.Add(Instantiate(_linePrefab, transform));
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
            SetLineColors(line, _xColor, _xColor);
        }
    }
    
    private void CreateLinesX()
    {
        _lineX = new List<LineRenderer>();
        int x = -_line.x;
        for (int i = 0; i < _line.x * 2; i++)
        {
            _lineX.Add(Instantiate(_linePrefab, transform));
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
}
