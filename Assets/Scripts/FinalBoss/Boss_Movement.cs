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
    public enum AttackState
    {
        NotAttacking, // default state, when the boss is not attacking already
        SpawnMinions, // spawning minions, take w seconds to spawn z minions. Only do this if at y distance and minions are not currently spawned (if they die boss can spawn more)
        MeleeAttack, // use this attack if the player is within x distance of player, boss will rush player, attack, then run back again. This should do significant knockback, since it's mainly to get player away from the boss. very fast attack, no charge
        RangedAttack, // use this attack if between x distance and y distance, the boss will stop for a few frames, shoot a fireball, then keep running. HOWEVER boss will always use this attack when in idle if minions are not being spawned
        RangedAttack2 // use this attack if the player is y distance or greater away, unlike the fireball it's aoe so harder to dodge but takes w/n seconds to load so more risky
    }

    public BossState currentState = BossState.Sleeping;
    public AttackState attackState = AttackState.NotAttacking;

    public Transform m_player;
    public Rigidbody2D m_playerRb;

    public float awakeDistance = 4.0f; // boss will stay between awakeDistance and scaredDistance when awake
    public float scaredDistance = 6.0f; // boss will stay between scaredDistance and 1.5x scaredDistance when scared
    public float distanceToPlayer = 0f;

    public float moveSpeed = 3.0f;
    public float idleWiggleAmount = 0.2f;
    public float idleWiggleSpeed = 0.25f;

    public float attackCheckCooldown = 3f;
    public float minionSpawnTime = 3f;
    public float minionsSpawned = 4f;
    public float rangedAttackTime = 0.3f;
    public float rangedAttack2Time = 1f;
    public float meleeRange = 2.5f;
    public float longRange = 6f;

    private Rigidbody2D rb;
    private float idleTimer;
    private float idleRadius;
    private Vector2 idleCenter;
    private float idleArcAngle;
    private BossHealth bossHealth;
    private bool bossGotScared = false;
    private float nextAttackCheckTime = 0f;

    public GameObject fireballPrefab;
    public GameObject firerainPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bossHealth = GetComponent<BossHealth>();
    }

    void FixedUpdate()
    {
        if (m_player == null || m_playerRb == null)
            return;

        distanceToPlayer = Vector2.Distance(transform.position, m_player.position);
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
                if (playerSpeed < 0.1f && distanceToPlayer > scaredDistance && distanceToPlayer < 1.5f * scaredDistance)
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
        if (currentState != BossState.Sleeping && attackState == AttackState.NotAttacking && Time.time >= nextAttackCheckTime) // check whether we should/can attack player
        {
            nextAttackCheckTime = Time.time + attackCheckCooldown;
            SelectAttack();
        }
        HandleMovement(distanceToPlayer);
    }


    void HandleMovement(float distanceToPlayer)
    {
        if (m_player == null || attackState != AttackState.NotAttacking) return;

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
                else if (distanceToPlayer > 1.5f * awakeDistance)
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

    void SelectAttack() // assume we are not currently attacking via gate to activate this, then pick 1 of 4 attacks
    {
        if (distanceToPlayer >= longRange && false)
        {
            attackState = AttackState.SpawnMinions;
            StartCoroutine(DoAttack());
            return;
        }

        if (distanceToPlayer <= meleeRange)
        {
            attackState = AttackState.MeleeAttack;
            StartCoroutine(DoAttack());
            return;
        }

        if ((distanceToPlayer >= meleeRange && distanceToPlayer < longRange) || currentState == BossState.Idle)
        {
            attackState = AttackState.RangedAttack;
            StartCoroutine(DoAttack());
            return;
        }

        if (distanceToPlayer >= longRange)
        {
            attackState = AttackState.RangedAttack2;
            StartCoroutine(DoAttack());
            return;
        }
    }


    IEnumerator DoAttack()
    {
        rb.linearVelocity = Vector2.zero; // Stop movement during attack
        switch (attackState)
        {
            case AttackState.SpawnMinions:
                yield return new WaitForSeconds(minionSpawnTime);
                break;
            case AttackState.MeleeAttack:
                Vector2 toPlayer = (m_player.position - transform.position).normalized;
                rb.linearVelocity = 2f*moveSpeed*toPlayer;
                yield return new WaitForSeconds(1f);
                break;
            case AttackState.RangedAttack:
                yield return new WaitForSeconds(rangedAttackTime);
                Instantiate(fireballPrefab, transform.position, Quaternion.identity);
                break;
            case AttackState.RangedAttack2:
                yield return new WaitForSeconds(rangedAttack2Time);
                Instantiate(firerainPrefab, m_player.position, Quaternion.identity);
                break;
        }
        attackState = AttackState.NotAttacking;
    }
}