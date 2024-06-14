using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererGraph : MonoBehaviour
{
    private readonly List<SpriteRenderer> _lineX = new();
    private readonly List<SpriteRenderer> _lineY = new();
    private readonly Stack<SpriteRenderer> _linePool = new Stack<SpriteRenderer>();
    
    private Camera _mainCamera;
    private Vector3 _lastPosition;

    [SerializeField]
    private SpriteRenderer _spritePrefab;
    [Space]
    [Header("__SETTINGS__")]
    [SerializeField]    
    private Vector2Int _line = new Vector2Int(7, 7);
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
        _mainCamera = Camera.main;
        _lastPosition = _mainCamera.transform.position;
        CreateLines(_line);
    }

    private void Update()
    {
        if (_mainCamera.transform.position.x > _lastPosition.x)
        {
            Vector3 max = CameraHelper.GetMaxViewFromCameraTo(_mainCamera, transform);
        }
        _lastPosition = Camera.main.transform.position;
    }

    private void CreateLines(Vector2 expand)
    {
        for (int x = (int)(-expand.x / 2); x < expand.x / 2; x++)
        {
            SpriteRenderer sprite = GetLineInPool();
            sprite.transform.up = Vector3.right;
            sprite.transform.position = new Vector3(x, 0);
            _lineX.Add(sprite);
        }

        for (int y = (int)(-expand.y / 2); y < expand.y / 2; y++)
        {
            SpriteRenderer sprite = GetLineInPool();
            sprite.transform.position = new Vector3(0, y);
            _lineY.Add(sprite);
        }
    }

    private SpriteRenderer GetLineInPool()
    {
        if (_linePool.Count > 0)
        {
            return _linePool.Pop();
        }
        return Instantiate(_spritePrefab, transform);
    }

    private void OnDrawGizmos()
    {
        Vector3 fieldOfViewSize = CameraHelper.GetFieldOfViewSizeFromCameraTo(Camera.main, transform, out Vector3 fieldOfView);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fieldOfView, fieldOfViewSize);
    }
}
