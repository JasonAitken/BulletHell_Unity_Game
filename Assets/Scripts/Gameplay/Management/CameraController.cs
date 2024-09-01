using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Ensure the component is present on the gameobject the script is attached to
[RequireComponent(typeof(Rigidbody2D))]
public class CameraController : MonoBehaviour
{
    // Local rigidbody variable to hold a reference to the attached Rigidbody2D component
    new Rigidbody2D rigidbody2D;

    public float movementSpeed = 6;

    [SerializeField]
    private Player player;
    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameManager gameManager;

    private float maxMovementX = 0.0f;
    private float maxMovementY = 0.0f;

    float playerPositonX;
    float playerPositonY;

    Vector2 targetVelocity;

    public bool paused = true;

    void Awake()
    {
        // Setup Rigidbody for frictionless top down movement and dynamic collision
        rigidbody2D = GetComponent<Rigidbody2D>();

        rigidbody2D.isKinematic = false;
        rigidbody2D.angularDrag = 0.0f;
        rigidbody2D.gravityScale = 0.0f;

        maxMovementX = background.transform.localScale.x / 8;
        maxMovementY = background.transform.localScale.y / 8;

    }

    private void Start()
    {
        movementSpeed = player.getMovespeed();
    }

    void FixedUpdate()
    {
        //preformVarUpdates();

        gameplayUpdate();

        //preformChecks();
    }

    void Update()
    {
        preformVarUpdates();

        //gameplayUpdate();
        handleUserInput();

        preformChecks();
    }

    void preformVarUpdates() 
    {
        updatePausedState();
        updateMovespeed();
        updatePlayerPosition();
    }

    void preformChecks()
    {
        //checkCameraPositionToMaximums();
        checkCameraPositionToPlayer();
    }
    void gameplayUpdate()
    {
        if (paused) { return; }

        //handleUserInput();
        Move(targetVelocity);
    }

    void Move(Vector2 targetVelocity)
    {
        // Set rigidbody velocity
        // Multiply the target by deltaTime to make movement speed consistent across different framerates
        rigidbody2D.velocity = (targetVelocity * (movementSpeed * 1000.0f)) * Time.deltaTime; 
    }

    void updateMovespeed() 
    {
        if (movementSpeed != player.getMovespeed()) {
            movementSpeed = player.getMovespeed();
        }
    }

    void updatePlayerPosition()
    {
        playerPositonX = player.transform.position.x;
        playerPositonY = player.transform.position.y;
    }

    void checkCameraPositionToMaximums() 
    {
        if (Mathf.Abs(transform.position.x) > maxMovementX) { 
            transform.position = 
                new Vector3(gameManager.returnSignOf(transform.position.x, maxMovementX), 
                transform.position.y, 
                transform.position.z); 
        }

        if (Mathf.Abs(transform.position.y) > maxMovementY)
        {
            transform.position = 
                new Vector3(transform.position.x,
                gameManager.returnSignOf(transform.position.y, maxMovementY),
                transform.position.z);
        }
    }

    void checkCameraPositionToPlayer() 
    {
        if (Mathf.Abs(Mathf.Abs(playerPositonX) - Mathf.Abs(transform.position.x)) >= 0.2f && 
            transform.position.x != gameManager.returnSignOf(transform.position.x, maxMovementX))
        {
            transform.position = new Vector3(playerPositonX, transform.position.y, transform.position.z);
        }

        if (Mathf.Abs(Mathf.Abs(playerPositonY) - Mathf.Abs(transform.position.y)) >= 0.2f &&
            transform.position.y != gameManager.returnSignOf(transform.position.y, maxMovementY))
        {
            transform.position = new Vector3(transform.position.x, playerPositonY, transform.position.z);
        }
    }

    void updatePausedState() 
    {
        paused = gameManager.paused;

        if (paused)
        {
            targetVelocity = new Vector2(0, 0);
            rigidbody2D.velocity = targetVelocity;
        }
    }

    void handleUserInput()
    {
        if (paused) { return; }
        setTargetVelocity(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void setTargetVelocity(float HorizontalRawAxisInput, float VeriticalRawAxisInput)
    {
        float targetVelocityX = 0;
        float targetVelocityY = 0;

        if ((Mathf.Abs(playerPositonX) - Mathf.Abs(transform.position.x)) <= 0.15f)
        { 
            targetVelocityX = HorizontalRawAxisInput; 
        }

        if ((Mathf.Abs(playerPositonY) - Mathf.Abs(transform.position.y)) <= 0.15f)
        { 
            targetVelocityY = VeriticalRawAxisInput; 
        }

        targetVelocity = new Vector2(targetVelocityX, targetVelocityY).normalized;
    }

}
