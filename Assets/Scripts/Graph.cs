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
            g.transform.position = min;
            OnCameraMoveToRight(min);
        }
        else if (Camera.main.transform.position.x < _lastPosition.x)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(Camera.main, transform);
            OnCameraMoveToLeft(max);
        }

        if (transform.position.y > _lastPosition.y)
        {

        }
        else if (transform.position.y < _lastPosition.y)
        {

        }

        _lastPosition = Camera.main.transform.position;
    }

    private void OnDrawGizmos()
    {
        Vector3 fieldOfViewSize = CameraHelper.GetFieldOfViewSizeFromCameraTo(Camera.main, transform, out Vector3 fieldOfView);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fieldOfView, fieldOfViewSize);
    }

    private void OnCameraMoveToRight(Vector3 min)
    {
        for (int i = 0; i < _lineX.Length; i++)
        {
            if (_lineX[i].GetPosition(1).x < min.x)
            {
                LineRenderer largestLine = GetLargestLineX(_lineX);
                Debug.Log($"largest: {largestLine.GetPosition(1).x}");
                SetLineColors(largestLine, Color.magenta, Color.cyan);

                Vector3[] positions = GetLineXTo(largestLine, 1);
                SetLineXToPosition(_lineX[i], positions);
            }
        }
    }

    private void OnCameraMoveToLeft(Vector3 max)
    {
        for (int i = _lineX.Length - 1; i >= 0; i--)
        {
            if (_lineX[i].GetPosition(1).x > max.x)
            {
                LineRenderer smallestLine = GetSmallestLineX(_lineX);
                SetLineColors(smallestLine, Color.red, Color.cyan);

                Vector3[] positions = GetLineXTo(smallestLine, -1);
                SetLineXToPosition(_lineX[i], positions);
            }
        }
    }

    private Vector3[] GetLineXTo(LineRenderer line, float value)
    {
        Vector3[] positions = new Vector3[2];

        positions[0] = line.GetPosition(0);
        positions[0].x += value;

        positions[1] = line.GetPosition(1);
        positions[1].x += value;
        return positions;
    }

    private void SetLineXToPosition(LineRenderer line, Vector3[] linePositions)
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

    private void CreateLinesY()
    {
        _lineY = new LineRenderer[_line.y * 2];
        int y = -_line.y;
        for (int i = 0; i < _line.y * 2; i++)
        {
            _lineY[i] = Instantiate(_linePrefab, transform);
            SetLineY(_lineY[i], y);

            if (y != 0)
            {
                SetLineColors(_lineY[i], _lineColor, _lineColor);
            }
            else
            {
                SetLineColors(_lineY[i], _yColor, _yColor);
            }
            y++;
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

            if (x != 0)
            {
                SetLineColors(_lineX[i], _lineColor, _lineColor);
            }
            else
            {
                SetLineColors(_lineX[i], _xColor, _xColor);
            }
            x++;
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
