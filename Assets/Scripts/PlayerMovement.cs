using System.Collections;
using System.Collections.Generic;
// using System.Numerics;

// since only using Vector2/3/4, don't need to import System.Numerics
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
// MonoBehaviour lets the script be attached to a GameObject
// It’s a special Unity base class that gives you access to Unity’s event methods:
// Start(), Update(), FixedUpdate(), OnCollisionEnter2D(), etc.
{
    // Movement parameters
    // Public --> can be modified in the inspector
    // Private --> can only be modified in the script
    public float speed = 40;
    public float maxSpeed = 50;
    public float upSpeed = 20;

    // State Tracking
    private bool onGroundState = true;
    private bool faceRightState = true;
    private bool moving = false;
    private bool jumpedState = false;

    [System.NonSerialized] // do not show in inspector
    public bool alive = true;

    // Components
    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public MarioActions marioActions;

    // UI Objects
    public Transform gameCamera;
    public GameObject gameOverCanvas;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI scoreText;

    // Other parameters
    public AudioClip marioDeath;
    public float deathImpulse = 15;
    public GameObject enemies; // to reset Goombas
    public JumpOverGoomba jumpOverGoomba; // to reset score
    public PlayerInput playerInput; // to reset input actions
    private InputAction jumpHoldAction;
    private InputAction moveAction;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;

        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);

        // Uncomment below to directly call input actions from this script (for send messages behavior)
        marioActions = new MarioActions();
        marioActions.gameplay.Enable();
        // marioActions.gameplay.jump.performed += OnJump;
        marioActions.gameplay.jumphold.performed += OnJumpHold;
        // marioActions.gameplay.move.started += OnMove;
        // marioActions.gameplay.move.canceled += OnMove;
    }

    // ------------------------- //
    // ------- Updates --------  //
    // ------------------------- //
    // Update is called once per frame
    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    // FixedUpdate may be called once per frame.
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    // ------------------------- //
    // ----- Lab3 Stuffs ------  //
    // ------------------------- //
    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.5f)
                marioAnimator.SetTrigger("onSkid");
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.5f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    public void OnJump(InputValue value)
    {
        Debug.Log("OnJump called!");
        // Uncomment below to directly call Jump from here (for send messages behavior)
        Jump();
    }

    public void OnJumpHold(InputAction.CallbackContext context)
    {
        Debug.Log($"OnJumpHold performed with value {context.ReadValue<float>()}");
        // Uncomment below to directly call JumpHold from here (for send messages behavior)
        JumpHold();
    }

    public void OnMove(InputValue value)
    {
        float moveInput = value.Get<float>();
        Debug.Log("OnMove called with value: " + moveInput);

        MoveCheck(moveInput > 0 ? 1 : (moveInput < 0 ? -1 : 0));
    }

    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
            moving = false;
        else
        {
            moving = true;
            FlipMarioSprite(value);
            Move(value);
        }
    }

    public void Jump()
    {
        if (alive && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            marioAnimator.SetBool("onGround", onGroundState);
            Debug.Log($"jumped state :{jumpedState}");
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            Debug.Log("JumpHold applied");
            // marioBody.AddForce(Vector2.up * upSpeed * 0.2f, ForceMode2D.Force);

            // used Impulse instead of Force to make the jump more responsive and obvious lmao
            marioBody.AddForce(Vector2.up * upSpeed * 0.7f, ForceMode2D.Impulse);
            jumpedState = false;
        }
    }

    // Mouse
    public void OnClick(InputValue value)
    {
        bool pressed = value.isPressed;
        if (pressed)
            Debug.Log("Mouse down (pressed)");
        else
            Debug.Log("Mouse up (released)");
    }

    public void OnPoint(InputValue value)
    {
        Vector2 point = value.Get<Vector2>();
        if (point != Vector2.zero) // optional filter
            Debug.Log($"Mouse position: {point}");
    }

    // Other Functions
    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }

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

    // Triggeres when Mario collides with another object
    void OnCollisionEnter2D(Collision2D col)
    {
        if (
            (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Obstacles"))
            && !onGroundState
        )
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }

        if (col.gameObject.CompareTag("Enemy") && alive)
        {
            // Debug.Log("Player collided with enemy!");

            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;

            // Time.timeScale = 0.0f;
            // gameOverCanvas.SetActive(true);
            // finalScoreText.text = scoreText.text;
        }
    }

    public void RestartButtonCallback(int input)
    {
        // Debug.Log("Restart!");
        // reset everything
        ResetGame();
        gameOverCanvas.SetActive(false);
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        // transform.position = new Vector2(-3.259f, -2.45551f, -0.03384f);
        // hardcoded position for Mario
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


// Just some notes for self
// Player Input Component ----- Behavior ---- Send Messages vss Invoke Unity Events
// in Send Messages :
//      - Unity looks for methods with specific names in the same GameObject
//      - when input actio is triggered, it calls the method with the same name
//      - method name has to match exactly
//      - method has to be public
//      - method has to be void

// in Invoke Unity Events :
//      - Unity calls the Action Manager
//      - Action Manager has public Unity Events
//      - Unity Events can be linked to any public method in any GameObject
