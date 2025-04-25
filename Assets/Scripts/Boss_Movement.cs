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
    
    public float awakeDistance = 4.0f; // boss will stay between awakeDistance and scaredDistance when awake
    public float scaredDistance = 6.0f; // boss will stay between scaredDistance and 1.5x scaredDistance when scared

    public float moveSpeed = 3.0f;
    public float idleWiggleAmount = 0.2f;
    public float idleWiggleSpeed = 0.25f;

    private Rigidbody2D rb;
    private float idleTimer;
    private float idleRadius;
    private Vector2 idleCenter;
    private float idleArcAngle;
    private BossHealth bossHealth;
    private bool bossGotScared = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bossHealth = GetComponent<BossHealth>();
    }

    void FixedUpdate()
    {
        if (m_player == null || m_playerRb == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, m_player.position);
        float playerSpeed = m_playerRb.linearVelocity.magnitude;

        switch (currentState)
        {
            case BossState.Sleeping:
                if (distanceToPlayer < awakeDistance)
                {
                    currentState = BossState.Awake;
                }
                break;

            case BossState.Awake:
                if (bossHealth != null && bossHealth.currentHealth < 6)
                {
                    currentState = BossState.Scared;
                    bossGotScared = true;
                    moveSpeed *= 2.0f;
                    break;
                }
                if (playerSpeed < 0.1f && distanceToPlayer > awakeDistance && distanceToPlayer < scaredDistance)
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
                if (playerSpeed < 0.1f && distanceToPlayer > scaredDistance && distanceToPlayer < 1.5f*scaredDistance)
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
                if (bossHealth != null && bossHealth.currentHealth < 6 && bossGotScared == false)
                {
                    currentState = BossState.Scared;
                    bossGotScared = true;
                    moveSpeed *= 2.0f;
                    break;
                }
                // Return to active if player moves
                if (playerSpeed > 0.1f)
                {
                    if (bossGotScared)
                        currentState = BossState.Scared;
                    else
                        currentState = BossState.Awake;
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
                if (distanceToPlayer < awakeDistance)
                    moveTarget = (Vector2)transform.position - toPlayer;
                else if (distanceToPlayer > 1.5f*awakeDistance)
                    moveTarget = (Vector2)transform.position + toPlayer;
                else
                    return;
                break;

            case BossState.Scared:
                if (distanceToPlayer < scaredDistance)
                    moveTarget = (Vector2)transform.position - toPlayer;
                else if (distanceToPlayer > scaredDistance)
                    moveTarget = (Vector2)transform.position + toPlayer;
                else
                    return;
                break;

            case BossState.Idle:
                idleTimer += Time.deltaTime * idleWiggleSpeed;

                float angleOffset = Mathf.Sin(idleTimer) * (Mathf.PI / 3f);
                float totalAngle = idleArcAngle + angleOffset;
                Vector2 offset = new Vector2(Mathf.Cos(totalAngle), Mathf.Sin(totalAngle)) * idleRadius;
                moveTarget = idleCenter + offset;

                Vector2 arcDir = (moveTarget - (Vector2)transform.position).normalized;
                float arcDistance = Vector2.Distance(transform.position, moveTarget);
                Vector2 rayOrigin = (Vector2)transform.position;

                RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, arcDir, arcDistance, LayerMask.GetMask("Default")); // find objects with hitbox in default layer in our way

                foreach (var hit in hits) // the raycast is stupid and annoyingly always hits the boss itself first, so we have to check ALL raycasts to fnd the wall
                {
                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj == this.gameObject)
                        continue;

                    idleTimer = Mathf.PI - idleTimer;
                    break;
                }

                Debug.DrawLine(idleCenter, moveTarget, Color.magenta); // shows where the boss wants to go from the center of the arc of sin
                Debug.DrawRay(rayOrigin, arcDir * arcDistance, Color.yellow); // this traces the distance from boss to where it's trying to go (if any), thus showing if it's hit the wall yet
                break;

        }

        rb.MovePosition(Vector2.MoveTowards(rb.position, moveTarget, moveSpeed * Time.deltaTime));
    }
}
