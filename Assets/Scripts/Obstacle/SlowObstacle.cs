using UnityEngine;

public class SlowObstacle : MonoBehaviour
{
    [Header("Slow Settings")]
    [Tooltip("슬로우 적용 반경")]
    public float radius = 3f;

    [Tooltip("슬로우 배수")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.7f;

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

        float dist = Vector3.Distance(transform.position, player.transform.position);

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
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}