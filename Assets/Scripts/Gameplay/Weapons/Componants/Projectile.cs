using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    new Rigidbody2D rigidbody2D;
    Vector2 targetVelocity;
    Weapon.Attributes attributes;
    Vector2 startingPosition;
    GameObject target;
    public Weapon weapon;

    [SerializeField]
    GameObject aoeCircle;

    private bool paused = true;
    private int pierceCount = 0;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;

        rigidbody2D.isKinematic = false;
        rigidbody2D.angularDrag = 0.0f;
        rigidbody2D.gravityScale = 0.0f;
    }

    private void Start()
    {
        setTargetVelocity(target.transform.position);
    }

    private void FixedUpdate()
    {
        //preformVarUpdates();

        gameplayUpdate();
    }

    private void Update()
    {
        preformVarUpdates();

        gameplayUpdate();
    }

    void preformVarUpdates()
    {
        updatePausedState();
    }

    void gameplayUpdate()
    {
        if (paused) { return; }

        //setTargetVelocity(target.transform.position);
        move(targetVelocity);
        checkedDistanceTraveled();
    }
    void updatePausedState()
    {
        paused = GameObject.Find("GameManager").GetComponent<GameManager>().paused;

        if (paused) { rigidbody2D.velocity = new Vector2(0, 0); }
        
    }

    public void setTargetVelocity(Vector3 targetPosition)
    {
        float targetVelocityX = 0;
        float targetVelocityY = 0;

        Vector3 directionVector = targetPosition - weapon.transform.position;

        float velocityMagnitude = 100.0F / (Mathf.Abs(directionVector.x) + Mathf.Abs(directionVector.y));

        targetVelocityX = velocityMagnitude * directionVector.x;
        targetVelocityY = velocityMagnitude * directionVector.y; 

        targetVelocity = new Vector2(targetVelocityX, targetVelocityY);
    }

    //public void setTargetVelocityHoming(Vector3 targetPosition) //basic homing is called each update 
    //{
    //    float targetVelocityX = 0;
    //    float targetVelocityY = 0;

    //    if (Mathf.Abs(Mathf.Abs(targetPosition.x) - Mathf.Abs(transform.position.x)) >= 0.05f)
    //    {
    //        targetVelocityX = (targetPosition.x < transform.position.x) ? -1 : 1;
    //    }

    //    if (Mathf.Abs(Mathf.Abs(targetPosition.y) - Mathf.Abs(transform.position.y)) >= 0.05f)
    //    {
    //        targetVelocityY = (targetPosition.y < transform.position.y) ? -1 : 1;
    //    }

    //    targetVelocity = new Vector2(targetVelocityX, targetVelocityY);
    //}

    void move(Vector2 targetVelocity) 
    {
        rigidbody2D.velocity = (targetVelocity * (attributes.projectileSpeed * 10.0f)) * Time.deltaTime;
    }

    public void setTarget(GameObject newTarget) 
    {
        target = newTarget;
    }

    public void setAttributes(Weapon.Attributes attributes) 
    {
        this.attributes = attributes;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit collider");
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            Debug.Log("Hit enemy");
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.takeDamage(attributes.damage);            

            onHit();
        }
    }

    private void destoryProjectile() 
    {
        Destroy(gameObject);
    }

    private void checkedDistanceTraveled()
    {
        float distanceTraveled = Mathf.Abs(Vector2.Distance(transform.position, startingPosition));
        if (distanceTraveled >= attributes.range)
        {
            destoryProjectile();
        }
    }

    private bool pierce() 
    {
        if (attributes.canPierce.First && pierceCount < attributes.canPierce.Second -1)
        {
            pierceCount++;
            return true;
        }
        return false;
    }

    private void aoe() 
    {
        if (attributes.hasAOE.First) 
        {
            float aoeRadius = attributes.hasAOE.Second;
            Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, aoeRadius);


            aoeCircle.GetComponent<SpriteRenderer>().enabled = true;
            //aoeCircle.SetActive(true);
            aoeCircle.transform.localScale = new Vector3(aoeRadius, aoeRadius, transform.localScale.z);
            foreach (Collider2D collision in collisions)
            {
                if (collision.gameObject.GetComponent<Enemy>())
                {
                    //Debug.Log("AOE collision");
                    Enemy enemyHit = collision.gameObject.GetComponent<Enemy>();
                    //enemyHit.applyEffect();
                    enemyHit.takeDamage(attributes.damage);
                }
            }
        }
    }

    private void applyEffect() { }

    private void onHit() 
    {

        //apply effect

        //aoe
        aoe();

        //pierce
        if (!pierce())
        {
            destoryProjectile();
        }
    }

}
