using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLines : MonoBehaviour
{
    public int Damage = 1;
    void Start()
    {
        Debug.Log("Attack line spawned: " + gameObject.name);
    }

    void Update()
    {
        if (FindObjectOfType<EnemyBoss>() == null)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log("Hit: " + hitInfo.name);

        if (hitInfo.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(Damage);
        }
    }
}