using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Ensure the component is present on the gameobject the script is attached to
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{

    // Local rigidbody variable to hold a reference to the attached Rigidbody2D component
    new Rigidbody2D rigidbody2D;

    [SerializeField]
    private float movementSpeed = 2;

    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Player player;

    private float maxMovementX = 0.0f;
    private float maxMovementY = 0.0f;

    float playerPositonX;
    float playerPositonY;

    Vector2 targetVelocity;

    [SerializeField]
    float damagingDistance = 0.05f;
    
    public int baseDamage = 2;

    private bool paused = true;

    float immunityWindow = 0.1f;
    float lastDamaged = 0;
    private int maxHealth = 10;
    private int currentHealth = 10;

    [SerializeField]
    float spawnDelay = 1.5F;
    float spawnStartTime = 0.0F;
    private bool spawned = false;
    [SerializeField]
    GameObject spawnIndicatorPrefab;
    GameObject spawnIndicator;

    void Awake()
    {
        // Setup Rigidbody for frictionless top down movement and dynamic collision
        rigidbody2D = GetComponent<Rigidbody2D>();

        rigidbody2D.isKinematic = false;
        rigidbody2D.angularDrag = 0.0f;
        rigidbody2D.gravityScale = 0.0f;

        background = GameObject.Find("Background");
        player = GameObject.Find("Player").GetComponent<Player>();

        maxMovementX = (background.transform.localScale.x / 2) - (transform.localScale.x / 2);
        maxMovementY = (background.transform.localScale.y / 2) - (transform.localScale.y / 2);

    }

    private void Start()
    {        
        spawn();
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

        preformChecks();
    }
    void preformVarUpdates()
    {
        updatePausedState();
        updatePlayerPosition();
    }
      
    void gameplayUpdate()
    {
        if (paused) { return; }

        if (!spawned)
        {
            
            if ((Time.fixedTime - spawnStartTime) >= spawnDelay)
            {
                Debug.Log("spawning");
                spawned = true;
                Destroy(spawnIndicator);
                this.GetComponent<SpriteRenderer>().enabled = true;
                //transform.Find("Enemy_Light_2D").gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("not spawned!");
                return; 
            }
            
        }

        setTargetVelocity();
        Move(targetVelocity);
        checkForPlayer();
    }
    void preformChecks()
    {
        //checkEmemyPositionToMaximums();
    }

    public void setUpEnemy(GameManager globalGameManager) 
    {
        gameManager = globalGameManager;
    }

    void Move(Vector2 targetVelocity)
    {
        // Set rigidbody velocity 
        // Multiply the target by deltaTime to make movement speed consistent across different framerates
        rigidbody2D.velocity = (targetVelocity * (movementSpeed * 10.0f)) * Time.deltaTime;
    }
    void updatePlayerPosition()
    {
        playerPositonX = player.transform.position.x;
        playerPositonY = player.transform.position.y;
    }

    public float getMovespeed()
    {
        return movementSpeed;
    }

    void checkEmemyPositionToMaximums()
    {
        if (Mathf.Abs(transform.position.x) > maxMovementX)
        {
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

    void checkForPlayer() 
    {
        if (Mathf.Abs(playerPositonX - transform.position.x) <= damagingDistance)
        {
            if (Mathf.Abs(playerPositonY - transform.position.y) <= damagingDistance)
            {
                player.takeDamage(calculateDamage());
            }
        }        
    }

    //void setTargetVelocityOriginal()
    //{
    //    float targetVelocityX = 0;
    //    float targetVelocityY = 0;

    //    if (Mathf.Abs(Mathf.Abs(playerPositonX) - Mathf.Abs(transform.position.x)) >= 0.05f)
    //    {
    //        targetVelocityX = (playerPositonX < transform.position.x) ? -1 : 1;
    //    }

    //    if (Mathf.Abs(Mathf.Abs(playerPositonY) - Mathf.Abs(transform.position.y)) >= 0.05f )
    //    {
    //        targetVelocityY = (playerPositonY < transform.position.y) ? -1 : 1;
    //    }

    //    targetVelocity = new Vector2(targetVelocityX, targetVelocityY);
    //}

    public void setTargetVelocity()
    {
        Vector3 targetPosition = new Vector3(playerPositonX, playerPositonY, player.transform.position.z);

        float targetVelocityX = 0;
        float targetVelocityY = 0;

        Vector3 directionVector = targetPosition - transform.position;

        float velocityMagnitude = 100.0F / (Mathf.Abs(directionVector.x) + Mathf.Abs(directionVector.y));

        targetVelocityX = velocityMagnitude * directionVector.x;
        targetVelocityY = velocityMagnitude * directionVector.y;

        targetVelocity = new Vector2(targetVelocityX, targetVelocityY);
    }

    int calculateDamage() 
    {
        return baseDamage;
    }

    void updatePausedState()
    {
        paused = gameManager.paused;

        if (paused)
        {
            targetVelocity = new Vector2(0, 0);
            rigidbody2D.velocity = targetVelocity;
            spawnStartTime = Time.fixedTime - (Time.fixedTime - spawnStartTime);
        }
    }

    public void takeDamage(int damage)
    {
        if (((Time.fixedTime - lastDamaged) < immunityWindow) || !spawned)
        {
            return;
        }
        
        lastDamaged = Time.fixedTime;

        updateHealth(currentHealth - damage, maxHealth);

        if (currentHealth <= 0)
        {
            updateHealth(0, maxHealth);
        }
    }

    void die()
    {
        updateHealth(0, maxHealth);
        Destroy(gameObject);
    }

    public void kill() 
    {
        die();
    }

    public void updateHealth(int updateCurrentHealth, int updateMaxHealth)
    {
        currentHealth = Mathf.Max(updateCurrentHealth, 0);
        maxHealth = Mathf.Max(updateMaxHealth, 0);

        //healthbar.updateHealth(currentHealth, maxHealth);
    }

    void spawn() 
    {
        spawnStartTime = Time.fixedTime;

        spawnIndicator = Instantiate<GameObject>(spawnIndicatorPrefab);
        Vector3 spawnPos = transform.position;
        spawnIndicator.transform.localScale = new Vector3(2.0F, 2.0F, transform.localScale.z);
        spawnIndicator.transform.position = spawnPos;
    }

    //TODO update to return struct with all attributes for game manager to manage rather than on enemy script itself
    public int checkEnemy() 
    {
        return currentHealth;
    }

    public bool getIsSpawned() 
    {
        return spawned;
    }

}
