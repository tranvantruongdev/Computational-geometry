using UnityEngine;

public class Sinwave : MonoBehaviour
{
    #region ===== Field =====

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int points;

    #endregion

    #region ===== Unity Event =====

    private void Update()
    {
        Draw();
    }

    #endregion

    #region ===== Private Function =====

    private void Draw()
    {
        float xStart = 0;
        float tau = 2 * Mathf.PI;
        float xFinish = tau;

        lineRenderer.positionCount = points;
        for (int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            float progress = (float)currentPoint/(points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = Mathf.Sin(x);

            lineRenderer.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }

    #endregion
}
