using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLogic : MonoBehaviour
{
    [Header("Movement")]
    //We set the move speed of the missile.
    public float moveSpeed = 20.0f;
    //We create a RigidBody2D variable to hold our rigidbody.
    [SerializeField]
    private Rigidbody2D rb2D;
    //We create a variable to hold our field of impact.
    [SerializeField]
    private float fieldOfImpact = 0.0f;
    //We create a variable to hold our layermask to tell it what layers to effect.
    public LayerMask layerToHit;

    [Header("Explosions!!!")]
    //We create a variable to hold the damage value of enemy missiles.
    [SerializeField]
    private float damage = 20.0f;
    //We create a variable to hold our explosion effect.
    [SerializeField]
    private GameObject explosionEffect;
    //We create a variable to hold the circle collider for the innerbase
    private CircleCollider2D innerBaseCircleCollider;
    //We create a float to hold our damage to the player
    private float dmgToPlayer;
    //We create a bool to limit our damage output
    private bool dmgLimiter;
    //We create a float to carry our damage multiplier
    private float dmgMultiplier;

    [Header("UI")]
    //Here we create a bool to store a value to determine whether the missile fieldOfImpact gizmo is on.
    [SerializeField]
    private bool isGizmoOn = true;


    // Start is called before the first frame update
    void Start()
    {
        rb2D.velocity = transform.right * moveSpeed;

        innerBaseCircleCollider = GameObject.Find("innerbase").GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        this.transform.rotation = Quaternion.FromToRotation(Vector2.right, rb2D.velocity);
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        //We test whether the gameObjects collided with have the requisite tags to trigger a reaction. If true, we call the explosion script.
        if (obj.tag == "PlayerBase" || obj.tag == "Ground")
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        //We collect all the collider2D objects in a list based on whether they were in the field of impact and had the correct layer to be effected
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, fieldOfImpact, layerToHit);

        //We iterate through the list of colliders affected and calculate a dmg multiplier then apply dmg as necessary
        foreach (Collider2D obj in objects)
        {
            float distanceFromObj = Vector3.Distance(obj.transform.position, this.transform.position);

            float dmgMultiplier = 1 / distanceFromObj;

            dmgToPlayer = damage * dmgMultiplier;

            if (dmgToPlayer > 20)
            {
                dmgToPlayer = 20;
            }
           
            if (obj.gameObject.GetComponent<ObjectHealth>() != null)
            {
                obj.gameObject.GetComponent<ObjectHealth>().TakeDamage(dmgToPlayer);
            } else
            {
                Debug.Log($"ObjectHealth script not found for {obj}.");
            }
        }

        if (innerBaseCircleCollider.OverlapPoint(transform.position) == true) 
            {
                dmgToPlayer = damage * 2;
                GameObject.Find("PlayerBase").GetComponent<PlayerBase>().InnerBaseDmg(dmgToPlayer);
            }

        //Here we instantiate an explosion effect, have it last for half a second, then destroy both the explosion and the missile game objects
        GameObject ExplosionEffectIns = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(ExplosionEffectIns, .5f);
        Destroy(gameObject);

    }

    //We use this function to visually track the field of impact from these missiles
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        if (isGizmoOn)
        {
            Gizmos.DrawWireSphere(transform.position, fieldOfImpact);
        }
    }
}
