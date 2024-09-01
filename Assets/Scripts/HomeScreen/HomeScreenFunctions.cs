using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
    {
        SceneManager.LoadScene("GameScreen_Scene");
    }
    public void startTestZone()
    {
        SceneManager.LoadScene("GameScreen_Scene_TEST_ZONE");
    }

    public void quitGame() 
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Quit");
    }
}
