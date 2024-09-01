using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPanel : MonoBehaviour
{

    bool isShown = false;
    [SerializeField]
    GameObject itemsPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggleIsShown();
        }
    }

    void toggleIsShown()
    {
        isShown = !isShown;
        itemsPanel.SetActive(isShown);
    }
}
