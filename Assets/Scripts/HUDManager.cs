using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private Vector3[] scoreTextPosition = { new Vector3(-747, 473, 0), new Vector3(0, 0, 0) };
    private Vector3[] restartButtonPosition = { new Vector3(844, 455, 0), new Vector3(0, -150, 0) };

    public GameObject scoreText;
    public Transform restartButton;

    public GameObject gameOverCanvas;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void GameStart()
    {
        // hide gameover panel
        gameOverCanvas.SetActive(false);
        scoreText.transform.localPosition = scoreTextPosition[0];
        restartButton.localPosition = restartButtonPosition[0];
    }

    public void SetScore(int score)
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        scoreText.transform.localPosition = scoreTextPosition[1];
        restartButton.localPosition = restartButtonPosition[1];
    }
}
