using UnityEngine;

public static class CameraHelper
{
    public static Vector3 GetMinViewFromCameraTo(Camera camera, Transform transform)
    {
        Vector3 fieldOfViewPosition = GetFieldOfViewPosition(camera, transform);
        float distance = Vector3.Distance(fieldOfViewPosition, camera.transform.position);

        Vector3 minRect = camera.rect.min;
        minRect.z = distance;
        return camera.ViewportToWorldPoint(minRect);
    }

    public static Vector3 GetMaxViewFromCameraTo(Camera camera, Transform transform)
    {
        Vector3 fieldOfViewPosition = GetFieldOfViewPosition(camera, transform);
        float distance = Vector3.Distance(fieldOfViewPosition, camera.transform.position);

        Vector3 maxRect = camera.rect.max;
        maxRect.z = distance;
        return camera.ViewportToWorldPoint(maxRect);
    }

    public static Vector3 GetFieldOfViewSizeFromCameraTo(Camera camera, Transform transform, out Vector3 fieldOfViewPosition)
    {
        fieldOfViewPosition = GetFieldOfViewPosition(camera, transform);

        float distance = Vector3.Distance(fieldOfViewPosition, camera.transform.position);

        Vector3 sizeRect = camera.rect.size;
        sizeRect.z = distance;

        var center = GetRectCenter(camera, distance);

        var viewSize = (camera.ViewportToWorldPoint(sizeRect) - center) * 2f;
        viewSize.z = 0;
        return viewSize;
    }

    public static Vector3 GetFieldOfViewPosition(Camera camera, Transform transform)
    {
        Vector3 fieldOfViewPosition = camera.transform.position;
        fieldOfViewPosition.z = transform.position.z;
        return fieldOfViewPosition;
    }

    private static Vector3 GetRectCenter(Camera camera, float distance)
    {
        var center = camera.ViewportToWorldPoint(camera.rect.center);
        center.z = distance;
        return center;
    }
}
