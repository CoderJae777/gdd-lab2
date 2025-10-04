// ---- OLD SCRIPT ---- //

// This is working up til lab 2
// Duplicated because of major changes in Lab 3

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine.InputSystem;

// public class PlayerMovement : MonoBehaviour
// // MonoBehaviour lets the script be attached to a GameObject
// // It’s a special Unity base class that gives you access to Unity’s event methods:
// // Start(), Update(), FixedUpdate(), OnCollisionEnter2D(), etc.
// {
//     // Movement parameters
//     // Public --> can be modified in the inspector
//     // Private --> can only be modified in the script
//     public float speed = 40;
//     public float maxSpeed = 50;
//     public float upSpeed = 20;

//     // State Tracking
//     private bool onGroundState = true;
//     private bool faceRightState = true;
//     [System.NonSerialized] // do not show in inspector
//     public bool alive = true;

//     // Components
//     private Rigidbody2D marioBody;
//     private SpriteRenderer marioSprite;
//     public Animator marioAnimator;
//     public AudioSource marioAudio;
//     public MarioActions marioActions;

//     // UI Objects
//     public Transform gameCamera;
//     public GameObject gameOverCanvas;
//     public TextMeshProUGUI finalScoreText;
//     public TextMeshProUGUI scoreText;

//     // Other parameters
//     public AudioClip marioDeath;
//     public float deathImpulse = 15;
//     public GameObject enemies; // to reset Goombas
//     public JumpOverGoomba jumpOverGoomba; // to reset score
//     public PlayerInput playerInput; // to reset input actions
//     private InputAction jumpHoldAction;
//     private InputAction moveAction;

//     // Start is called before the first frame update
//     void Start()
//     {
//         // Set to be 30 FPS
//         Application.targetFrameRate = 30;
//         marioBody = GetComponent<Rigidbody2D>();
//         marioSprite = GetComponent<SpriteRenderer>();
//         marioAnimator.SetBool("onGround", onGroundState);

//         marioActions = new MarioActions();
//         marioActions.gameplay.Enable();
//         marioActions.gameplay.jump.performed += onJump;
//         marioActions.gameplay.jumphold.performed += OnJumpHoldPerformed;
//         marioActions.gameplay.move.started += OnMove;
//         marioActions.gameplay.move.canceled += OnMove;
//     }

//     // Update is called once per frame
//      void Update()
//     {
//         // toggle state
//         if (Input.GetKeyDown("a") && faceRightState)
//         {
//             faceRightState = false;
//             marioSprite.flipX = true;
//             if (marioBody.linearVelocity.x > 0.1f)
//                 marioAnimator.SetTrigger("onSkid");
//         }

//         if (Input.GetKeyDown("d") && !faceRightState)
//         {
//             faceRightState = true;
//             marioSprite.flipX = false;
//             if (marioBody.linearVelocity.x < -0.1f)
//                 marioAnimator.SetTrigger("onSkid");
//         }

//         marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
//     }

//     // Functions
//     public void onJump(InputAction.CallbackContext context)
//     {
//         Debug.Log("OnJump called!");
//     }

//     public void OnJumpHoldPerformed(InputAction.CallbackContext context)
//     {
//         Debug.Log($"OnJumpHold performedwith value {context.ReadValue<float>()}");
//     }

//     public void OnMove(InputAction.CallbackContext context)
//     {
//         Debug.Log("OnMove called!");
//         if (context.canceled)
//             Debug.Log("Move released");
//         else if (context.started)
//             Debug.Log($"OnMove started with value {context.ReadValue<float>()}");
//     }

//     void PlayJumpSound()
//     {
//         marioAudio.PlayOneShot(marioAudio.clip);
//     }

//     public void PlayDeathImpulse()
//     {
//         marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
//     }

//     public void GameOverScene()
//     {
//         // stop time
//         Time.timeScale = 0.0f;
//         // set gameover scene
//         gameOverCanvas.SetActive(true);
//         finalScoreText.text = scoreText.text;
//     }

//     // Triggeres when Mario collides with another object
//     void OnCollisionEnter2D(Collision2D col)
//     {

//         if ((col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Obstacles")) && !onGroundState)
//         {
//             onGroundState = true;
//             // update animator state
//             marioAnimator.SetBool("onGround", onGroundState);
//         }

//         if (col.gameObject.CompareTag("Enemy") && alive)
//         {
//             // Debug.Log("Player collided with enemy!");

//             marioAnimator.Play("mario-die");
//             marioAudio.PlayOneShot(marioDeath);
//             alive = false;

//             // Time.timeScale = 0.0f;
//             // gameOverCanvas.SetActive(true);
//             // finalScoreText.text = scoreText.text;
//         }
//     }

//     // FixedUpdate may be called once per frame.
//     void FixedUpdate()
//     {
//         float moveHorizontal = Input.GetAxisRaw("Horizontal");

//         if (Mathf.Abs(moveHorizontal) > 0)
//         {
//             Vector2 movement = new Vector2(moveHorizontal, 0);
//             // check if it doesn't go beyond maxSpeed
//             if (marioBody.linearVelocity.magnitude < maxSpeed)
//                 marioBody.AddForce(movement * speed);
//         }

//         // stop
//         if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
//         {
//             // stop
//             marioBody.linearVelocity = Vector2.zero;
//         }
//         if (Input.GetKeyDown("space") && onGroundState)
//         {
//             marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
//             onGroundState = false;
//             // Debug.Log("Jump!");
//         }
//         marioAnimator.SetBool("onGround", onGroundState);
//     }

//     public void RestartButtonCallback(int input)
//     {
//         // Debug.Log("Restart!");
//         // reset everything
//         ResetGame();
//         gameOverCanvas.SetActive(false);
//         // resume time
//         Time.timeScale = 1.0f;
//     }

//     private void ResetGame()
//     {
//         // reset position
//         // transform.position = new Vector3(-3.259f, -2.45551f, -0.03384f);
//         // hardcoded position for Mario
//         marioBody.transform.position = new Vector3(-3.259f, -2.45551f, -0.03384f);
//         // reset sprite direction
//         faceRightState = true;
//         marioSprite.flipX = false;
//         // reset score
//         scoreText.text = "Score: 0";

//         jumpOverGoomba.ResetScore();
//         // reset Goomba
//         foreach (Transform eachChild in enemies.transform)
//         {
//             eachChild.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
//         }

//         // reset question boxes
//         foreach (QuestionBox qb in FindObjectsByType<QuestionBox>(FindObjectsSortMode.None))
//         {
//             qb.ResetBox();
//         }

//         // reset bricks
//         foreach (Brick b in FindObjectsByType<Brick>(FindObjectsSortMode.None))
//         {
//             b.ResetBrick();
//         }
//         marioAnimator.SetTrigger("gameRestart");
//         alive = true;

//         gameCamera.position = new Vector3(0, 0, -10);

//     }
// }
