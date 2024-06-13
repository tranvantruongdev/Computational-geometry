using UnityEngine;

public class QuadraticGraph : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private int numPoints = 100;  // Number of points to plot

    [SerializeField]
    private float xMin = -10f;  // Minimum x value
    [SerializeField]
    private float xMax = 10f;   // Maximum x value

    public void DrawQuadraticGraph(float a, float b, float c)
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned.");
            return;
        }

        lineRenderer.positionCount = numPoints;
        Vector3[] positions = new Vector3[numPoints];
        float step = (xMax - xMin) / (numPoints - 1);

        for (int i = 0; i < numPoints; i++)
        {
            float x = xMin + i * step;
            float y = a * x * x + b * x + c;
            positions[i] = new Vector3(x, y, 0);
        }

        lineRenderer.SetPositions(positions);
    }
}

