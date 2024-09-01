using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

    int currentHealth;
    int maxHealth;

    [SerializeField]
    Transform healthBarFill;

    Slider fillSlider;

    [SerializeField]
    TMP_Text healthText;

    // Start is called before the first frame update
    void Start()
    {
    }
    private void Awake()
    {
        fillSlider = gameObject.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateHealth(int updateCurrentHealth, int updateMaxHealth) 
    {
        currentHealth = updateCurrentHealth;
        maxHealth = updateMaxHealth;

        fillSlider.maxValue = maxHealth;
        fillSlider.value = currentHealth;

        healthText.text = currentHealth + " / " + maxHealth + " HP";
    }
}
