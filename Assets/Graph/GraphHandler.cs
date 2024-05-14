using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphHandler : MonoBehaviour
{
    #region ===== Field =====

    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_InputField ifA;
    [SerializeField] private TMP_InputField ifB;
    [SerializeField] private TMP_InputField ifC;

    private GraphSettings       GS;
    private RectTransform       graph;
    private RectTransform       graphContent;

    private Vector2             contentScale = Vector2.zero;

    private List<int>           sortedIndices;
    private Vector2Int          xAxisRange = new(-1, -1);
    private Vector2Int          prevXAxisRange = new(-1, -1);
    private Vector2             activePointValue = Vector2.zero;
    private bool                pointIsActive = false;

    private List<GameObject>            points;
    private List<Image>                 pointImages;
    private List<RectTransform>         pointRects;
    private List<GameObject>            pointOutlines;
    private List<RectTransform>         pointOutlineRects;
    private List<Image>                 pointOutlineImages;
    private List<GameObject>            lines;
    private List<RectTransform>         lineRects;
    private List<Image>                 lineImages;
    private List<RectTransform>         xGridRects;
    private List<Image>                 xGridImages;
    private List<TextMeshProUGUI>       xAxisTexts;
    private List<RectTransform>         xAxisTextRects;
    private List<RectTransform>         yGridRects;
    private List<Image>                 yGridImages;
    private List<TextMeshProUGUI>       yAxisTexts;
    private List<RectTransform>         yAxisTextRects;

    private readonly RectTransform      zoomSelectionRectTransform;
    private readonly RectTransform      pointSelectionRectTransform;

    private RectTransform               maskObj;
    private Image                       backgroundImage;
    private RectTransform               backgroundRect;
    private GameObject                  pointParent;
    private GameObject                  lineParent;
    private GameObject                  gridParent;
    private GameObject                  outlineParent;
    private List<RectTransform>         outlines;
    private List<Image>                 outlineImages;

    private List<int>       lockedHoveredPoints;
    private List<int>       lockedPoints;
    private List<int>       fixedHoveredPoints;
    private Vector2         contentOffset = Vector2.zero;

    private Vector2         bottomLeft, topRight, center;
    private List<int>       initialLockedPoints;
    private List<int>       recentlyLockedPoints;

    private bool            mouseInsideBounds = false;
    private Vector2         mousePos;
    private Vector2         previousMousePos;
    private Vector2         initialMousePos = Vector2.zero;
    private bool            initialMouseInsideBounds = false;
    private Vector2         zoomPoint = Vector2.zero;
    private Vector2         absoluteZoomPoint = Vector2.zero;
    private Vector2         zoom = new(1f, 1f);

    private Vector2         moveOffset;
    private Vector2         initialMoveOffset = Vector2.zero;

    private float           timeToUpdateMouse = 0;
    private float           timeToUpdateTouch = 0;
    private float           timeToUpdateScroll = 0;
    private bool            error;

    #endregion

    #region ===== Property =====

    public bool updateGraph = false;
    public List<Vector2> Values { get; private set; }
    public Vector2Int XAxisRange => xAxisRange;
    public int activePointIndex = -1;
    public Vector2 ActivePointValue => activePointValue;
    public int fixedPointIndex = -1;
    public Vector2 BottomLeft => bottomLeft;
    public Vector2 TopRight => topRight;
    public Vector2 Center => center;

    public enum MouseActionType
    {
        Move,
    }
    public MouseActionType mouseActionType;

    public Vector2 targetZoom = new(1f, 1f);
    public Vector2 targetMoveOffset;

    [Flags]
    public enum UpdateMethod
    {
        UpdatePositionAndScale = 1 << 0,
        UpdateOutlines = 1 << 1,
        UpdatePointVisuals = 1 << 2,
        UpdateContent = 1 << 3,
        MouseZoom = 1 << 4,
        MouseAction = 1 << 5,
        UpdateGridLines = 1 << 6,
        All = 1 << 7
    }

    #endregion

    #region ===== API =====

    public void CreatePoint(Vector2 newValue)
    {
        CreatePointInternal(newValue);
    }

    public void ChangePoint(int indexToChange, Vector2 newValue)
    {
        ChangePointInternal(indexToChange, newValue);
    }

    public void SetCornerValues(Vector2 newBottomLeft, Vector2 newTopRight)
    {
        SetCornerValuesInternal(newBottomLeft, newTopRight);
    }

    public void UpdateGraph()
    {
        UpdateGraphInternal(UpdateMethod.All);
    }

    public void DrawSinGraph()
    {
        for (float i = -5; i < 5; i += 0.2f)
        {
            CreatePoint(new Vector2(i, Mathf.Sin(i)));
        }

        UpdateGraph();
        targetZoom = new(1.030771f, 1.030771f);
        targetMoveOffset = new(-1347.53f, -547.9221f);
    }

    public void DrawCosGraph()
    {
        for (float i = -5; i < 5; i += 0.2f)
        {
            CreatePoint(new Vector2(i, Mathf.Cos(i)));
        }

        UpdateGraph();
        targetZoom = new(1.030771f, 1.030771f);
        targetMoveOffset = new(-1347.53f, -547.9221f);
    }

    public void DrawTanGraph()
    {
        for (float i = -10; i < 10; i += 0.2f)
        {
            CreatePoint(new Vector2(i, Mathf.Tan(i)));
        }

        UpdateGraph();
    }

    public void DrawCotGraph()
    {
        for (float i = -10; i < 10; i += 0.2f)
        {
            CreatePoint(new Vector2(i, 1 / Mathf.Tan(i)));
        }

        UpdateGraph();
    }

    public void DrawQuadraticFunction()
    {
        List<Vector2> vectors = new();
        int AValue = int.Parse(ifA.text.Trim());
        int BValue = int.Parse(ifB.text.Trim());
        int CValue = int.Parse(ifC.text.Trim());
        float xMin = -BValue / (2f * AValue);

        Func<float, float> getY = new(x => (AValue * x * x) + (BValue * x) + CValue);

        for (int i = 0; i < 15; i++)
        {
            if (i == 0)
            {
                vectors.Add(new Vector2(xMin, getY(xMin)));
            }
            else
            {
                float step = i * 0.25f;
                vectors.Insert(0, new Vector2(xMin - step, getY(xMin - step)));
                vectors.Add(new Vector2(xMin + step, getY(xMin + step)));
            }
        }

        foreach (Vector2 vector in vectors)
        {
            CreatePoint(vector);
        }

        UpdateGraph();
    }

    public void Clear()
    {
        foreach (GameObject point in points)
        {
            Destroy(point);
        }
        points.Clear();
        Values.Clear();

        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
        lines.Clear();

        lockedHoveredPoints.Clear();

        foreach (GameObject pointOutline in pointOutlines)
        {
            Destroy(pointOutline);
        }
        pointOutlines.Clear();

        sortedIndices.Clear();

        pointRects.Clear();

        fixedHoveredPoints.Clear();

        pointOutlineRects.Clear();

        lineRects.Clear();

        pointOutlineImages.Clear();

        UpdateGraph();
    }

    #endregion

    #region ===== Unity Event =====

    private void Awake()
    {
        Values = new List<Vector2>();
        sortedIndices = new List<int>();
        points = new List<GameObject>();
        pointRects = new List<RectTransform>();
        pointImages = new List<Image>();
        pointOutlines = new List<GameObject>();
        pointOutlineRects = new List<RectTransform>();
        pointOutlineImages = new List<Image>();
        lines = new List<GameObject>();
        lineRects = new List<RectTransform>();
        lineImages = new List<Image>();
        xGridRects = new List<RectTransform>();
        xGridImages = new List<Image>();
        yGridRects = new List<RectTransform>();
        yGridImages = new List<Image>();
        xAxisTexts = new List<TextMeshProUGUI>();
        yAxisTexts = new List<TextMeshProUGUI>();
        xAxisTextRects = new List<RectTransform>();
        yAxisTextRects = new List<RectTransform>();
        outlines = new List<RectTransform>();
        outlineImages = new List<Image>();
        lockedHoveredPoints = new List<int>();
        lockedPoints = new List<int>();
        initialLockedPoints = new List<int>();
        recentlyLockedPoints = new List<int>();
        fixedHoveredPoints = new List<int>();
    }

    private void Start()
    {
        if (CheckForErrors())
        {
            return;
        }

        GS = GetComponent<GraphSettings>();
        PrepareGraph();
    }

    private void Update()
    {
        mouseActionType = MouseActionType.Move;

        if (error)
        {
            return;
        }

        CheckIfUpdateGraph();
        if (updateGraph)
        {
            UpdateGraphInternal(UpdateMethod.All);
        }

        if (lockedHoveredPoints.Count > 0)
        {
            UpdatePoints();
        }

        if (fixedHoveredPoints.Count > 0)
        {
            UpdatePointOutlines();
        }

        zoom = Vector2.Lerp(zoom, targetZoom, GS.SmoothZoomSpeed * Time.deltaTime);
        moveOffset = Vector2.Lerp(moveOffset, targetMoveOffset, GS.SmoothMoveSpeed * Time.deltaTime);
    }

    #endregion

    #region ===== Private Function =====

    private void PrepareGraph()
    {
        if (canvas == null)
        {
            canvas = new GameObject("GraphCanvas").AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        if (GetComponent<RectTransform>() == null)
        {
            this.gameObject.AddComponent<RectTransform>();
        }

        graph = this.gameObject.GetComponent<RectTransform>();
        graph.SetParent(canvas.transform);
        graph.anchoredPosition = Vector2.zero;
        graph.sizeDelta = GS.GraphSize;

        maskObj = new GameObject("MaskObj").AddComponent<RectTransform>();
        maskObj.SetParent(graph);
        maskObj.anchoredPosition = Vector2.zero;
        maskObj.gameObject.AddComponent<Image>();
        Mask mask = maskObj.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        backgroundRect = new GameObject("Background").AddComponent<RectTransform>();
        backgroundRect.SetParent(maskObj);
        backgroundRect.anchoredPosition = Vector2.zero;
        backgroundImage = backgroundRect.gameObject.AddComponent<Image>();

        graphContent = new GameObject("GraphContent").AddComponent<RectTransform>();
        graphContent.SetParent(backgroundRect.transform);
        graphContent.sizeDelta = Vector2.zero;

        gridParent = CreateParent("GridParent");
        lineParent = CreateParent("LineParent");
        pointParent = CreateParent("PointParent");

        outlineParent = CreateParent("OutlineParent");
        CreateOutlines();
        outlineParent.transform.SetParent(graph);

        fixedPointIndex = -1;
        UpdateGraphInternal(UpdateMethod.All);
    }

    private GameObject CreateParent(string name)
    {
        GameObject parent = new(name);
        parent.transform.SetParent(name == "OutlineParent" ? graph : graphContent);
        Image image = parent.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0);
        image.raycastTarget = false;
        parent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        return parent;
    }

    private void CreateOutlines()
    {
        for (int i = 0; i < 4; i++)
        {
            Image outlineImage = new GameObject("Outline").AddComponent<Image>();
            RectTransform outline = outlineImage.GetComponent<RectTransform>();
            outline.SetParent(outlineParent.transform);
            outlineImage.color = GS.OutlineColor;
            outlineImage.raycastTarget = false;
            outlines.Add(outline);
            outlineImages.Add(outlineImage);
        }
    }

    private void CreatePointInternal(Vector2 value)
    {
        int i = points.Count;
        GameObject outline = CreatePointOutline(i);
        GameObject point = new("Point" + i);

        points.Add(point);
        Values.Add(value);

        point.transform.SetParent(outline.transform);

        Image image = point.AddComponent<Image>();
        image.color = GS.PointColor;
        image.sprite = GS.PointSprite;
        pointImages.Add(image);

        RectTransform pointRectTransform = point.GetComponent<RectTransform>();
        pointRectTransform.sizeDelta = Vector2.one * GS.PointRadius;
        pointRects.Add(pointRectTransform);

        EventTrigger trigger = point.AddComponent<EventTrigger>();
        var eventTypes = new[]
        {
            new { Type = EventTriggerType.PointerEnter, Callback = (Action) (() => MouseTrigger(i, true)) },
            new { Type = EventTriggerType.PointerExit, Callback = (Action) (() => MouseTrigger(i, false)) },
            new { Type = EventTriggerType.PointerClick, Callback = (Action) (() => PointClicked(i)) }
        };

        foreach (var eventType in eventTypes)
        {
            EventTrigger.Entry entry = new() { eventID = eventType.Type };
            entry.callback.AddListener((data) => { eventType.Callback(); });
            trigger.triggers.Add(entry);
        }

        if (points.Count > 1)
        {
            GameObject line = new("Line");
            line.transform.SetParent(lineParent.transform);
            lineImages.Add(line.AddComponent<Image>());
            line.GetComponent<Image>().color = GS.LineColor;
            lineRects.Add(line.GetComponent<RectTransform>());
            lines.Add(line);
            if (value.x < bottomLeft.x || value.x > topRight.x)
            {
                line.SetActive(false);
            }
        }

        lockedHoveredPoints.Add(i);
        SortIndices();

        if (value.x < bottomLeft.x || value.x > topRight.x)
        {
            outline.SetActive(false);
        }
    }

    private void ChangePointInternal(int index, Vector2 newValue)
    {
        Values[index] = newValue;
        SortIndices();
    }

    private void SortIndices()
    {
        sortedIndices = Values.Select((vector, index) => new { vector, index })
            .OrderBy(item => item.vector.x)
            .ThenBy(item => item.vector.y)
            .Select(item => item.index)
            .ToList();
    }

    private GameObject CreatePointOutline(int i)
    {
        GameObject outline = new("PointOutline" + i);
        pointOutlines.Add(outline);

        if (pointParent != null)
        {
            outline.transform.SetParent(pointParent.transform);
        }

        Image image = outline.AddComponent<Image>();
        image.color = GS.PointColor;
        pointOutlineImages.Add(image);
        RectTransform rectTransform = outline.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(GS.PointRadius, GS.PointRadius);
        pointOutlineRects.Add(rectTransform);

        Sprite sprite = GS.PointSprite;
        image.sprite = sprite;

        return outline;
    }

    private void CreateGridLineY()
    {
        GameObject yGrid = new("yGrid" + yGridRects.Count);
        yGrid.transform.SetParent(gridParent.transform);
        Image yGridImage = yGrid.AddComponent<Image>();
        yGridImage.raycastTarget = false;
        yGridRects.Add(yGrid.GetComponent<RectTransform>());
        yGridImages.Add(yGridImage);
        if (yGridRects.Count > 1)
        {
            TextMeshProUGUI yText = new GameObject("yText" + yGridRects.Count).AddComponent<TextMeshProUGUI>();
            RectTransform textRect = yText.gameObject.GetComponent<RectTransform>();
            textRect.SetParent(yGrid.GetComponent<RectTransform>());
            yText.font = GS.GridTextFont;
            yText.fontStyle = FontStyles.Bold;
            yText.alignment = TextAlignmentOptions.Center;
            yText.verticalAlignment = VerticalAlignmentOptions.Middle;
            yText.color = GS.YAxisTextColor;
            yText.enableAutoSizing = true;
            textRect.sizeDelta = Vector2.one * GS.YAxisTextSize;
            yText.raycastTarget = false;
            yAxisTexts.Add(yText);
            yAxisTextRects.Add(textRect);
        }
    }

    private void CreateGridLineX()
    {
        GameObject xGrid = new("xGrid" + xGridRects.Count);
        xGrid.transform.SetParent(gridParent.transform);
        Image xGridImage = xGrid.AddComponent<Image>();
        xGridImage.raycastTarget = false;
        xGridRects.Add(xGrid.GetComponent<RectTransform>());
        xGridImages.Add(xGridImage);
        if (xGridRects.Count > 1)
        {
            TextMeshProUGUI xText = new GameObject("xText" + xGridRects.Count).AddComponent<TextMeshProUGUI>();
            RectTransform textRect = xText.gameObject.GetComponent<RectTransform>();
            textRect.SetParent(xGrid.GetComponent<RectTransform>());
            xText.font = GS.GridTextFont;
            xText.fontStyle = FontStyles.Bold;
            xText.fontStyle = FontStyles.Bold;
            xText.alignment = TextAlignmentOptions.Center;
            xText.verticalAlignment = VerticalAlignmentOptions.Middle;
            xText.color = GS.XAxisTextColor;
            xText.enableAutoSizing = true;
            textRect.sizeDelta = Vector2.one * GS.XAxisTextSize;
            xText.raycastTarget = false;
            xAxisTexts.Add(xText);
            xAxisTextRects.Add(textRect);
        }
    }

    private void CheckIfUpdateGraph()
    {
        CalculateMousePosition();
        if (mouseInsideBounds)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            {
                timeToUpdateMouse = GS.updatePeriod;
            }
            if (Input.touchCount > 0)
            {
                timeToUpdateTouch = GS.updatePeriod;

            }
            if (Input.mouseScrollDelta.y != 0)
            {
                timeToUpdateScroll = GS.updatePeriod;
            }
        }
        if (timeToUpdateMouse > 0)
        {
            UpdateGraphInternal(UpdateMethod.UpdatePositionAndScale | UpdateMethod.UpdatePointVisuals | UpdateMethod.UpdateContent | UpdateMethod.MouseAction | UpdateMethod.UpdateGridLines);
        }

        if (timeToUpdateTouch > 0)
        {
            UpdateGraphInternal(UpdateMethod.UpdatePositionAndScale | UpdateMethod.UpdatePointVisuals | UpdateMethod.UpdateContent | UpdateMethod.MouseZoom | UpdateMethod.MouseAction | UpdateMethod.UpdateGridLines);
        }

        if (timeToUpdateScroll > 0)
        {
            UpdateGraphInternal(UpdateMethod.UpdatePositionAndScale | UpdateMethod.UpdatePointVisuals | UpdateMethod.UpdateContent | UpdateMethod.MouseZoom | UpdateMethod.UpdateGridLines);
        }

        timeToUpdateMouse -= Time.deltaTime;
        timeToUpdateTouch -= Time.deltaTime;
        timeToUpdateScroll -= Time.deltaTime;
    }

    public void UpdateGraphInternal(UpdateMethod methodsToUpdate)
    {
        if (methodsToUpdate.HasFlag(UpdateMethod.UpdatePositionAndScale) || methodsToUpdate.HasFlag(UpdateMethod.All))
        {
            UpdatePositionAndScale();
        }

        CalculateCornerValues();
        if (methodsToUpdate.HasFlag(UpdateMethod.UpdateOutlines) || methodsToUpdate.HasFlag(UpdateMethod.All))
        {
            UpdateOutlines();
        }

        if (methodsToUpdate.HasFlag(UpdateMethod.UpdateContent) || methodsToUpdate.HasFlag(UpdateMethod.All))
        {
            HandleActiveObjects();
        }

        if (methodsToUpdate.HasFlag(UpdateMethod.UpdateContent) || methodsToUpdate.HasFlag(UpdateMethod.All))
        {
            UpdateContent();
        }

        CalculateMousePosition();
        if (methodsToUpdate.HasFlag(UpdateMethod.MouseZoom) || methodsToUpdate.HasFlag(UpdateMethod.All))
        {
            MouseZoom();
        }

        if (methodsToUpdate.HasFlag(UpdateMethod.MouseAction) || methodsToUpdate.HasFlag(UpdateMethod.All))
        {
            MouseAction();
        }

        if (methodsToUpdate.HasFlag(UpdateMethod.UpdateGridLines) || methodsToUpdate.HasFlag(UpdateMethod.All))
        {
            UpdateGridLines();
        }
    }

    private void UpdatePositionAndScale()
    {
        contentScale = GS.GraphScale * zoom;
        maskObj.sizeDelta = GS.GraphSize;
        contentOffset = absoluteZoomPoint - (zoomPoint * contentScale) - moveOffset;
        graphContent.anchoredPosition = (-GS.GraphSize / 2) + contentOffset;
        graph.sizeDelta = GS.GraphSize;
        backgroundRect.sizeDelta = GS.GraphSize;
        backgroundImage.color = GS.BackgroundColor;
    }

    private void UpdateOutlines()
    {
        for (int i = 0; i < outlines.Count; i++)
        {
            if (i % 2 == 0) // Left and Right outlines
            {
                outlines[i].sizeDelta = new Vector2(GS.OutlineWidth, GS.GraphSize.y + (GS.OutlineWidth * 2));
                outlines[i].anchoredPosition = new Vector2((i == 0 ? -1 : 1) * (GS.GraphSize.x + GS.OutlineWidth) / 2, 0);
            }
            else // Top and Bottom outlines
            {
                outlines[i].sizeDelta = new Vector2(GS.GraphSize.x + (GS.OutlineWidth * 2), GS.OutlineWidth);
                outlines[i].anchoredPosition = new Vector2(0, (i == 1 ? -1 : 1) * (GS.GraphSize.y + GS.OutlineWidth) / 2);
            }
            outlineImages[i].color = GS.OutlineColor;
        }
    }

    private void CalculateCornerValues()
    {
        bottomLeft = -contentOffset / contentScale;
        topRight = bottomLeft + (GS.GraphSize / contentScale);
        center = ((topRight - bottomLeft) / 2f) + bottomLeft;
    }

    private void UpdateContent()
    {
        if (xAxisRange.x == -1 || xAxisRange.y == -1)
        {
            return;
        }

        Vector2 bounds = new(bottomLeft.y, topRight.y);
        for (int i = xAxisRange.x - 1; i <= xAxisRange.y + 1; i++)
        {
            if (i < 0 || i > sortedIndices.Count - 1)
            {
                continue;
            }

            int index = sortedIndices[i];
            float currentValue = Values[index].y;
            float prevValue = Values[Mathf.Clamp(index - 1, 0, Values.Count - 1)].y;
            float nextValue = Values[Mathf.Clamp(index + 1, 0, Values.Count - 1)].y;

            if ((currentValue < bounds.x && prevValue < bounds.x && nextValue < bounds.x) ||
                (currentValue > bounds.y && prevValue > bounds.y && nextValue > bounds.y))
            {
                continue;
            }
            UpdateAnchoredPosition(pointOutlineRects[index], CalculatePosition(i));
            if (lines.Count > 0 && index < lines.Count)
            {
                Vector2 point1 = CalculatePosition(index);
                Vector2 point2 = CalculatePosition(index + 1);
                float distance = Vector2.Distance(point1, point2);
                UpdateAnchoredPosition(lineRects[index], (point2 + point1) / 2f);
                UpdateSizeDelta(lineRects[index], new Vector2(distance, GS.LineWidth));
                Vector2 direction = point2 - point1;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRects[index].rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                lineImages[index].color = GS.LineColor;
            }
        }
    }

    private void HandleActiveObjects()
    {
        if (prevXAxisRange.x < xAxisRange.x)
        {
            for (int i = prevXAxisRange.x - 1; i < xAxisRange.x - 1; i++)
            {
                if (i < 0)
                {
                    continue;
                }

                pointOutlines[sortedIndices[i]].SetActive(false);
                if (i < lines.Count)
                {
                    lines[sortedIndices[i]].SetActive(false);
                }
            }
        }
        else if (prevXAxisRange.x > xAxisRange.x && xAxisRange.x >= 0)
        {
            for (int i = xAxisRange.x - 1; i < prevXAxisRange.x; i++)
            {
                if (i < 0)
                {
                    continue;
                }

                pointOutlines[sortedIndices[i]].SetActive(true);
                if (i < lines.Count)
                {
                    lines[sortedIndices[i]].SetActive(true);
                }
            }
        }
        if (prevXAxisRange.y > xAxisRange.y)
        {
            for (int i = xAxisRange.y + 2; i <= prevXAxisRange.y + 2; i++)
            {
                if (i > pointOutlines.Count - 1 || i < 0)
                {
                    continue;
                }

                pointOutlines[sortedIndices[i]].SetActive(false);
                if (i < lines.Count)
                {
                    lines[sortedIndices[i]].SetActive(false);
                }
            }
        }
        else if (xAxisRange.y > prevXAxisRange.y)
        {
            for (int i = prevXAxisRange.y + 2; i <= xAxisRange.y + 1; i++)
            {
                if (i > pointOutlines.Count - 1 || i < 0)
                {
                    continue;
                }

                pointOutlines[sortedIndices[i]].SetActive(true);
                if (i < lines.Count)
                {
                    lines[sortedIndices[i]].SetActive(true);
                }
            }
        }
        prevXAxisRange = xAxisRange;
        xAxisRange = new Vector2Int(MinMaxBinarySearch(true), MinMaxBinarySearch(false));
    }

    private Vector2 CalculatePosition(int i)
    {
        return Values[i] * contentScale;
    }

    private void MouseTrigger(int pointIndex, bool enter)
    {
        fixedHoveredPoints.Add(pointIndex);
        fixedHoveredPoints = fixedHoveredPoints.Distinct().ToList();

        if (enter)
        {
            activePointIndex = pointIndex;
            activePointValue = Values[pointIndex];
            pointIsActive = enter;
        }
        else
        {
            activePointIndex = pointIndex;
            activePointValue = Values[pointIndex];
            pointIsActive = enter;
        }
    }

    private void PointClicked(int pointIndex)
    {
        if (lockedPoints.Contains(pointIndex))
        {
            lockedPoints.Remove(pointIndex);
        }
        else
        {
            lockedPoints.Add(pointIndex);
        }
    }

    private void UpdatePoints()
    {
        for (int i = 0; i < lockedHoveredPoints.Count; i++)
        {
            Vector2 targetSize;
            Color targetColor;
            float targetSpeed;
            int numToUpdate = lockedHoveredPoints[i];
            bool isActive = activePointIndex == numToUpdate && pointIsActive;

            if (lockedPoints.Contains(numToUpdate))
            {
                targetSize = Vector2.one * GS.PointLockedRadius;
                targetColor = GS.PointLockedColor;
                targetSpeed = GS.PointLockedSpeed;
            }
            else
            {
                targetSize = Vector2.one * GS.PointRadius;
                targetColor = GS.PointColor;
                targetSpeed = GS.PointHoverSpeed;
            }

            pointRects[numToUpdate].sizeDelta = Vector2.Lerp(pointRects[numToUpdate].sizeDelta, targetSize, Time.deltaTime * targetSpeed);
            pointImages[numToUpdate].color = Color.Lerp(pointImages[numToUpdate].color, targetColor, Time.deltaTime * targetSpeed);

            if (!isActive && Vector2.Distance(pointRects[numToUpdate].sizeDelta, targetSize) < 0.5f && Vector4.Distance(pointImages[numToUpdate].color, targetColor) < 0.5f)
            {
                pointImages[numToUpdate].color = targetColor;
                pointRects[numToUpdate].sizeDelta = targetSize;
                lockedHoveredPoints.RemoveAt(i);
            }
            else if (!fixedHoveredPoints.Contains(numToUpdate) && numToUpdate != fixedPointIndex)
            {
                pointOutlineRects[numToUpdate].sizeDelta = pointRects[numToUpdate].sizeDelta + (Vector2.one * GS.UnfixedPointOutlineWidth);
            }
        }
        UpdatePointOutlines();
    }

    private void UpdatePointOutlines()
    {
        for (int i = 0; i < fixedHoveredPoints.Count; i++)
        {
            Vector2 targetSize;
            Color targetColor;
            float targetSpeed;
            int numToUpdate = fixedHoveredPoints[i];
            bool isActive = activePointIndex == numToUpdate && pointIsActive;

            if (fixedPointIndex == numToUpdate)
            {
                targetSize = new Vector2(GS.FixedPointOutlineWidth, GS.FixedPointOutlineWidth);
                targetColor = GS.FixedPointOutlineColor;
                targetSpeed = GS.FixedPointOutlineSpeed;
            }
            else
            {
                targetSize = new Vector2(GS.UnfixedPointOutlineWidth, GS.UnfixedPointOutlineWidth);
                targetColor = GS.UnfixedPointOutlineColor;
                targetSpeed = GS.UnfixedPointOutlineHoverSpeed;
            }

            targetSize += pointRects[numToUpdate].sizeDelta;
            RectTransform outlineRectTransform = pointOutlineRects[numToUpdate];

            outlineRectTransform.sizeDelta = targetSize;

            Image outlineImage = pointOutlineImages[numToUpdate];
            targetColor = Color.Lerp(outlineImage.color, targetColor, Time.deltaTime * targetSpeed);
            outlineImage.color = targetColor;

            if (!isActive && Vector2.Distance(outlineRectTransform.sizeDelta, targetSize) < 0.5f)
            {
                fixedHoveredPoints.RemoveAt(i);
            }
        }
    }

    private void UpdateGridLines()
    {
        Vector2 GridStartPoint;
        Vector2 spacing = CalculateGridSpacing();
        GridStartPoint = new Vector2(
            Mathf.Ceil(bottomLeft.x * spacing.x) / spacing.x,
            Mathf.Ceil(bottomLeft.y * spacing.y) / spacing.y) * contentScale;
        int2 eventualOverlay = new(-1, -1);
        int requiredYGridlines = Mathf.CeilToInt((topRight.y - bottomLeft.y) * spacing.y) + 1;
        int requiredXGridlines = Mathf.CeilToInt((topRight.x - bottomLeft.x) * spacing.x) + 1;
        while (xGridRects.Count <= requiredXGridlines)
        {
            CreateGridLineX();
        }
        while (yGridRects.Count <= requiredYGridlines)
        {
            CreateGridLineY();
        }

        for (int i = 0; i < requiredXGridlines; i++)
        {
            RectTransform rect = xGridRects[i];
            Image rectImage = xGridImages[i];
            if (!rect.gameObject.activeSelf)
            {
                rect.gameObject.SetActive(true);
            }

            if (i == 0)
            {
                UpdateSizeDelta(rect, new Vector2(GS.XAxisWidth, GS.GraphSize.y * 2f));
                rectImage.color = GS.XAxisColor;
                UpdateAnchoredPosition(rect, new Vector2(0, center.y * contentScale.y));
            }
            else
            {
                UpdateSizeDelta(rect, new Vector2(GS.XGridWidth, GS.GraphSize.y * 2f));
                rectImage.color = GS.XGridColor;

                if (Mathf.Round(GridStartPoint.x + ((i + eventualOverlay.x) / spacing.x * contentScale.x)) == 0)
                {
                    eventualOverlay.x = 0;
                }

                UpdateAnchoredPosition(rect, new Vector2(GridStartPoint.x + ((i + eventualOverlay.x) / spacing.x * contentScale.x), center.y * contentScale.y));
                UpdateSizeDelta(xAxisTextRects[i - 1], new Vector2(1f / spacing.x * contentScale.x, GS.XAxisTextSize));
                UpdateAnchoredPosition(xAxisTextRects[i - 1], new Vector2(0, (-center.y * contentScale.y) + GS.XAxisTextOffset));

                xAxisTexts[i - 1].text
                    = Mathf.Floor(1f / spacing.x) > 0
                        ? Mathf.RoundToInt((GridStartPoint.x / contentScale.x) + ((i + eventualOverlay.x) / spacing.x)).ToString()
                        : ((GridStartPoint.x / contentScale.x) + ((i + eventualOverlay.x) / spacing.x)).ToString("F0");
            }
        }

        for (int i = 0; i < requiredYGridlines; i++)
        {
            RectTransform rect = yGridRects[i];
            Image rectImage = yGridImages[i];
            if (!rect.gameObject.activeSelf)
            {
                rect.gameObject.SetActive(true);
            }

            if (i == 0)
            {
                UpdateSizeDelta(rect, new Vector2(GS.GraphSize.x * 2f, GS.YAxisWidth));
                rectImage.color = GS.YAxisColor;
                UpdateAnchoredPosition(rect, new Vector2(center.x * contentScale.x, 0));
            }
            else
            {
                UpdateSizeDelta(rect, new Vector2(GS.GraphSize.x * 2f, GS.YGridWidth));
                rectImage.color = GS.YGridColor;

                if (Mathf.Round(GridStartPoint.y + ((i + eventualOverlay.y) / spacing.y * contentScale.y)) == 0)
                {
                    eventualOverlay.y = 0;
                }

                UpdateAnchoredPosition(rect, new Vector2(center.x * contentScale.x, GridStartPoint.y + ((i + eventualOverlay.y) / spacing.y * contentScale.y)));
                UpdateSizeDelta(yAxisTextRects[i - 1], new Vector2(1f / spacing.x * contentScale.x, GS.XAxisTextSize));
                UpdateAnchoredPosition(yAxisTextRects[i - 1], new Vector2((-center.x * contentScale.x) + GS.YAxisTextOffset, 0));
                yAxisTexts[i - 1].text = Mathf.Floor(1f / spacing.y) > 0 ? Mathf.RoundToInt((GridStartPoint.y / contentScale.y) + ((i + eventualOverlay.y) / spacing.y)).ToString() : ((GridStartPoint.y / contentScale.y) + ((i + eventualOverlay.y) / spacing.y)).ToString("R");
            }
        }
        for (int i = requiredXGridlines; i < xGridRects.Count; i++)
        {
            if (xGridRects[i].gameObject.activeSelf)
            {
                xGridRects[i].gameObject.SetActive(false);
            }
        }
        for (int i = requiredYGridlines; i < yGridRects.Count; i++)
        {
            if (yGridRects[i].gameObject.activeSelf)
            {
                yGridRects[i].gameObject.SetActive(false);
            }
        }
    }

    private Vector2 CalculateGridSpacing()
    {
        int exponentX = Mathf.FloorToInt(Mathf.Log(zoom.x, 2));
        int exponentY = Mathf.FloorToInt(Mathf.Log(zoom.y, 2));
        float closestX = Mathf.Pow(2, exponentX);
        float closestY = Mathf.Pow(2, exponentY);
        return new Vector2(closestX, closestY) * GS.GridSpacing;
    }

    private void SetCornerValuesInternal(Vector2 newBottomLeft, Vector2 newTopRight)
    {
        Vector2 newCenter = ((newTopRight - newBottomLeft) / 2f) + newBottomLeft;
        targetMoveOffset = ((newCenter - center) * contentScale) + moveOffset;

        ChangeZoomPoint(newCenter);
        targetZoom = GS.GraphSize / GS.GraphScale / (newTopRight - newBottomLeft);
    }

    private void CalculateMousePosition()
    {
        mousePos = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - new Vector2(graphContent.transform.position.x, graphContent.transform.position.y)) / contentScale;
        mouseInsideBounds = mousePos.x > bottomLeft.x && mousePos.y > bottomLeft.y && mousePos.x < topRight.x && mousePos.y < topRight.y;
        mousePos = new Vector2(Mathf.Clamp(mousePos.x, bottomLeft.x, topRight.x), Mathf.Clamp(mousePos.y, bottomLeft.y, topRight.y));
    }

    private void MouseZoom()
    {
        if (!mouseInsideBounds)
        {
            return;
        }

        if (Input.mouseScrollDelta.y == 0)
        {
            return;
        }

        if (fixedPointIndex == -1)
        {
            ChangeZoomPoint(mousePos);
        }

        targetZoom = zoom + (GS.ZoomSpeed * Input.mouseScrollDelta.y * zoom / 100f);

        if (targetZoom.x > 1.971156f)
        {
            targetZoom.x = targetZoom.y = 1.971156f;
            return;
        }

        if (targetZoom.x < 1.012735f)
        {
            targetZoom.x = targetZoom.y = 1.012735f;
        }
    }

    private void ChangeZoomPoint(Vector2 newZoomPoint)
    {
        absoluteZoomPoint = ((newZoomPoint - zoomPoint) * contentScale) + absoluteZoomPoint;
        zoomPoint = newZoomPoint;
    }

    private void MouseAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialMouseInsideBounds = mouseInsideBounds;
            if (!mouseInsideBounds)
            {
                return;
            }

            initialMousePos = Input.mousePosition;
            if (mouseActionType == MouseActionType.Move)
            {
                initialMoveOffset = moveOffset;
            }
            return;
        }
        if (Input.GetMouseButton(0) && initialMouseInsideBounds)
        {
            if (Input.GetMouseButtonDown(1))
            {
                initialMouseInsideBounds = false;
                zoomSelectionRectTransform.gameObject.SetActive(false);
                pointSelectionRectTransform.gameObject.SetActive(false);
            }
            if (previousMousePos != mousePos)
            {
                Vector2 currentMousePos = Input.mousePosition;
                if (mouseActionType == MouseActionType.Move)
                {
                    targetMoveOffset = initialMousePos - currentMousePos + initialMoveOffset;
                }
            }
            previousMousePos = mousePos;
        }
        else if (Input.GetMouseButtonUp(0) && initialMouseInsideBounds)
        {
            recentlyLockedPoints.Clear();
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                initialMousePos = touch.position;
                mousePos = (new Vector2(touch.position.x, touch.position.y) - new Vector2(graphContent.transform.position.x, graphContent.transform.position.y)) / contentScale;
                initialMouseInsideBounds = mousePos.x > bottomLeft.x && mousePos.y > bottomLeft.y && mousePos.x < topRight.x && mousePos.y < topRight.y;
                if (mouseActionType == MouseActionType.Move)
                {
                    initialMoveOffset = moveOffset;
                }
            }
            else if (touch.phase == TouchPhase.Moved && initialMouseInsideBounds)
            {
                if (mouseActionType == MouseActionType.Move)
                {
                    Vector2 currentTouchPos = touch.position;
                    targetMoveOffset = initialMousePos - currentTouchPos + initialMoveOffset;
                }
            }
            else if (touch.phase == TouchPhase.Ended && initialMouseInsideBounds)
            {
                recentlyLockedPoints.Clear();
            }
        }
    }

    private int MinMaxBinarySearch(bool findLeft) //important for large numbers of points
    {
        //this function finds the points that are closest to the sides of the graph window
        float target;
        target = findLeft ? bottomLeft.x : topRight.x;
        int min = 0;
        int max = sortedIndices.Count - 1;
        float value;
        while (min <= max)
        {
            int middle = min + ((max - min) / 2);
            value = Values[sortedIndices[middle]].x;
            if (findLeft ? value >= target : value <= target)
            {
                if ((findLeft && (middle == 0 || Values[sortedIndices[middle - 1]].x < target)) ||
                    (!findLeft && (middle == sortedIndices.Count - 1 || Values[sortedIndices[middle + 1]].x > target)))
                {
                    return middle;
                }

                if (findLeft)
                {
                    max = middle - 1;
                }
                else
                {
                    min = middle + 1;
                }
            }
            else
            {
                if (findLeft)
                {
                    min = middle + 1;
                }
                else
                {
                    max = middle - 1;
                }
            }
        }
        return -1;
    }

    private void UpdateSizeDelta(RectTransform rect, Vector2 size)
    {
        if (Mathf.Abs(rect.sizeDelta.x - size.x) > 0.1f || Mathf.Abs(rect.sizeDelta.y - size.y) > 0.1f)
        {
            rect.sizeDelta = size;
        }
    }

    private void UpdateAnchoredPosition(RectTransform rect, Vector2 position)
    {
        if (Mathf.Abs(rect.sizeDelta.x - position.x) > 0.1f || Mathf.Abs(rect.sizeDelta.y - position.y) > 0.1f)
        {
            rect.anchoredPosition = position;
        }
    }

    private bool CheckForErrors()
    {
        if (GetComponent<GraphSettings>() == null)
        {
            Debug.LogError("This GameObject has no GraphSettings script attached. Attach GraphSettings and restart");
            error = true;
            return true;
        }
        if (GetComponent<GraphSettings>().GridTextFont == null)
        {
            Debug.LogError("No font was found. Assign a font for GraphSettings.GridTextFont and restart");
            error = true;
        }
        if (GetComponent<GraphSettings>().PointSprite == null)
        {
            Debug.LogError("No point sprite was found. Assign a sprite for GraphSettings.PointSprite and restart");
            error = true;
        }
        return error;
    }

    #endregion
}


