using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;

    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        // get the starting position
        originalX = transform.position.x;
        // record the spawn/start position so GameRestart can restore it
        startPosition = transform.localPosition;
        ComputeVelocity();
    }

    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }

    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        { // move goomba
            Movegoomba();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
    }

    public void GameRestart()
    {
        gameObject.SetActive(true);

        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();
        // Ensure this component is enabled so movement resumes
        this.enabled = true;

        // restore visual scale (in case squash animation scaled it)
        Transform visual = transform.Find("Visual");
        if (visual == null)
        {
            // fallback: first child with a SpriteRenderer
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                visual = sr.transform;
        }
        if (visual != null)
        {
            visual.localScale = Vector3.one;
            // reset animator state if present
            var anim = visual.GetComponent<Animator>();
            if (anim != null)
            {
                // return animator to its default state
                anim.Rebind();
                anim.Update(0f);
            }
        }

        // re-enable colliders and physics
        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var c in colliders)
            c.enabled = true;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = true;
    }
}
