using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public AudioSource enemyAudio;

    // cache initial positions of children so we can respawn them on restart
    private Vector3[] childStartPositions;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        if (enemyAudio != null)
            enemyAudio.Play();

        // cache child positions
        int count = transform.childCount;
        childStartPositions = new Vector3[count];
        for (int i = 0; i < count; i++)
            childStartPositions[i] = transform.GetChild(i).localPosition;

        // try to subscribe to GameManager restart event if present
        var gmObj = GameObject.FindGameObjectWithTag("Manager");
        if (gmObj != null)
        {
            gameManager = gmObj.GetComponent<GameManager>();
            if (gameManager != null)
                gameManager.gameRestart.AddListener(GameRestart);
        }
    }

    // Update is called once per frame
    void Update() { }

    public void GameRestart()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            var em = child.GetComponent<EnemyMovement>();
            Vector3 pos;
            if (i < childStartPositions.Length)
            {
                pos = childStartPositions[i];
            }
            else
            {
                // fallback: use current localPosition as a sensible start position
                pos = child.localPosition;
                Debug.Log(
                    $"[EnemyManager] No cached start position for child '{child.name}' (index {i}); using current localPosition={pos}"
                );
            }

            if (em != null)
            {
                Debug.Log($"[EnemyManager] Resetting enemy '{child.name}' to {pos}");
                em.startPosition = pos;

                em.GameRestart();
            }
            else
            {
                // if not EnemyMovement, still try to reactivate
                Debug.Log($"[EnemyManager] Reactivating non-enemy child '{child.name}' to {pos}");
                child.gameObject.SetActive(true);
                child.localPosition = pos;
            }
            i++;
        }
    }
}
