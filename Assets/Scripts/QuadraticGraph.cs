using UnityEngine;
using UnityEngine.UI;

public class QuadraticGraph : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private readonly int numPoints = 100;  // Number of points to plot

    private readonly float xMin = -10f;  // Minimum x value
    private readonly float xMax = 10f;   // Maximum x value

    [SerializeField]
    private TMPro.TMP_InputField ifA;
    [SerializeField]
    private TMPro.TMP_InputField ifB;
    [SerializeField]
    private TMPro.TMP_InputField ifC;
    [SerializeField]
    private Button btnDraw;

    private void Awake()
    {
        btnDraw.onClick.AddListener(DrawQuadraticGraph);
    }

    public void DrawQuadraticGraph()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned.");
            return;
        }

        float a = float.Parse(ifA.text.Trim());
        float b = float.Parse(ifB.text.Trim());
        float c = float.Parse(ifC.text.Trim());

        float yLimit = 10f;

        Vector3[] positions = new Vector3[numPoints];
        float step = (xMax - xMin) / (numPoints - 1);
        int validPoints = 0;

        for (int i = 0; i < numPoints; i++)
        {
            float x = xMin + (i * step);
            float y = (a * x * x) + (b * x) + c;

            if (Mathf.Abs(y) > yLimit)
            {
                continue;  // Skip points where y exceeds the yLimit
            }

            positions[validPoints] = new Vector3(x, y, 0);
            validPoints++;
        }

        lineRenderer.positionCount = validPoints;
        lineRenderer.SetPositions(positions);
    }
}
