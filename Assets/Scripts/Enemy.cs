using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVect = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVect));
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVect = transform.position - other.transform.position;
            Destroy(other.gameObject);
            
            StartCoroutine(OnDamage(reactVect));
        }
    }

    IEnumerator OnDamage(Vector3 reactVect)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        if(curHealth > 0) 
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12;

            reactVect = reactVect.normalized;
            reactVect += Vector3.up;

            rigid.AddForce(reactVect * 5, ForceMode.Impulse);

            Destroy(gameObject, 4f);
        }
    }
}
