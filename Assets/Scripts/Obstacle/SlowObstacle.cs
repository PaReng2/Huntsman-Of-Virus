using UnityEngine;

public class SlowObstacle : MonoBehaviour
{
    [Header("둔화 범위")]
    public float radius = 3f;

    [Header("둔화율")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.5f;

    private PlayerController player;
    private bool isSlowing = false;
    private float originalSpeed;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        if (player == null) return;

        Vector2 playerXZ = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 centerXZ = new Vector2(transform.position.x, transform.position.z);

        float dist = Vector2.Distance(playerXZ, centerXZ);

        if (dist <= radius && !isSlowing)
        {
            originalSpeed = player.playerMoveSpeed;
            player.playerMoveSpeed *= slowMultiplier;
            isSlowing = true;
        }
        else if (dist > radius && isSlowing)
        {
            player.playerMoveSpeed = originalSpeed;
            isSlowing = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Vector3 center = transform.position;
        
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            center = rend.bounds.center;
            center.y = transform.position.y; 
        }

        int segments = 64;
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            Vector3 nextPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}