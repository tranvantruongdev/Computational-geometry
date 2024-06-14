using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererGraph : MonoBehaviour
{
    private List<SpriteRenderer> _lineX;
    private List<SpriteRenderer> _lineY;
    private Stack<SpriteRenderer> _linePool = new Stack<SpriteRenderer>();
    private Vector3 _lastPosition;

    [SerializeField]
    private GameObject _prefab;
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
        _lastPosition = Camera.main.transform.position;
    }

    private void Update()
    {

        _lastPosition = Camera.main.transform.position;
    }

    private void OnDrawGizmos()
    {
        Vector3 fieldOfViewSize = CameraHelper.GetFieldOfViewSizeFromCameraTo(Camera.main, transform, out Vector3 fieldOfView);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(fieldOfView, fieldOfViewSize);
    }
}
