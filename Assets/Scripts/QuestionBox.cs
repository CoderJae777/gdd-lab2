using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public Animator boxAnimator;
    public Sprite usedSprite;
    public GameObject coinPrefab;

    private bool used = false;
    private SpriteRenderer sr;

    public float bounceHeight = 0.2f;
    public float bounceSpeed = 5f;

    private Vector3 originalPos;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPos = transform.localPosition;
    }

    public void Bounce()
    {
        StopAllCoroutines();
        StartCoroutine(BounceRoutine());
    }

    private System.Collections.IEnumerator BounceRoutine()
    {
        Vector3 targetPos = originalPos + Vector3.up * bounceHeight;

        // Move up
        while (Vector3.Distance(transform.localPosition, targetPos) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                targetPos,
                bounceSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Move back down
        while (Vector3.Distance(transform.localPosition, originalPos) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                originalPos,
                bounceSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.localPosition = originalPos; // snap to original position
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (used)
            return;

        if (col.gameObject.CompareTag("Player"))
        {
            // Check if collision is from below (Mario's head)
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f) // means Mario hit from below
                {
                    ActivateBox();
                    break;
                }
            }
        }
    }

    public void ResetBox()
    {
        used = false;
        transform.localPosition = originalPos;

        if (sr != null && usedSprite != null)
        {
            // restore the original sprite (only if you're swapping manually)
            // sr.sprite = originalSprite;  <-- keep a reference to it if needed
        }

        if (boxAnimator != null)
        {
            // Clear triggers
            boxAnimator.ResetTrigger("Hit");

            // Force back to Idle
            boxAnimator.Play("Idle", 0, 0f);
        }
    }

    void ActivateBox()
    {
        used = true;

        Bounce();

        // Play bounce animation
        if (boxAnimator != null)
            boxAnimator.SetTrigger("Hit");
        // Change sprite to used
        if (sr != null && usedSprite != null)
            sr.sprite = usedSprite;

        // Spawn coin
        if (coinPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1f;
            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
}
