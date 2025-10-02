using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 40;
    public float maxSpeed = 50;
    public float upSpeed = 20;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public Transform gameCamera;

    public GameObject gameOverCanvas;
    public TextMeshProUGUI finalScoreText;

    public Animator marioAnimator;
    public AudioSource marioAudio;

    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }
    public AudioClip marioDeath;
    public float deathImpulse = 15;

    [System.NonSerialized]
    public bool alive = true;

    public void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }


    public void GameOverScene()
    {
        // stop time
        Time.timeScale = 0.0f;
        // set gameover scene
        gameOverCanvas.SetActive(true);
        finalScoreText.text = scoreText.text;
    }


    public JumpOverGoomba jumpOverGoomba;

    // For restart button
    public TextMeshProUGUI scoreText;
    public GameObject enemies;

    private Rigidbody2D marioBody;



    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);

    }

    // Update is called once per frame

    void Update()
    {
        // toggle state
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        if ((col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Obstacles")) && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }


        if (col.gameObject.CompareTag("Enemy") && alive)
        {
            Debug.Log("Player collided with enemy!");

            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;

            // Time.timeScale = 0.0f;
            // gameOverCanvas.SetActive(true);
            // finalScoreText.text = scoreText.text;
        }
    }



    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.linearVelocity.magnitude < maxSpeed)
                marioBody.AddForce(movement * speed);
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            // stop
            marioBody.linearVelocity = Vector2.zero;
        }
        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }
        marioAnimator.SetBool("onGround", onGroundState);
    }



    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        gameOverCanvas.SetActive(false);
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        // transform.position = new Vector3(-3.259f, -2.45551f, -0.03384f);
        marioBody.transform.position = new Vector3(-3.259f, -2.45551f, -0.03384f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";

        jumpOverGoomba.ResetScore();
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }

        // reset question boxes
        foreach (QuestionBox qb in FindObjectsByType<QuestionBox>(FindObjectsSortMode.None))
        {
            qb.ResetBox();
        }

        // reset bricks
        foreach (Brick b in FindObjectsByType<Brick>(FindObjectsSortMode.None))
        {
            b.ResetBrick();
        }
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        gameCamera.position = new Vector3(0, 0, -10);


    }
}


