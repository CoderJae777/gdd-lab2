using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    private int score = 0;

    void Start()
    {
        gameStart.Invoke();
        // ensure event containers are initialized (in case not set in inspector)
        if (scoreChange == null)
            scoreChange = new UnityEvent<int>();

        if (gameRestart == null)
            gameRestart = new UnityEvent();

        if (gameStart == null)
            gameStart = new UnityEvent();

        if (gameOver == null)
            gameOver = new UnityEvent();

        // Try to auto-wire HUD manager if present in scene
        var hudObj = GameObject.FindAnyObjectByType<HUDManager>();
        if (hudObj != null)
        {
            Debug.Log("[GameManager] HUDManager found; subscribing HUD handlers.");
            // subscribe HUD to scoreChange event
            scoreChange.AddListener(hudObj.SetScore);
            // subscribe HUD to game over and restart events
            gameOver.AddListener(hudObj.GameOver);
            gameRestart.AddListener(hudObj.GameStart);
        }
        else
        {
            Debug.Log("[GameManager] HUDManager not found during Start().");
        }
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update() { }

    public void GameRestart()
    {
        // reset score
        score = 0;
        SetScore(score);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        Debug.Log($"[GameManager] IncreaseScore called. New score={score}");
        SetScore(score);
    }

    public void SetScore(int score)
    {
        Debug.Log($"[GameManager] SetScore invoking scoreChange with {score}");
        // Invoke event for any listeners
        if (scoreChange != null)
        {
            try
            {
                scoreChange.Invoke(score);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[GameManager] Exception invoking scoreChange: " + ex.Message);
            }
        }

        // Fallback: if HUDManager didn't subscribe for any reason, update it directly so the in-game score shows
        var hud = GameObject.FindAnyObjectByType<HUDManager>();
        if (hud != null)
        {
            hud.SetScore(score);
        }
    }

    // Return the authoritative current score
    public int GetScore()
    {
        return score;
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }
}
