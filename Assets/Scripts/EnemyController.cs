using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent nav;
    public float stopRange;
    public float attackRange;

    Transform playerTransform;

    public float maxHealth = 30;
    float currentHealth;

    public Animator anim;

    public LookAtConstraint lookAt;
    ConstraintSource source;
    

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        playerTransform = FindObjectOfType<PlayerController>().groundPoint;

        currentHealth = maxHealth;
        source.sourceTransform = GameObject.FindGameObjectWithTag("Head").transform;
        source.weight = 1f;
        lookAt.SetSource(0,source);
    }

    void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) > stopRange)
        {
            nav.destination = playerTransform.position;
        }
        if (Vector3.Distance(playerTransform.position, transform.position) < attackRange)
        {
            anim.SetBool("Attack", true);
        }
        else anim.SetBool("Attack", false);

    }

    public void TakeDamage(int i)
    {
        currentHealth -= i;
        anim.SetTrigger("Hit");
        AudioManager.Instance.Play("SwordHit0");

        FindObjectOfType<PlayerController>().CallHitStop();

        FindObjectOfType<PlayerController>().IncreaseSpirit(10);
        if (currentHealth <= 0)
        {
            AudioManager.Instance.Play("Splatter0");
            FindObjectOfType<EnemySpawner>().ReduceEnemyCount();
            Destroy(gameObject);
        }
    }
}
