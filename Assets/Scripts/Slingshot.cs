using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public GameObject pinPrefab;
    public Transform launchPoint;
    public float maxStretch = 3.0f;
    public float launchForceMultiplier = 10.0f;

    private bool isDragging = false;
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private LineRenderer lineRenderer;

    void Start()
    {
        // Placeholder for a slingshot band.
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragStartPos = GetWorldPositionFromTouch(touch.position);
                isDragging = true;
            }

            if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector3 currentPos = GetWorldPositionFromTouch(touch.position);
                DrawSlingshot(dragStartPos, currentPos);
            }

            if (touch.phase == TouchPhase.Ended && isDragging)
            {
                dragEndPos = GetWorldPositionFromTouch(touch.position);
                LaunchPin();
                isDragging = false;
                lineRenderer.enabled = false;
            }
        }
    }

    Vector3 GetWorldPositionFromTouch(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    void DrawSlingshot(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    void LaunchPin()
    {
        Vector3 launchDirection = dragStartPos - dragEndPos;
        launchDirection.z = 20;
        float stretch = Mathf.Clamp(launchDirection.magnitude, 0, maxStretch);
        launchDirection.Normalize();

        GameObject pin = Instantiate(pinPrefab, launchPoint.position, Quaternion.identity);
        pin.GetComponent<Rigidbody>().AddForce(launchDirection * stretch * launchForceMultiplier, ForceMode.Impulse);
    }
}
