using System.Collections;
using UnityEngine;

public class Boss_Movement : MonoBehaviour
{
    public enum BossState
    {
        Sleeping,
        Awake,
        Scared,
        Idle
    }

    public BossState currentState = BossState.Sleeping;

    public Transform m_player;
    public Rigidbody2D m_playerRb;

    public float wakeDistance = 4.0f;
    public float awakeDistance = 4.0f;
    public float scaredDistance = 8.0f;

    public float moveSpeed = 6.0f;
    public float idleWiggleAmount = 0.2f;
    public float idleWiggleSpeed = 0.25f;

    private Rigidbody2D rb;
    private float idleTimer;
    private float idleRadius;
    private Vector2 idleCenter;
    private float idleArcAngle;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (m_player == null || m_playerRb == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, m_player.position);
        float playerSpeed = m_playerRb.linearVelocity.magnitude;

        switch (currentState)
        {
            case BossState.Sleeping:
                if (distanceToPlayer < wakeDistance)
                {
                    currentState = BossState.Awake;
                }
                break;

            case BossState.Awake:
                if (playerSpeed < 0.1f && Mathf.Abs(distanceToPlayer - awakeDistance) < 0.2f)
                {
                    currentState = BossState.Idle;
                    Vector2 toBoss = (Vector2)transform.position - (Vector2)m_player.position;
                    idleCenter = m_player.position;
                    idleRadius = toBoss.magnitude;
                    idleArcAngle = Mathf.Atan2(toBoss.y, toBoss.x); // Angle where the boss is when idle starts
                    idleTimer = 0f;
                }
                break;

            case BossState.Scared:
                if (playerSpeed < 0.1f && Mathf.Abs(distanceToPlayer - scaredDistance) < 0.2f)
                {
                    currentState = BossState.Idle;
                    Vector2 toBoss = (Vector2)transform.position - (Vector2)m_player.position;
                    idleCenter = m_player.position;
                    idleRadius = toBoss.magnitude;
                    idleArcAngle = Mathf.Atan2(toBoss.y, toBoss.x); // Angle where the boss is when idle starts
                    idleTimer = 0f;
                }
                break;

            case BossState.Idle:
                // Return to active if player moves
                if (playerSpeed > 0.1f)
                {
                    float dist = Vector2.Distance(transform.position, m_player.position);
                    currentState = dist > awakeDistance * 1.5f ? BossState.Scared : BossState.Awake;
                }
                break;
        }

        HandleMovement(distanceToPlayer);
    }


    void HandleMovement(float distanceToPlayer)
    {
        if (m_player == null) return;

        Vector2 toPlayer = (m_player.position - transform.position).normalized;
        Vector2 moveTarget = transform.position;

        switch (currentState)
        {
            case BossState.Sleeping:
                rb.linearVelocity = Vector2.zero;
                return;

            case BossState.Awake:
                if (distanceToPlayer < awakeDistance * 0.95f)
                    moveTarget = (Vector2)transform.position - toPlayer;
                else if (distanceToPlayer > awakeDistance * 1.05f)
                    moveTarget = (Vector2)transform.position + toPlayer;
                else
                    return;
                break;

            case BossState.Scared:
                if (distanceToPlayer < scaredDistance * 0.95f)
                    moveTarget = (Vector2)transform.position - toPlayer;
                else if (distanceToPlayer > scaredDistance * 1.05f)
                    moveTarget = (Vector2)transform.position + toPlayer;
                else
                    return;
                break;

            case BossState.Idle: // boss will hit the walls, that's fine idrc
                idleTimer += Time.deltaTime * idleWiggleSpeed;

                float angleOffset = Mathf.Sin(idleTimer) * (Mathf.PI / 3f);

                float totalAngle = idleArcAngle + angleOffset;

                Vector2 offset = new Vector2(Mathf.Cos(totalAngle),Mathf.Sin(totalAngle)) * idleRadius;

                moveTarget = idleCenter + offset;

                Debug.DrawLine(idleCenter, moveTarget, Color.magenta);

                break;
        }

        rb.MovePosition(Vector2.MoveTowards(rb.position, moveTarget, moveSpeed * Time.deltaTime));
    }
}
