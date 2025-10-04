using UnityEngine;

// Attach this to the Visual child (Animator) and add an Animation Event
// calling OnSquashFinished at the last frame of the squashed clip.
public class SquashEventHandler : MonoBehaviour
{
    [Tooltip("Seconds to hold flattened pose before deactivation (optional)")]
    public float holdDuration = 0.0f;

    // This is called from the Animation Event at the end of the squashed clip
    public void OnSquashFinished()
    {
        StartCoroutine(HandleSquashEnd());
    }

    private System.Collections.IEnumerator HandleSquashEnd()
    {
        if (holdDuration > 0f)
            yield return new WaitForSeconds(holdDuration);

        // Find the Animator on this GameObject
        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            // clear squish flag
            try { anim.SetBool("squish", false); } catch { }
            try { anim.ResetTrigger("Squish"); } catch { }
        }

        // Find the top-level enemy root (parent with EnemyMovement) and deactivate
        Transform t = transform;
        while (t != null && t.GetComponent<EnemyMovement>() == null)
        {
            t = t.parent;
        }

        if (t != null)
        {
            // re-enable physics/colliders if needed (in case we want a short delay)
            var colliders = t.GetComponentsInChildren<Collider2D>();
            foreach (var c in colliders)
                c.enabled = true;

            var rb = t.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.simulated = true;

            // deactivate the enemy root (or return to pool)
            t.gameObject.SetActive(false);
        }
    }
}
