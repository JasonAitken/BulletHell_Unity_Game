using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text pausedText;
    public bool paused = true;

    List<Enemy> enemies = new List<Enemy>();
    public List<Bonfire> bonfires = new List<Bonfire>();
    [SerializeField]
    int maxEnemies = 1;
    [SerializeField]
    GameObject[] enemyPrefabs;

    [SerializeField]
    GameObject currentLocationGameObject;
    [SerializeField]
    Player player;
    [SerializeField]
    Location startLocation;

    LocationGenerator locationGenerator;

    private void Awake()
    {
        locationGenerator = new LocationGenerator(startLocation);
    }

    void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    togglePause();
        //}

        gameplayUpdate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            togglePause();
        }

        //gameplayUpdate();
    }

    void gameplayUpdate()
    {
        if (paused) { return; }

        spawnEnemies();
        killEnemies();
    }

    public float returnSignOf(float signToReturnValue, float valueToReturn)
    {
        if (signToReturnValue < 0 && valueToReturn >= 0 || signToReturnValue >= 0 && valueToReturn < 0)
        {
            return -valueToReturn;
        }
        return valueToReturn;
    }

    void togglePause() {
        paused = !paused;
        pausedText.gameObject.SetActive(paused);
    }

    void spawnEnemies()
    {
        if (enemies.Count < maxEnemies)
        {
            //TODO Prevent enimies spawning within radius of player or add spawn delay and indicator
            GameObject newEnemy = Instantiate<GameObject>(enemyPrefabs[0]);
            newEnemy.GetComponent<Enemy>().setUpEnemy(this);
            float spawnX = (currentLocationGameObject.transform.localScale.x / 2.0f) - (Random.value * currentLocationGameObject.transform.localScale.x);
            float spawnY = (currentLocationGameObject.transform.localScale.y / 2.0f) - (Random.value * currentLocationGameObject.transform.localScale.y);
            Vector3 spawnPos = new Vector3(spawnX, spawnY, newEnemy.transform.position.z);
            spawnPos.x += currentLocationGameObject.transform.position.x;
            spawnPos.y += currentLocationGameObject.transform.position.y;
            newEnemy.transform.position = spawnPos;
            enemies.Add(newEnemy.GetComponent<Enemy>());
        }
    }

    public List<Enemy> getEnemies() 
    {
        return enemies;
    }

    void killEnemies() 
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemyToCheck = enemies[i];
            if (enemyToCheck.checkEnemy() <= 0 && enemyToCheck.getIsSpawned())
            {
                Enemy enemyToKill = enemies[i];

                Bonfire closestBonfire = null;
                float shortestCurrentDistance = 100.0F;
                // Check if enemy is within range of a bonfire, and should drop a soul
                for (int j = 0; j < bonfires.Count; j++)
                {
                    float distanceFromBonfire = bonfires[j].distanceFromEnemy(enemyToKill);
                    if ((distanceFromBonfire < shortestCurrentDistance) && (distanceFromBonfire < bonfires[j].getBonfireRadius()))
                    {
                        closestBonfire = bonfires[j];
                        shortestCurrentDistance = distanceFromBonfire;
                    }
                }

                if (closestBonfire != null) 
                {
                    //enemyToKill.dropSoul(closestBonfire);
                    Debug.Log("drop soul");
                    closestBonfire.updateSoulCount(1);
                }

                enemies.RemoveAt(i);
                enemyToKill.kill();
                i--;
            }
        }
    }

    /* Method for creating a new weapon and placing it in the scene.
     * Returns true if a new weapon has been spawned. TODO: might remove if handled out of method
     */
    bool spawnWeapon()
    {
        if (paused) { return false; }



        return false;
    }

    public Player getPlayer() 
    {
        return player;
    }

    public void setCurrentLocationGameObject(Location newCurrentLocation) 
    {
        currentLocationGameObject = newCurrentLocation.gameObject;
    }
}
