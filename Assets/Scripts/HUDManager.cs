using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private Vector3[] scoreTextPosition = { new Vector3(-747, 473, 0), new Vector3(0, 0, 0) };
    private Vector3[] restartButtonPosition = { new Vector3(844, 455, 0), new Vector3(0, -150, 0) };

    public TextMeshProUGUI scoreText;
    public Transform restartButton;

    public GameObject gameOverCanvas;

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
    }

    // Update is called once per frame
    void Update() { }

    public void GameStart()
    {
        // hide gameover panel
        gameOverCanvas.SetActive(false);
        if (scoreText != null)
            scoreText.gameObject.transform.localPosition = scoreTextPosition[0];
        else
            Debug.LogWarning("[HUDManager] scoreText is not assigned in the inspector.");
        restartButton.localPosition = restartButtonPosition[0];
    }

    public void SetScore(int score)
    {
        Debug.Log($"[HUDManager] SetScore called with {score}");
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
        else
            Debug.LogWarning("[HUDManager] SetScore called but scoreText is not assigned.");
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        if (scoreText != null)
            scoreText.gameObject.transform.localPosition = scoreTextPosition[1];
        else
            Debug.LogWarning("[HUDManager] GameOver: scoreText is not assigned.");
        restartButton.localPosition = restartButtonPosition[1];
    }
}
