using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBase : MonoBehaviour
{
    [Header("Energy Systems")]
    //We create a list to hold the generator game objects
    private List<Transform> _generators;
    //We create a variable to store our Power Level value
    public GameObject pwrLvlText;
    //We create a variable to store our generator count
    private int generatorCount;
    //We create two variables to hold our initial generators
    public Transform generator;
    public Transform generator1;
    //We create two floats to hold our time and energy tick variables and one int to hold the power level
    public float energyTime = 0.0f;
    public float energyTick = 0.0f;
    public int pwrLvl = 0;

    [Header("PBase Health System")]
    //We create a list to hold our object health values
    private List<float> _pBaseObjectHealth;
    //We create a list to grab all the child gameobjects
    private List<GameObject> _pBaseChildWithObjectHealth;
    //We create a variable to hold our pBase Health value
    public float pBaseHealth = 0.0f;
    //We create a variable to hold our inner base health
    public float innerBaseHealth = 1000.0f;
    //Bool to check whether we've pulled our object health yet
    private bool objectHealthPull = false;
    //We create two variables to hold our healthsystem script and our healthbar
    [SerializeField]
    private HealthSystem healthbar;
    [SerializeField]
    private GameObject hBarGameobject;

    void Awake()
    {
        //We create the _generators list
        _generators = new List<Transform>();
        //And add in the two generator gameObjects
        _generators.Add(generator);
        _generators.Add(generator1);

        _pBaseChildWithObjectHealth = new List<GameObject>();
        _pBaseObjectHealth = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        pwrLvlText.GetComponent<TextMeshProUGUI>().text = pwrLvl.ToString();

        //We check whether PullObjectHealth has been run and if not we run it, set our health bar to the pBaseHealth value and turn objectHealthPull to true
        if (objectHealthPull == false)
        {
            PullObjectHealth();
            healthbar.SetMaxHealth(pBaseHealth);
            objectHealthPull = true;
        }
    }

    private void FixedUpdate()
    {
        energyTime += Time.fixedDeltaTime;
        
        //We create a while loop to continually produce energy whenever the generators are alive
        if (_generators.Count > 0)
        {
            energyTick = (energyTime * (50 * _generators.Count));
            int energyTickInt = (int)energyTick;
            pwrLvl = energyTickInt;
        }

        //We consistently update our healthbar with the pBaseHealth to account for damage taken
        healthbar.SetHealth(pBaseHealth);
    }

    void PullObjectHealth()
    {
        //Grab all children and only keep the ones with PlayerBase tags
        foreach (Transform child in transform)
        {
            if (child.tag == "PlayerBase")
            {
                _pBaseChildWithObjectHealth.Add(child.gameObject);
            }
        }
        
        foreach (GameObject child in _pBaseChildWithObjectHealth)
        {
            _pBaseObjectHealth.Add(child.GetComponent<ObjectHealth>().currentHealth);
        }

        foreach (float health in _pBaseObjectHealth)
        {
            pBaseHealth = pBaseHealth + health;
        }

        pBaseHealth = pBaseHealth + innerBaseHealth;
    }

    public void InnerBaseDmg(float damage)
    {
        if (innerBaseHealth - damage > 0)
        {
            innerBaseHealth -= damage;
            pBaseHealth -= damage;
        } else if (innerBaseHealth - damage <= 0)
        {
            innerBaseHealth = 0;
            pBaseHealth -= damage;
        }

        healthbar.SetHealth(pBaseHealth);

        Debug.Log($"This base has taken {damage} damage! Total health remaining: {pBaseHealth}. Inner Base health remaining: {innerBaseHealth}");
    }
}
