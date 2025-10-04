using UnityEngine;

public class Coin : MonoBehaviour
{
    // Coin Settings
    public float riseHeight = 2f;
    public float speed = 4f;
    public AudioClip coinSound;

    // Coin State
    private Vector3 startPos;
    private bool rising = true;

    void Start()
    {
        startPos = transform.position;

        // Play coin sound once on spawn
        if (coinSound != null)
        {
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            // Debug.Log("[Coin] Played coin sound.");
        }
        else
        {
            // Debug.LogWarning("[Coin] No coinSound assigned!");
        }
    }

    void Update()
    {
        if (rising)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            if (transform.position.y >= startPos.y + riseHeight)
                rising = false;
        }
        else
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
            if (transform.position.y <= startPos.y)
                {
                    // before destroying, award score to player
                    try
                    {
                        var gm = GameObject.FindGameObjectWithTag("Manager");
                        if (gm != null)
                        {
                            var gmc = gm.GetComponent<GameManager>();
                            if (gmc != null)
                                gmc.IncreaseScore(1);
                            else
                            {
                                // fallback: try find by type
                                var gmByType = GameObject.FindAnyObjectByType<GameManager>();
                                if (gmByType != null)
                                    gmByType.IncreaseScore(1);
                            }
                        }
                    }
                    catch (System.Exception) { }

                    Destroy(gameObject); // disappears back into box
                }
        }
    }
}
