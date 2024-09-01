using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bonfire : MonoBehaviour
{
    /* Plan:
     * Enimes that die near the bonfire will empower it.
     *  - Enemies will drop a "soul"
     *  - Soul will need to travel to bonfire for empower to happen
     * When bonfire in empowered, light radius will increase and you will get a reward
     * 
     * BONUS IDEAS:
     * more empowered more intense enimies become (harder or more of them)
     * Get to choose a reward? or reward teir based on over empowered or how fast you empower?
     * some kind of risk and reward? empower teirs? (eg. can base empower it then you can keep empowering past base for enhancments? or reroll of reward)
     * 
     */

    [SerializeField]
    private TMPro.TextMeshProUGUI bonfireSoulCountText;
    [SerializeField]
    private Light2D bonfireRadiusLight;

    [SerializeField]
    private GameManager gameManager;
    bool paused = false;

    [SerializeField]
    private float radius = 10.0F;
    private int soulCount = 0;
    private int soulGoal = 25;

    private bool activated = false;
    private bool completed = false;

    //CircleCollider2D bonfireColider;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        preformVarUpdates();
        gameManager.bonfires.Add(this);
        updateBonfireRadius(radius);
    }

    void Start()
    {
        
    }

    //void FixedUpdate()
    //{
    //    preformVarUpdates();
    //}

    void Update()
    {
        preformVarUpdates();
    }

    void preformVarUpdates()
    {
        updatePausedState();
    }

    void updatePausedState()
    {
        paused = gameManager.paused;
    }

    void onPlayerEnterRadius()
    {
        activated = true;
    }

    public float distanceFromEnemy(Enemy enemy)
    {
        return Mathf.Abs(Vector2.Distance(transform.position, enemy.transform.position));
    }

    //TODO make private again
    public void updateSoulCount(int numberOfSouls) 
    {
        //if (!activated || paused) { return; }

        if (numberOfSouls <= 0) { return; }
        soulCount += numberOfSouls;
        bonfireSoulCountText.text = soulCount + "/" + soulGoal;

        if (soulCount >= soulGoal)
        {
            completeBonfire();
        }
    }

    void completeBonfire() 
    {
        if (completed) { return; }
        completed = true;
        updateBonfireRadius(radius * 2.0F);
        Debug.Log("completeBonfire!");
    }

    public int getSoulCount()
    {
        return soulCount;
    }

    public int getSoulGoal()
    {
        return soulGoal;
    }

    public float getBonfireRadius()
    {
        return radius;
    }

    void updateBonfireRadius(float newRadius) 
    {
        radius = newRadius;
        bonfireRadiusLight.pointLightInnerRadius = radius / 2.0F;
        bonfireRadiusLight.pointLightOuterRadius = radius;
    }
}
