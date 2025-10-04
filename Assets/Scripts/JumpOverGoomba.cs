using System.Collections;
using TMPro;
using UnityEngine;

public class JumpOverGoomba : MonoBehaviour
{
    private Rigidbody2D marioBody;
    private GameManager gameManager;

    public float bounceForce = 10f;

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
        Animator anim = enemy.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Squish");
            yield return new WaitForSeconds(0.2f);
        }
        enemy.SetActive(false);
    }
}
