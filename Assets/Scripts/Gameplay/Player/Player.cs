using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Ensure the component is present on the gameobject the script is attached to
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{

    // Local rigidbody variable to hold a reference to the attached Rigidbody2D component
    new Rigidbody2D rigidbody2D;

    [SerializeField]
    private float movementSpeed = 5;
    Vector2 targetVelocity;// = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameManager gameManager;

    private float maxMovementX = 0.0f;
    private float maxMovementY = 0.0f;

    private int maxHealth = 10;
    private int currentHealth = 10;

    [SerializeField]
    HealthBar healthbar;

    float immunityWindow = 0.2f;
    float lastDamaged = 0;

    private bool paused = true;

    [SerializeField]
    Weapon weapon;
    [SerializeField]
    Weapon weaponToSwap; // temp

    /* TODO:
     * add death for player VERY BASIC DONE
     * tweak difficulty
     * make a vertical slice eg. add some assets etc (black and red only, necro / blood mage style) : too hard
     * review current system
     * decide on next steps
     */

    void Awake()
    {
        // Setup Rigidbody for frictionless top down movement and dynamic collision
        rigidbody2D = GetComponent<Rigidbody2D>();

        rigidbody2D.isKinematic = false;
        rigidbody2D.angularDrag = 0.0f;
        rigidbody2D.gravityScale = 0.0f;

        maxMovementX = (background.transform.localScale.x / 2) - (transform.localScale.x / 2);
        maxMovementY = (background.transform.localScale.y / 2) - (transform.localScale.y / 2);

        healthbar.updateHealth(maxHealth, maxHealth);
    }

    private void FixedUpdate()
    {
        //preformVarUpdates();

        gameplayUpdate();

        //preformChecks();
    }

    void Update()
    {
        preformVarUpdates();

        handleUserInput();
        //gameplayUpdate();

        preformChecks();
    }

    void preformVarUpdates()
    {
        updatePausedState();
    }

    void preformChecks()
    {
        //checkPlayerPositionToMaximums();
    }

    void gameplayUpdate() 
    {
        if (paused) { return; }

        if(Input.GetKeyDown(KeyCode.E)) // bit finicy
        {
            swapWeapon();
        }

        move(targetVelocity);
        aim(weapon);
        fireWeapon(weapon);
    }
    void handleUserInput()
    {
        if (paused) { return; }
        targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    void move(Vector2 targetVelocity)
    {
        // Set rigidbody velocity
        // Multiply the target by deltaTime to make movement speed consistent across different framerates
        float moveX = targetVelocity.x * (movementSpeed * 1000.0f);
        float moveY = targetVelocity.y * (movementSpeed * 1000.0f);

        targetVelocity.x = moveX;
        targetVelocity.y = moveY;

        rigidbody2D.velocity = targetVelocity * Time.fixedDeltaTime;
    }

    public float getMovespeed() 
    {
        return movementSpeed;
    }

    void checkPlayerPositionToMaximums()
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
    void updatePausedState()
    {
        paused = gameManager.paused;
        if (paused)
        {
            rigidbody2D.velocity = new Vector2(0, 0);
        }
    }

    public void takeDamage(int damage) 
    {
        if ((Time.fixedTime - lastDamaged) < immunityWindow)
        {
            return;
        }

        lastDamaged = Time.fixedTime;

        updateHealth(currentHealth - damage, maxHealth);

        if (currentHealth <= 0) 
        {
            die();
        }
    }

    void die()
    {
        updateHealth(0, maxHealth);
        //SceneManager.LoadScene("HomeScreen_Scene"); //removed for testing
    }

    public void updateHealth(int updateCurrentHealth, int updateMaxHealth) 
    {
        currentHealth = Mathf.Max(updateCurrentHealth, 0);
        maxHealth = Mathf.Max(updateMaxHealth, 0);

        healthbar.updateHealth(currentHealth, maxHealth);
    }

    void aim(Weapon weapon) 
    {
        Enemy closestEnemy = getClosestEnemy(transform.position);
        if (closestEnemy == null) { return; }

        Vector3 target = closestEnemy.transform.position;
        target.z = 0f;

        Vector3 objectPos = transform.position;
        target.x = target.x - objectPos.x;
        target.y = target.y - objectPos.y;

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    Enemy getClosestEnemy(Vector3 fromPoint)
    {
        if (gameManager.getEnemies().Count == 0) { return null; }

        List<Enemy> enimies = gameManager.getEnemies();

        float lowestDist = Mathf.Abs(Vector3.Distance(fromPoint, enimies[0].transform.position));
        int closestEnemy = 0;

        for (int i = 0; i < enimies.Count; i++) 
        {
            float dist = Mathf.Abs(Vector3.Distance(fromPoint, enimies[i].transform.position));
            if (dist < lowestDist && enimies[i].getIsSpawned())
            {
                lowestDist = dist;
                closestEnemy = i;
            }
        }

        return (gameManager.getEnemies()[closestEnemy].getIsSpawned()) ? gameManager.getEnemies()[closestEnemy] : null;
    }

    void fireWeapon(Weapon weapon) 
    {
        Enemy closestEnemy = getClosestEnemy(transform.position);

        if (closestEnemy != null) {
            //if (weapon.fire(weapon.gameObject))
            if (weapon.cast(closestEnemy.gameObject))
            {
                Debug.Log("Bang!");

            }
        }
    }

    void swapWeapon()
    {
        /* TODO: Figure out how to choice which weapon to swap
         *       Have inventory?
         *       Like noita (pause game show two weapons to swap out)
         *       Hover over weapon shows stats and based on proximity pressing E will swap weapon
         */

        Weapon weaponToSwapTemp = weaponToSwap;
        Vector3 weaponToSwapPositionTemp = weaponToSwap.gameObject.transform.position;
        Transform weaponToSwapParentTransformTemp = weaponToSwap.gameObject.transform.parent;

        weaponToSwap.gameObject.transform.position = weapon.gameObject.transform.position;
        weaponToSwap.gameObject.transform.SetParent(weapon.gameObject.transform.parent);

        weapon.gameObject.transform.position = weaponToSwapPositionTemp;
        weapon.gameObject.transform.SetParent(weaponToSwapParentTransformTemp);

        weaponToSwap = weapon;
        weapon = weaponToSwapTemp;


    }

    public Vector3 getPlayerPosition()
    {
        return transform.position;
    }

}
