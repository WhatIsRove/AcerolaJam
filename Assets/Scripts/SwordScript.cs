using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public float knockbackForce;
    public float finalHitMultiplier;
    public int swordDamage;

    PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            var force = knockbackForce;
            if (player.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_Slash_3")) force = knockbackForce * finalHitMultiplier;
            other.attachedRigidbody.AddForce(player.transform.forward * force, ForceMode.Impulse);
            if (player.spiritMode) player.Heal();
            other.GetComponent<EnemyController>().TakeDamage(swordDamage);
        }
    }
}
