using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;                // Player to follow

    [Header("Limits References")]
    [SerializeField] private BoxCollider2D boundsCollider;    // Big room or level area
    [SerializeField] private BoxCollider2D deadZoneCollider;  // Defines the center zone where camera won't move

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.2f;         // Smoothing factor for camera movement

    private Camera _cam;
    private Vector3 _currentVelocity;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 camPos = transform.position;
        Vector3 targetPos = target.position;

        // Start from current camera position as "desired"
        Vector3 desiredPos = camPos;

        // --- DEAD ZONE LOGIC (using BoxCollider2D on the camera) ---
        if (deadZoneCollider != null)
        {
            // Local center of the dead zone relative to the camera
            Vector2 localCenter = deadZoneCollider.offset;
            Vector2 halfSize = deadZoneCollider.size * 0.5f;

            // Convert the dead zone center to world space
            Vector2 dzCenterWorld = (Vector2)transform.position + localCenter;

            // Offset between target and dead zone center
            float offsetX = targetPos.x - dzCenterWorld.x;
            float offsetY = targetPos.y - dzCenterWorld.y;

            // X axis: move camera only if target leaves dead zone horizontally
            if (Mathf.Abs(offsetX) > halfSize.x)
            {
                float signX = Mathf.Sign(offsetX); // +1 or -1
                // We want the target to be on the edge of the dead zone after moving
                float boundaryX = signX * halfSize.x;

                float targetMinusBoundary = targetPos.x - boundaryX;
                // Solve for new camera X position:
                //   dzCenterWorldNew.x = desiredPos.x + localCenter.x
                //   targetPos.x - dzCenterWorldNew.x = boundaryX
                // => desiredPos.x = targetPos.x - boundaryX - localCenter.x
                desiredPos.x = targetMinusBoundary - localCenter.x;
            }

            // Y axis: move camera only if target leaves dead zone vertically
            if (Mathf.Abs(offsetY) > halfSize.y)
            {
                float signY = Mathf.Sign(offsetY);
                float boundaryY = signY * halfSize.y;

                float targetMinusBoundary = targetPos.y - boundaryY;
                desiredPos.y = targetMinusBoundary - localCenter.y;
            }
        }
        else
        {
            // Fallback: if no deadZoneCollider assigned, just center camera on target
            desiredPos.x = targetPos.x;
            desiredPos.y = targetPos.y;
        }

        // --- BOUNDS CLAMPING (room) ---
        if (boundsCollider != null)
        {
            Bounds b = boundsCollider.bounds;

            float camHalfHeight = _cam.orthographicSize;
            float camHalfWidth = camHalfHeight * _cam.aspect;

            float minX = b.min.x + camHalfWidth;
            float maxX = b.max.x - camHalfWidth;
            float minY = b.min.y + camHalfHeight;
            float maxY = b.max.y - camHalfHeight;

            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);
        }

        // Keep current Z
        desiredPos.z = camPos.z;

        // Small threshold to avoid micro-jitter
        float sqrDist = (desiredPos - camPos).sqrMagnitude;
        float minMoveDist = 0.0001f;
        if (sqrDist < minMoveDist * minMoveDist)
        {
            return;
        }

        // Smoothly move camera to final position
        transform.position = Vector3.SmoothDamp(
            camPos,
            desiredPos,
            ref _currentVelocity,
            smoothTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        // Draw dead zone
        if (deadZoneCollider != null)
        {
            Gizmos.color = Color.yellow;

            Vector3 worldCenter = transform.position + (Vector3)deadZoneCollider.offset;
            Vector3 worldSize = new Vector3(deadZoneCollider.size.x, deadZoneCollider.size.y, 0f);

            Gizmos.DrawWireCube(worldCenter, worldSize);
        }

        // Draw room bounds
        if (boundsCollider != null)
        {
            Gizmos.color = Color.cyan;
            Bounds b = boundsCollider.bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}
