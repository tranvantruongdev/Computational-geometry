using TMPro;

using UnityEngine;

public class GraphSettings : MonoBehaviour
{
    [Header("Graph Settings")]
    [Space]
    public int updatePeriod = 5;
    [SerializeField] private Vector2 graphSize = new(800f, 400f);
    public Vector2 GraphSize
    {
        get => graphSize;
        set { graphSize = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePositionAndScale | GraphHandler.UpdateMethod.UpdateContent | GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [SerializeField] private Vector2 graphScale = new(100f, 100f);
    public Vector2 GraphScale
    {
        get => graphScale;
        set { graphScale = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePositionAndScale | GraphHandler.UpdateMethod.UpdateContent | GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [Space]
    [Header("Graph Visuals")]
    [Space]
    [SerializeField] private Color backgroundColor = new(0, 0, 0, 1f);
    public Color BackgroundColor
    {
        get => backgroundColor;
        set { backgroundColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateOutlines); }
    }
    [SerializeField] private float outlineWidth = 5f;
    public float OutlineWidth
    {
        get => outlineWidth;
        set { outlineWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateOutlines); }
    }

    [SerializeField] private Color outlineColor = new(0, 0.8f, 1f, 1f);
    public Color OutlineColor
    {
        get => outlineColor;
        set { outlineColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateOutlines); }
    }
    [Space]
    [SerializeField] private float lineWidth = 8f;
    public float LineWidth
    {
        get => lineWidth;
        set { lineWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateContent); }
    }
    [SerializeField] private Color lineColor = new(1f, 0.35f, 0f, 1f);
    public Color LineColor
    {
        get => lineColor;
        set { lineColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateContent); }
    }
    [Space]
    public Sprite PointSprite;
    [SerializeField] private float pointRadius = 5f;
    public float PointRadius
    {
        get => pointRadius;
        set { pointRadius = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    [SerializeField] private Color pointColor = new(1f, 0.35f, 0f, 1f);
    public Color PointColor
    {
        get => pointColor;
        set { pointColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    [Space]
    [SerializeField] private float pointHoverRadius = 15f;
    public float PointHoverRadius
    {
        get => pointHoverRadius;
        set { pointHoverRadius = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    public float PointHoverSpeed = 5f;

    [SerializeField] private Color pointHoverColor = new(1, 0.6f, 0, 1f);
    public Color PointHoverColor
    {
        get => pointHoverColor;
        set { pointHoverColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    [Space]
    [SerializeField] private float pointLockedRadius = 17f;
    public float PointLockedRadius
    {
        get => pointLockedRadius;
        set { pointLockedRadius = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    public float PointLockedSpeed = 5f;

    [SerializeField] private Color pointLockedColor = new(1, 0.8f, 0, 1f);
    public Color PointLockedColor
    {
        get => pointLockedColor;
        set { pointLockedColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    [Space]
    [SerializeField] private float unfixedPointOutlineWidth = 10f;
    public float UnfixedPointOutlineWidth
    {
        get => unfixedPointOutlineWidth;
        set { unfixedPointOutlineWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    [SerializeField] private Color unfixedPointOutlineColor = new(0, 0.8f, 1f, 1f);
    public Color UnfixedPointOutlineColor
    {
        get => unfixedPointOutlineColor;
        set { unfixedPointOutlineColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    [Space]
    [SerializeField] private float unfixedPointOutlineHoverWidth = 15f;
    public float UnfixedPointOutlineHoverWidth
    {
        get => unfixedPointOutlineHoverWidth;
        set { unfixedPointOutlineHoverWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    public float UnfixedPointOutlineHoverSpeed = 5f;

    [Space]
    [SerializeField] private Color unfixedPointOutlineHoverColor = new(0, 0.5f, 1f, 1f);
    public Color UnfixedPointOutlineHoverColor
    {
        get => unfixedPointOutlineHoverColor;
        set { unfixedPointOutlineHoverColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    [Space]
    [SerializeField] private float fixedPointOutlineWidth = 17f;
    public float FixedPointOutlineWidth
    {
        get => fixedPointOutlineWidth;
        set { fixedPointOutlineWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }
    public float FixedPointOutlineSpeed = 5f;
    [SerializeField] private Color fixedPointOutlineColor = new(0, 0.8f, 1f, 1f);
    public Color FixedPointOutlineColor
    {
        get => fixedPointOutlineColor;
        set { fixedPointOutlineColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdatePointVisuals); }
    }

    [Space]
    [Header("Grid Settings")]
    [Space]

    public TMP_FontAsset GridTextFont;
    [SerializeField] private Vector2 gridSpacing = new(1, 1);
    public Vector2 GridSpacing
    {
        get => gridSpacing;
        set { gridSpacing = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [Space]
    [SerializeField] private float xAxisWidth = 3f;
    public float XAxisWidth
    {
        get => xAxisWidth;
        set { xAxisWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }

    [SerializeField] private Color xAxisColor = new(0, 0.8f, 1f, 1f);
    public Color XAxisColor
    {
        get => xAxisColor;
        set { xAxisColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [Space]
    [SerializeField] private Color xAxisTextColor = new(0, 0.8f, 1f, 1f);
    public Color XAxisTextColor
    {
        get => xAxisTextColor;
        set { xAxisTextColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [SerializeField] private float xAxisTextSize = 10f;
    public float XAxisTextSize
    {
        get => xAxisTextSize;
        set { xAxisTextSize = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [SerializeField] private float xAxisTextOffset = 10f;
    public float XAxisTextOffset
    {
        get => xAxisTextOffset;
        set { xAxisTextOffset = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [Space]
    [SerializeField] private float yAxisWidth = 3f;
    public float YAxisWidth
    {
        get => yAxisWidth;
        set { yAxisWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }

    [SerializeField] private Color yAxisColor = new(0, 0.8f, 1f, 1f);
    public Color YAxisColor
    {
        get => yAxisColor;
        set { yAxisColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [Space]
    [SerializeField] private Color yAxisTextColor = new(0, 0.8f, 1f, 1f);
    public Color YAxisTextColor
    {
        get => yAxisTextColor;
        set { yAxisTextColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [SerializeField] private float yAxisTextSize = 10f;
    public float YAxisTextSize
    {
        get => yAxisTextSize;
        set { yAxisTextSize = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [SerializeField] private float yAxisTextOffset = 10f;
    public float YAxisTextOffset
    {
        get => yAxisTextOffset;
        set { yAxisTextOffset = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [Space]
    [SerializeField] private float xGridWidth = 2f;
    public float XGridWidth
    {
        get => xGridWidth;
        set { xGridWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [SerializeField] private Color xGridColor = new(0, 0.8f, 1f, 0.6f);
    public Color XGridColor
    {
        get => xGridColor;
        set { xGridColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }
    [Space]
    [SerializeField] private float yGridWidth = 2f;
    public float YGridWidth
    {
        get => yGridWidth;
        set { yGridWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }

    [SerializeField] private Color yGridColor = new(0, 0.8f, 1f, 0.6f);
    public Color YGridColor
    {
        get => yGridColor;
        set { yGridColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.UpdateGridLines); }
    }

    [Space]
    [SerializeField] private Color zoomSelectionColor = new(0, 0.8f, 1f, 0.2f);
    public Color ZoomSelectionColor
    {
        get => zoomSelectionColor;
        set { zoomSelectionColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.MouseAction); }
    }
    [SerializeField] private float zoomSelectionOutlineWidth = 5f;
    public float ZoomSelectionOutlineWidth
    {
        get => zoomSelectionOutlineWidth;
        set { zoomSelectionOutlineWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.MouseAction); }
    }
    [SerializeField] private Color zoomSelectionOutlineColor = new(0, 0.8f, 1f, 0.6f);
    public Color ZoomSelectionOutlineColor
    {
        get => zoomSelectionOutlineColor;
        set { zoomSelectionOutlineColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.MouseAction); }
    }
    [Space]
    [SerializeField] private Color pointSelectionColor = new(1, 0.35f, 0f, 0.2f);
    public Color PointSelectionColor
    {
        get => pointSelectionColor;
        set { pointSelectionColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.MouseAction); }
    }
    [SerializeField] private float pointSelectionOutlineWidth = 5f;
    public float PointSelectionOutlineWidth
    {
        get => pointSelectionOutlineWidth;
        set { pointSelectionOutlineWidth = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.MouseAction); }
    }
    [SerializeField] private Color pointSelectionOutlineColor = new(1, 0.35f, 0f, 0.4f);
    public Color PointSelectionOutlineColor
    {
        get => pointSelectionOutlineColor;
        set { pointSelectionOutlineColor = value; GH.UpdateGraphInternal(GraphHandler.UpdateMethod.MouseAction); }
    }
    [Space]
    public float ZoomSpeed = 5f;
    public float SmoothZoomSpeed = 20f;
    public float SmoothMoveSpeed = 20f;

    private GraphHandler GH;
    private void Awake()
    {
        GH = GetComponent<GraphHandler>();
    }
}
