using UnityEngine;

public class FaceZoneChecker : MonoBehaviour
{
    [SerializeField] private RectTransform faceZone;
    [SerializeField] private Camera uiCamera;

    public bool IsInsideFaceZone(Vector2 screenPoint)
    {
        if (faceZone == null)
        {
            Debug.LogWarning("[FaceZoneChecker] faceZone не назначен!");
            return false;
        }
        return RectTransformUtility.RectangleContainsScreenPoint(faceZone, screenPoint, uiCamera);
    }

    private void OnDrawGizmos()
    {
        if (faceZone == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
        Vector3[] corners = new Vector3[4];
        faceZone.GetWorldCorners(corners);
        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);
    }
}
