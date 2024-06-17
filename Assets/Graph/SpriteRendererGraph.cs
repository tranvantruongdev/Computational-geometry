using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererGraph : MonoBehaviour
{
    private readonly List<SpriteRenderer> _lineX = new();
    private readonly List<SpriteRenderer> _lineY = new();
    private readonly Stack<SpriteRenderer> _linePoolX = new();
    private readonly Stack<SpriteRenderer> _linePoolY = new();
    
    private Camera _mainCamera;
    private Vector3 _lastPosition;
    private float _size;

    [Header("__SETTINGS__")]
    public Vector2Int line = new Vector2Int(7, 7);
    public Vector2 space;
    public float lineThickness = 1f;
    [Space]
    [SerializeField]
    private SpriteRenderer _spritePrefab;
    [Space]
    [Header("__COLOR__")]
    [SerializeField]
    private Color _yColor;
    [SerializeField]
    private Color _xColor;
    [SerializeField]
    private Color _lineColor;

    #region UNITY
    private void Start()
    {
        _mainCamera = Camera.main;
        _lastPosition = _mainCamera.transform.position;
        CreateLines(line);
    }

    private void Update()
    {
        if (_mainCamera.transform.position.x > _lastPosition.x)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(_mainCamera, transform);
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(_mainCamera, transform);
            OnCameraMoveRight(max, min);
        }
        else if (_mainCamera.transform.position.x < _lastPosition.x)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(_mainCamera, transform);
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(_mainCamera, transform);
            OnCameraMoveLeft(max, min);
        }

        if (_mainCamera.transform.position.y > _lastPosition.y)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(_mainCamera, transform);
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(_mainCamera, transform);
            OnCameraMoveUp(max, min);
        }
        else if (_mainCamera.transform.position.y < _lastPosition.y)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(_mainCamera, transform);
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(_mainCamera, transform);
            OnCameraMoveDown(max, min);
        }

        bool isPerspectiveZoomOut = !_mainCamera.orthographic && _mainCamera.transform.position.z < _lastPosition.z;
        bool isPerspectiveZoomIn = !_mainCamera.orthographic && _mainCamera.transform.position.z > _lastPosition.z;

        bool isOrthographicZoomOut = _mainCamera.orthographic && _mainCamera.orthographicSize > _size;
        bool isOrthographicZoomIn = _mainCamera.orthographic && _mainCamera.orthographicSize < _size;

        if (isPerspectiveZoomOut || isOrthographicZoomOut)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(_mainCamera, transform);
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(_mainCamera, transform);
            OnCameraZoomOut(max, min);
        }
        else if (isPerspectiveZoomIn || isOrthographicZoomIn)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(_mainCamera, transform);
            Vector3 min = CameraHelper.GetMinViewFromCameraTo(_mainCamera, transform);
            OnCameraZoomIn(max, min);
        }

        _lastPosition = _mainCamera.transform.position;
        _size = _mainCamera.orthographicSize;
    }

    private void OnDrawGizmos()
    {
        Vector3 fieldOfViewSize = CameraHelper.GetFieldOfViewSizeFromCameraTo(Camera.main, transform, out Vector3 fieldOfView);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fieldOfView, fieldOfViewSize);
    }
    #endregion

    private void CreateLines(Vector2 expand)
    {
        for (int x = (int)(-expand.x / 2); x < expand.x / 2; x++)
        {
            SpriteRenderer sprite = GetLineX();
            sprite.transform.position = new Vector3(x * space.x, 0);
            AutoSetLineColorX(sprite);
            _lineX.Add(sprite);
        }

        for (int y = (int)(-expand.y / 2); y < expand.y / 2; y++)
        {
            SpriteRenderer sprite = GetLineFromPool(_linePoolY);
            sprite.transform.position = new Vector3(0, y * space.y);
            AutoSetLineColorY(sprite);
            _lineY.Add(sprite);
        }
    }

    private SpriteRenderer GetLineFromPool(Stack<SpriteRenderer> pool)
    {
        if (pool.Count > 0)
        {
            SpriteRenderer sprite = pool.Pop();
            sprite.gameObject.SetActive(true);
            return sprite;
        }
        SpriteRenderer newSprite = Instantiate(_spritePrefab, transform);
        newSprite.transform.localScale = new Vector3(newSprite.transform.localScale.x, lineThickness, newSprite.transform.localScale.z);
        return newSprite;
    }

    private void OnCameraMoveRight(Vector3 max, Vector3 min)
    {
        SpriteRenderer largestLine = GetLargestLineX(_lineX);
        if (largestLine.transform.position.x <= max.x)
        {
            SpriteRenderer newLine = GetLineX();
            newLine.transform.position = largestLine.transform.position + Vector3.right * space.x;
            AutoSetLineColorX(newLine);
            _lineX.Add(newLine);
        }

        SpriteRenderer smallestLine = GetSmallestLineX(_lineX);
        if (smallestLine.transform.position.x <= min.x)
        {
            PushLineXToPool(smallestLine);
        }
    }

    private void OnCameraMoveLeft(Vector3 max, Vector3 min)
    {
        SpriteRenderer smallestLine = GetSmallestLineX(_lineX);
        if (smallestLine.transform.position.x >= min.x)
        {
            SpriteRenderer newLine = GetLineX();
            newLine.transform.position = smallestLine.transform.position + Vector3.left * space.x;
            AutoSetLineColorX(newLine);
            _lineX.Add(newLine);
        }

        SpriteRenderer largestLine = GetLargestLineX(_lineX);
        if (largestLine.transform.position.x >= max.x)
        {
            PushLineXToPool(largestLine);
        }
    }

    private void OnCameraMoveUp(Vector3 max, Vector3 min)
    {
        SpriteRenderer largestLine = GetLargestLineY(_lineY);
        if (largestLine.transform.position.y <= max.y)
        {
            SpriteRenderer newLine = GetLineFromPool(_linePoolY);
            newLine.transform.position = largestLine.transform.position + Vector3.up * space.y;
            AutoSetLineColorY(newLine);
            _lineY.Add(newLine);
        }

        SpriteRenderer smallestLine = GetSmallestLineY(_lineY);
        if (smallestLine.transform.position.y <= min.y)
        {
            PushLineYToPool(smallestLine);
        }
    }

    private void OnCameraMoveDown(Vector3 max, Vector3 min)
    {
        SpriteRenderer smallestLine = GetSmallestLineY(_lineY);
        if (smallestLine.transform.position.y >= min.y)
        {
            SpriteRenderer newLine = GetLineFromPool(_linePoolY);
            newLine.transform.position = smallestLine.transform.position + Vector3.down * space.y;
            AutoSetLineColorY(newLine);
            _lineY.Add(newLine);
        }

        SpriteRenderer largestLine = GetLargestLineY(_lineY);
        if (largestLine.transform.position.y >= max.y)
        {
            PushLineYToPool(largestLine);
        }
    }

    private void OnCameraZoomOut(Vector3 max, Vector3 min)
    {
        SpriteRenderer largestLineX = GetLargestLineX(_lineX);
        if (largestLineX.transform.position.x <= max.x)
        {
            SpriteRenderer newLine = GetLineX();
            newLine.gameObject.SetActive(true);
            newLine.transform.position = largestLineX.transform.position + Vector3.right * space.x;
            AutoSetLineColorX(newLine);
            _lineX.Add(newLine);
        }

        SpriteRenderer smallestLineX = GetSmallestLineX(_lineX);
        if (smallestLineX.transform.position.x >= min.x)
        {
            SpriteRenderer newLine = GetLineX();
            newLine.gameObject.SetActive(true);
            newLine.transform.position = smallestLineX.transform.position + Vector3.left * space.x;
            AutoSetLineColorX(newLine);
            _lineX.Add(newLine);
        }

        SpriteRenderer largestLineY = GetLargestLineY(_lineY);
        if (largestLineY.transform.position.y <= max.y)
        {
            SpriteRenderer newLine = GetLineFromPool(_linePoolY);
            newLine.gameObject.SetActive(true);
            newLine.transform.position = largestLineY.transform.position + Vector3.up * space.y;
            AutoSetLineColorY(newLine);
            _lineY.Add(newLine);
        }

        SpriteRenderer smallestLineY = GetSmallestLineY(_lineY);
        if (smallestLineY.transform.position.y >= min.y)
        {
            SpriteRenderer newLine = GetLineFromPool(_linePoolY);
            newLine.gameObject.SetActive(true);
            newLine.transform.position = smallestLineY.transform.position + Vector3.down * space.y;
            AutoSetLineColorY(newLine);
            _lineY.Add(newLine);
        }
    }

    private void OnCameraZoomIn(Vector3 max, Vector3 min)
    {
        SpriteRenderer largestLineX = GetLargestLineX(_lineX);
        if (largestLineX.transform.position.x >= max.x)
        {
            PushLineXToPool(largestLineX);
        }

        SpriteRenderer smallestLineX = GetSmallestLineX(_lineX);
        if (smallestLineX.transform.position.x <= min.x)
        {
            PushLineXToPool(smallestLineX);
        }

        SpriteRenderer largestLineY = GetLargestLineY(_lineY);
        if (largestLineY.transform.position.y >= max.y)
        {
            PushLineYToPool(largestLineY);
        }

        SpriteRenderer smallestLineY = GetSmallestLineY(_lineY);
        if (smallestLineY.transform.position.y <= min.y)
        {
            PushLineYToPool(smallestLineY);
        }
    }


    private SpriteRenderer GetLineX()
    {
        SpriteRenderer newLine = GetLineFromPool(_linePoolX);
        newLine.transform.up = Vector3.right;
        return newLine;
    }

    private void PushLineXToPool(SpriteRenderer spriteRenderer)
    {
        PushLineToPool(spriteRenderer, _lineX, _linePoolX);
    }

    private void PushLineYToPool(SpriteRenderer spriteRenderer)
    {
        PushLineToPool(spriteRenderer, _lineY, _linePoolY);
    }

    private void PushLineToPool(SpriteRenderer spriteRenderer, List<SpriteRenderer> lineList, Stack<SpriteRenderer> pool)
    {
        lineList.Remove(spriteRenderer);
        spriteRenderer.gameObject.SetActive(false);
        pool.Push(spriteRenderer);
    }

    private void AutoSetLineColorX(SpriteRenderer line)
    {
        if (line.transform.position.x != 0)
        {
            line.color = _lineColor;
            return;
        }
        line.color = _yColor;
    }

    private void AutoSetLineColorY(SpriteRenderer line)
    {
        if (line.transform.position.y != 0)
        {
            line.color = _lineColor;
            return;
        }
        line.color = _xColor;
    }

    private SpriteRenderer GetLargestLineX(List<SpriteRenderer> lines)
    {
        float x = float.NegativeInfinity;
        int index = 0;
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (x < lines[i].transform.position.x)
            {
                x = lines[i].transform.position.x;
                index = i;
            }
        }
        return lines[index];
    }

    private SpriteRenderer GetLargestLineY(List<SpriteRenderer> lines)
    {
        float y = float.NegativeInfinity;
        int index = 0;
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (y < lines[i].transform.position.y)
            {
                y = lines[i].transform.position.y;
                index = i;
            }
        }
        return lines[index];
    }

    private SpriteRenderer GetSmallestLineX(List<SpriteRenderer> lines)
    {
        float x = float.PositiveInfinity;
        int index = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            if (x > lines[i].transform.position.x)
            {
                x = lines[i].transform.position.x;
                index = i;
            }
        }
        return lines[index];
    }

    private SpriteRenderer GetSmallestLineY(List<SpriteRenderer> lines)
    {
        float y = float.PositiveInfinity;
        int index = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            if (y > lines[i].transform.position.y)
            {
                y = lines[i].transform.position.y;
                index = i;
            }
        }
        return lines[index];
    }
}
