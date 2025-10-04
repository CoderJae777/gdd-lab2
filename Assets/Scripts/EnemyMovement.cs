using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;

    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        // get the starting position
        originalX = transform.position.x;
        // record the spawn/start position so GameRestart can restore it
        startPosition = transform.localPosition;
        ComputeVelocity();
    }

    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }

    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        { // move goomba
            Movegoomba();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
    }

    void OnEnable()
    {
        Debug.Log("[EnemyMovement] OnEnable called for: " + gameObject.name);
        // When the enemy becomes active (including copied or pooled instances), ensure visuals and physics are reset.
        Transform visual = transform.Find("Visual");
        if (visual == null)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                visual = sr.transform;
        }

        if (visual != null)
        {
            Debug.Log(
                "[EnemyMovement] Visual found: "
                    + visual.name
                    + " (before reset scale="
                    + visual.localScale
                    + ")"
            );
            visual.localScale = Vector3.one;
            var anim = visual.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
                // ensure the squish flag is cleared on enable
                try
                {
                    if (AnimatorHasBoolParameter(anim, "squish"))
                    {
                        anim.SetBool("squish", false);
                        Debug.Log(
                            string.Format(
                                "[EnemyMovement] Cleared 'squish' bool on enable for {0}",
                                gameObject.name
                            )
                        );
                    }
                }
                catch (System.Exception) { }
                // log current animator state
                var state = anim.GetCurrentAnimatorStateInfo(0);
                Debug.Log(
                    "[EnemyMovement] Animator present on "
                        + visual.name
                        + ", current state hash="
                        + state.fullPathHash
                        + ", normalizedTime="
                        + state.normalizedTime
                );
                try
                {
                    anim.Play("idle", -1, 0f);
                    Debug.Log("[EnemyMovement] Played 'idle' on animator for " + gameObject.name);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("[EnemyMovement] Failed to Play('idle') on animator: " + ex.Message);
                }

                // Defensive fallback: force idle next frame to avoid lingering blend from squashed
                StartCoroutine(ForceSetIdle(anim, visual));
            }
        }

        // re-enable colliders and physics
        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var c in colliders)
            c.enabled = true;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = true;

        // ensure movement script is enabled
        this.enabled = true;
    }

    private System.Collections.IEnumerator ForceSetIdle(Animator anim, Transform visual)
    {
        // wait one frame to let animator settle
        yield return null;
        if (anim == null || visual == null)
            yield break;

        try
        {
            anim.Rebind();
            anim.Update(0f);
            int idleHash = Animator.StringToHash("Base Layer.idle");
            anim.Play(idleHash, 0, 0f);
            anim.Update(0f);
            Debug.Log("[EnemyMovement] ForceSetIdle applied to " + gameObject.name);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[EnemyMovement] ForceSetIdle failed: " + ex.Message);
        }

        // reset visual scale and ensure sprite renderer enabled
        visual.localScale = Vector3.one;
        var sr = visual.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = true;
    }

    public void GameRestart()
    {
        gameObject.SetActive(true);

        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();
        // Ensure this component is enabled so movement resumes
        this.enabled = true;

        // restore visual scale (in case squash animation scaled it)
        Transform visual = transform.Find("Visual");
        if (visual == null)
        {
            // fallback: first child with a SpriteRenderer
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                visual = sr.transform;
        }
        if (visual != null)
        {
            visual.localScale = Vector3.one;
            // reset animator state if present
            var anim = visual.GetComponent<Animator>();
            if (anim != null)
            {
                // return animator to its default state and force the idle state
                Debug.Log(
                    string.Format(
                        "[EnemyMovement] Resetting animator on '{0}' (visual='{1}')",
                        gameObject.name,
                        visual.name
                    )
                );
                anim.Rebind();
                anim.Update(0f);
                try
                {
                    if (AnimatorHasBoolParameter(anim, "squish"))
                    {
                        anim.SetBool("squish", false);
                        Debug.Log(
                            string.Format(
                                "[EnemyMovement] Cleared 'squish' bool on GameRestart for {0}",
                                gameObject.name
                            )
                        );
                    }
                }
                catch (System.Exception) { }
                // try to explicitly play 'idle' state (common name in controller)
                try
                {
                    anim.Play("idle", -1, 0f);
                }
                catch (System.Exception)
                {
                    // if 'idle' state not found, just leave it rebound
                }
            }
        }

        // re-enable colliders and physics
        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var c in colliders)
            c.enabled = true;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = true;
    }

    private bool AnimatorHasBoolParameter(Animator animator, string paramName)
    {
        if (animator == null)
            return false;
        foreach (var p in animator.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Bool && p.name == paramName)
                return true;
        }
        return false;
    }
}
