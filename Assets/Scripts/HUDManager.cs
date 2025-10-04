using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private Vector3[] scoreTextPosition = { new Vector3(-747, 473, 0), new Vector3(0, 0, 0) };
    private Vector3[] restartButtonPosition = { new Vector3(844, 455, 0), new Vector3(0, -150, 0) };

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText; // larger black score shown on game over
    public Transform restartButton;

    public GameObject gameOverCanvas;

    private int lastScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        // If designer hasn't assigned the scoreText field, try to find common names
        if (scoreText == null)
        {
            var go = GameObject.Find("ScoreText") ?? GameObject.Find("FinalScoreText");
            if (go != null)
            {
                var tmp = go.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    scoreText = tmp;
                    Debug.Log("[HUDManager] Auto-assigned scoreText from scene object: " + go.name);
                }
            }

            if (scoreText == null)
                Debug.LogWarning(
                    "[HUDManager] scoreText is not assigned in Inspector and auto-find failed."
                );
        }

        // try to find finalScoreText if not assigned (GameOver UI)
        if (finalScoreText == null)
        {
            var gof = GameObject.Find("FinalScoreText") ?? GameObject.Find("FinalScore");
            if (gof != null)
            {
                var tmpf = gof.GetComponent<TextMeshProUGUI>();
                if (tmpf != null)
                {
                    finalScoreText = tmpf;
                    Debug.Log(
                        "[HUDManager] Auto-assigned finalScoreText from scene object: " + gof.name
                    );
                }
            }
            if (finalScoreText == null)
                Debug.LogWarning("[HUDManager] finalScoreText not assigned and auto-find failed.");
        }
    }

    // Update is called once per frame
    void Update() { }

    public void GameStart()
    {
        // hide gameover panel
        gameOverCanvas.SetActive(false);
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            scoreText.gameObject.transform.localPosition = scoreTextPosition[0];
        }
        else
            Debug.LogWarning("[HUDManager] scoreText is not assigned in the inspector.");

        // hide final score until game over
        if (finalScoreText != null)
            finalScoreText.gameObject.SetActive(false);
        restartButton.localPosition = restartButtonPosition[0];
    }

    public void SetScore(int score)
    {
        Debug.Log($"[HUDManager] SetScore called with {score}");
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
        else
            Debug.LogWarning("[HUDManager] SetScore called but scoreText is not assigned.");
        lastScore = score;
    }

    public void GameOver()
    {
        // show game over panel
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        Debug.Log(
            $"[HUDManager] GameOver called. lastScore={lastScore}, scoreText={(scoreText != null ? scoreText.gameObject.name : "null")}, finalScoreText={(finalScoreText != null ? finalScoreText.gameObject.name : "null")}"
        );

        // hide in-game score text (white)
        if (scoreText != null)
            scoreText.gameObject.SetActive(false);
        else
            Debug.LogWarning("[HUDManager] GameOver: scoreText is not assigned.");

        // Determine authoritative score from GameManager if available
        int authoritativeScore = lastScore;
            GameManager gm = null;
            // First try: find by tag 'Manager'
            try
            {
                var gmObj = GameObject.FindWithTag("Manager");
                if (gmObj != null)
                {
                    gm = gmObj.GetComponent<GameManager>();
                    Debug.Log("[HUDManager] Found GameManager by tag 'Manager'.");
                }
            }
            catch (System.Exception) { }

            // Fallback: find by type if tag lookup failed
            if (gm == null)
            {
                gm = GameObject.FindAnyObjectByType<GameManager>();
                if (gm != null)
                    Debug.Log("[HUDManager] Found GameManager by type fallback.");
            }

            if (gm != null)
            {
                authoritativeScore = gm.GetScore();
                Debug.Log($"[HUDManager] Fetched authoritative score from GameManager: {authoritativeScore}");
            }
            else
            {
                Debug.LogWarning("[HUDManager] GameManager not found; using lastScore fallback: " + lastScore);
            }

        // show final score text (black/bigger) and set value
        if (finalScoreText != null)
        {
            Debug.Log(
                $"[HUDManager] GameOver: showing finalScoreText={finalScoreText.gameObject.name} with score={authoritativeScore}"
            );
            // make sure final score text is parented to the gameOverCanvas so it's visible above world UI
            if (
                gameOverCanvas != null
                && finalScoreText.transform.parent != gameOverCanvas.transform
            )
            {
                finalScoreText.transform.SetParent(gameOverCanvas.transform, false);
                // bring to front
                finalScoreText.transform.SetAsLastSibling();
            }

            finalScoreText.gameObject.SetActive(true);
            finalScoreText.text = "Score: " + authoritativeScore.ToString();
        }
        else
        {
            Debug.LogWarning("[HUDManager] GameOver: finalScoreText is not assigned.");
        }

        restartButton.localPosition = restartButtonPosition[1];
    }
}
