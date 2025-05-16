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

    private BossAnimationController animController;
    private BossAudioManager bossSound;

    public Transform m_player;
    public Rigidbody2D m_playerRb;

    public float awakeDistance = 4.0f; // boss will stay between awakeDistance and scaredDistance when awake
    public float scaredDistance = 6.0f; // boss will stay between scaredDistance and 1.5x scaredDistance when scared
    public float distanceToPlayer = 0f;

    public float moveSpeed = 3.0f;
    public float idleWiggleAmount = 0.2f;
    public float idleWiggleSpeed = 0.25f;

    public float attackCheckCooldown = 3f;
    public float minionSpawnTime = 2f;
    public float minionsSpawned = 4f;
    public float rangedAttackTime = 0.3f;
    public float rangedAttack2Time = 1f;
    public float meleeRange = 2.5f;
    public float longRange = 6f;

    public int orcCount = 0;

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
    public GameObject orcPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // getting various boss components for use later
        bossHealth = GetComponent<BossHealth>();
        animController = GetComponent<BossAnimationController>();
        bossSound = GetComponent<BossAudioManager>();
    }

    void FixedUpdate()
    {
        if (m_player == null || m_playerRb == null) // if there isn't a player return as we will do nothing
            return;

        distanceToPlayer = Vector2.Distance(transform.position, m_player.position); // get player location and speed
        float playerSpeed = m_playerRb.linearVelocity.magnitude;

        switch (currentState)
        {
            case BossState.Sleeping:
                if (distanceToPlayer < awakeDistance) // if player comes close to the boss, then activate it
                {
                    currentState = BossState.Awake;
                    animController.AwakenBoss();
                    bossSound.PlayBossWakesUp();
                }
                break;

            case BossState.Awake:
                if (bossHealth != null && bossHealth.currentHealth < 6) // if the boss gets low on HP it will become faster and move to scared state
                {
                    currentState = BossState.Scared;
                    bossGotScared = true;
                    moveSpeed *= 2.0f;
                    break;
                }
                if (playerSpeed < 0.1f && distanceToPlayer > awakeDistance && distanceToPlayer < scaredDistance) // if the player is not doing anything then start the idle loop (that way the player can't just stand still and game the AI)
                {
                    currentState = BossState.Idle;
                    Vector2 toBoss = (Vector2)transform.position - (Vector2)m_player.position; // various declarations relevant to idle movement, see movement function for this
                    idleCenter = m_player.position;
                    idleRadius = toBoss.magnitude;
                    idleArcAngle = Mathf.Atan2(toBoss.y, toBoss.x);
                    idleTimer = 0f;
                }
                break;

            case BossState.Scared:
                if (playerSpeed < 0.1f && distanceToPlayer > scaredDistance && distanceToPlayer < 1.5f * scaredDistance) // scared code same as active except no transition to scared (duh)
                {
                    currentState = BossState.Idle;
                    Vector2 toBoss = (Vector2)transform.position - (Vector2)m_player.position;
                    idleCenter = m_player.position;
                    idleRadius = toBoss.magnitude;
                    idleArcAngle = Mathf.Atan2(toBoss.y, toBoss.x);
                    idleTimer = 0f;
                }
                break;

            case BossState.Idle:
                if (bossHealth != null && bossHealth.currentHealth < 6 && bossGotScared == false) // need to enter scared if player attacks from idle
                {
                    currentState = BossState.Scared;
                    bossGotScared = true;
                    moveSpeed *= 2.0f;
                    break;
                }
                // Return to active if player does anything
                if (Input.anyKeyDown)
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
        CheckForWallsAndAvoid(); // avoid walls if any
        HandleMovement(distanceToPlayer); // start movement based on state
    }

    // I did not write this section, credit to Brian here
    private bool avoidingWall = false;
    private Vector2 centerPoint = new Vector2(1.08f, 52f); // Cambia según tu mapa
    private float avoidWallDuration = 1.5f;
    private float avoidWallEndTime = 0f;

    private float wallAvoidCooldown = 0f;
    private float wallAvoidDelay = 0.3f; // tiempo mínimo entre intentos de evitar paredes

    Vector2 wallPushDirection = Vector2.zero; // Global variable al inicio de la clase
    float wallAvoidDuration = 1.0f;           // Tiempo de empuje
    float wallAvoidTimer = 0f;                // Contador

    void CheckForWallsAndAvoid()
    {
        if (Time.time < wallAvoidCooldown)
            return;

        float wallCheckDistance = 1.5f;
        LayerMask wallMask = LayerMask.GetMask("Walls");
        Vector2[] directions = {
        Vector2.right, Vector2.left, Vector2.up, Vector2.down,
        new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized,
        new Vector2(1, -1).normalized, new Vector2(-1, -1).normalized
    };

        foreach (Vector2 dir in directions)
        {
            Vector2 origin = rb.position + dir * 0.3f; // mover el origen un poco hacia afuera
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheckDistance, wallMask);

            if (hit.collider != null && hit.collider.gameObject != this.gameObject)
            {
                Debug.Log("PARED DETECTADA: " + hit.collider.name);
                wallPushDirection = -dir;
                wallAvoidTimer = wallAvoidDuration;
                wallAvoidCooldown = Time.time + wallAvoidDelay;
                Debug.DrawRay(origin, dir * wallCheckDistance, Color.red);
                break;
            }
            else
            {
                Debug.DrawRay(origin, dir * wallCheckDistance, Color.green);
            }
        }
    }


    // end brian code



    void HandleMovement(float distanceToPlayer) // movement function
    {
        if (m_player == null || attackState != AttackState.NotAttacking) return; // if the player doesn't exist or we are attacking, we shouldn't be handling movement...

        if (wallAvoidTimer > 0f) // wall avoid timer decrement (for if moving away from wall)
        {
            rb.MovePosition(rb.position + wallPushDirection * moveSpeed * 2f * Time.deltaTime);
            wallAvoidTimer -= Time.deltaTime;
            return;
        }


        Vector2 toPlayer = (m_player.position - transform.position).normalized; // get player location
        Vector2 moveTarget = transform.position;

        switch (currentState)
        {
            case BossState.Sleeping: // if sleeping, we have no movement
                rb.linearVelocity = Vector2.zero;
                return;

            case BossState.Awake: // if awake they try to stay between awakeDistance and 1.5 awakeDistance
                if (distanceToPlayer < awakeDistance)
                    moveTarget = (Vector2)transform.position - toPlayer;
                else if (distanceToPlayer > 1.5f * awakeDistance)
                    moveTarget = (Vector2)transform.position + toPlayer;
                else
                    return;
                break;

            case BossState.Scared: // if scared try to stay between scaredDistance and 1.5 scaredDistance
                if (distanceToPlayer < scaredDistance)
                    moveTarget = (Vector2)transform.position - toPlayer;
                else if (distanceToPlayer > 1.5f * scaredDistance)
                    moveTarget = (Vector2)transform.position + toPlayer;
                else
                    return;
                break;

            case BossState.Idle: // complex idle movement
                idleTimer += Time.deltaTime * idleWiggleSpeed; // update the timer for idle

                float angleOffset = Mathf.Sin(idleTimer) * (Mathf.PI / 3f); // get the offset for where we should be based on the current idle time
                float totalAngle = idleArcAngle + angleOffset; // get the full angle
                Vector2 offset = new Vector2(Mathf.Cos(totalAngle), Mathf.Sin(totalAngle)) * idleRadius; // offset as a vector
                moveTarget = idleCenter + offset; // our target location is the center + offset (center is on the player)

                Vector2 arcDir = (moveTarget - (Vector2)transform.position).normalized;
                float arcDistance = Vector2.Distance(transform.position, moveTarget);
                Vector2 rayOrigin = (Vector2)transform.position;

                RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, arcDir, arcDistance, LayerMask.GetMask("Default")); // check for walls in idle

                foreach (var hit in hits) // we have to  check all hits because by default the boss will see itself as a wall
                {
                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj == this.gameObject) // skip if itself
                        continue;

                    idleTimer = Mathf.PI - idleTimer; // otherwise reset idle time to the time it would be if we finished the sin wave and are coming back to the current location from the other direction
                    break;
                }

                Debug.DrawLine(idleCenter, moveTarget, Color.magenta); // debug lines for idle
                Debug.DrawRay(rayOrigin, arcDir * arcDistance, Color.yellow);
                break;
        }

        // Ejecutar movimiento normal
        rb.MovePosition(Vector2.MoveTowards(rb.position, moveTarget, moveSpeed * Time.deltaTime));

        // Dirección del sprite
        if (toPlayer.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (toPlayer.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
    }


    void SelectAttack() // assume we are not currently attacking via gate to activate this, then pick 1 of 4 attacks
    {
        orcCount = GameObject.FindGameObjectsWithTag("Enemy").Length - 1; // how many orcs are there?
        if (orcCount < 1 && distanceToPlayer >= longRange) // if there are 0 and the player is far enough away, then good, spawn orc minion
        {
            attackState = AttackState.SpawnMinions;
            StartCoroutine(DoAttack());
            return;
        }
        else if (distanceToPlayer <= meleeRange) // if close to player do melee attack
        {
            attackState = AttackState.MeleeAttack;
            StartCoroutine(DoAttack());
            return;
        }
        else if ((distanceToPlayer >= meleeRange && distanceToPlayer < longRange) || currentState == BossState.Idle) // if medium distance or in idle to fireball (idle means player not moving so fireball should always hit)
        {
            attackState = AttackState.RangedAttack;
            StartCoroutine(DoAttack());
            return;
        }
        else if (distanceToPlayer >= longRange) // if far away and not spwaning orc then do fire rain (this seems to not work as fire rain spawns with orc sometimes)
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
            case AttackState.SpawnMinions: // spawn minions
                animController.PlaySummon(); // animation
                bossSound.PlayBossSpawnsMinion(); // sound
                yield return new WaitForSeconds(minionSpawnTime); // wait to spawn (charging effect)
                float checkDistance = 1f;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, checkDistance, LayerMask.GetMask("Default"));
                if (hit.collider == null) // if a wall on the right, spawn on left, otherwise spawn on right
                    Instantiate(orcPrefab, transform.position + Vector3.right, Quaternion.identity);
                else
                    Instantiate(orcPrefab, transform.position - Vector3.right, Quaternion.identity);
                animController.EndSummon(); // animation
                break;
            case AttackState.MeleeAttack: // melee attack
                animController.PlayAttack(); // animation
                Vector2 toPlayer = (m_player.position - transform.position).normalized; // find player location in case it changed
                rb.linearVelocity = 2f * moveSpeed * toPlayer; // rush player
                yield return new WaitForSeconds(1f); // don't change movement while charging
                break;
            case AttackState.RangedAttack: // fireball
                animController.PlayAttack(); // animation
                yield return new WaitForSeconds(rangedAttackTime); // wait while shooting
                Vector2 direction = (m_player.position - transform.position).normalized; // get direction to player and instantiate fireball facing that position
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
                fireball.transform.rotation = Quaternion.Euler(0, 0, angle + 45f);
                bossSound.PlayBossShootsFireball(); // sound after fireball spawns, not before (since we have to wait)
                break;
            case AttackState.RangedAttack2: // fire rain
                animController.PlaySummon(); // animation
                yield return new WaitForSeconds(rangedAttack2Time); // wait for summon
                Vector3 spawnPos = new Vector3(m_player.position.x, m_player.position.y, 0f); // spawn fire rain on player
                Instantiate(firerainPrefab, spawnPos, Quaternion.identity);
                animController.EndSummon(); // animation
                bossSound.PlayFireRain(); // sound after fire rain spawns, since the sound is the rain
                break;
        }
        attackState = AttackState.NotAttacking;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player")) // collision management, if hit the player then do some damage and a lot of knockback... fire hurts

        {
            Player_Health playerHealth = collision.gameObject.GetComponent<Player_Health>();

            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-3);
                Debug.Log("Boss hit player");
            }
            else
                Debug.Log("Boss hit player, but health was null");

            PlayerMovement2 playerMovement = collision.gameObject.GetComponent<PlayerMovement2>();
            if (playerMovement != null)
            {
                playerMovement.Knockback(transform, 100f, 0.5f);
            }
            else
                Debug.Log("Boss hit player, but movement was null");
        }
    }
}