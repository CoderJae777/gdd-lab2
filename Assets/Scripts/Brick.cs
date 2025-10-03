using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Brick Settings")]
    public bool hasCoin = false;          // true = spawns coin, false = just bounce
    public GameObject coinPrefab;         // coin prefab reference

    [Header("Bounce Settings")]
    public float bounceHeight = 0.2f;     // how high the brick moves
    public float bounceSpeed = 5f;        // how fast the bounce happens

    private Vector3 originalPos;
    private bool used = false;            // brick state (used or not)

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // If already used, ignore
        if (used) return;

        // Only react if Mario hits from BELOW
        if (col.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f) // means player hit brick from below
                {
                    ActivateBrick();
                    break;
                }
            }
        }
    }

    private void ActivateBrick()
    {
        // Mark as used only if it had a coin
        if (hasCoin) used = true;

        // Start bounce animation (script-driven)
        StopAllCoroutines();
        StartCoroutine(BounceRoutine());

        // If it has a coin, spawn it
        if (hasCoin && coinPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            Debug.Log("Coin spawned at: " + spawnPos);

        }
    }

    private System.Collections.IEnumerator BounceRoutine()
    {
        Vector3 targetPos = originalPos + Vector3.up * bounceHeight;

        // Move up
        while (Vector3.Distance(transform.localPosition, targetPos) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition, targetPos, bounceSpeed * Time.deltaTime);
            yield return null;
        }

        // Move back down
        while (Vector3.Distance(transform.localPosition, originalPos) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition, originalPos, bounceSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localPosition = originalPos; // snap back
    }
    public void ResetBrick()
    {
        used = false;
        transform.localPosition = originalPos;
        // If you want to reset sprite, do it here
        // sr.sprite = defaultBrickSprite;
    }

}
