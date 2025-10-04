using System.Collections;
using TMPro;
using UnityEngine;

public class JumpOverGoomba : MonoBehaviour
{
    private Rigidbody2D marioBody;
    private GameManager gameManager;

    public float bounceForce = 10f;

    // fallback duration (seconds) to wait for squash animation if clip length not inferred
    public float squashDuration = 0.5f;

    void Start()
    {
        marioBody = GetComponent<Rigidbody2D>();
        var gmObj = GameObject.FindGameObjectWithTag("Manager");
        if (gmObj != null)
        {
            gameManager = gmObj.GetComponent<GameManager>();
            if (gameManager == null)
                Debug.LogWarning(
                    "[JumpOverGoomba] Manager object found but GameManager component missing."
                );
        }
        else
        {
            Debug.LogWarning(
                "[JumpOverGoomba] Manager GameObject with tag 'Manager' not found in scene."
            );
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                // ✅ landed on top
                if (contact.normal.y > 0.5f)
                {
                    // Squish enemy
                    StartCoroutine(SquishEnemy(col.gameObject));

                    // Bounce Mario upward
                    marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0);
                    marioBody.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

                    // Increase score (try to lazily find GameManager if not cached)
                    if (gameManager == null)
                    {
                        var gmObj = GameObject.FindGameObjectWithTag("Manager");
                        if (gmObj != null)
                            gameManager = gmObj.GetComponent<GameManager>();
                    }

                    if (gameManager != null)
                    {
                        Debug.Log("[JumpOverGoomba] Increasing score via GameManager");
                        gameManager.IncreaseScore(1);
                    }
                    else
                    {
                        Debug.LogWarning(
                            "[JumpOverGoomba] Could not find GameManager to increase score."
                        );
                    }

                    break;
                }
            }
        }
    }

    IEnumerator SquishEnemy(GameObject enemy)
    {
        // Stop movement and collisions first
        var enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
            enemyMovement.enabled = false;

        var colliders = enemy.GetComponentsInChildren<Collider2D>();
        foreach (var c in colliders)
            c.enabled = false;

        var rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Try to find an Animator on the visual child
        Animator anim = enemy.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            // use a persistent bool parameter so animation event can clear it later
            anim.SetBool("squish", true);
        }

        // Do not deactivate here; the squashed animation should call an Animation Event
        // (OnSquashFinished) on the visual which will clear the flag and deactivate the enemy.
        yield break;
    }
}
