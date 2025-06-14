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

            StartCoroutine(OnDamage(reactVect, false));
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVect = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVect, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVect = transform.position - explosionPos;

        StartCoroutine(OnDamage(reactVect, true));
    }

    IEnumerator OnDamage(Vector3 reactVect, bool isGrenade)
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

            if(isGrenade)
            {
                reactVect = reactVect.normalized;
                reactVect += Vector3.up * 3f;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVect * 5, ForceMode.Impulse); //피격
                rigid.AddTorque(reactVect * 15, ForceMode.Impulse); //회전
            }
            else
            {
                reactVect = reactVect.normalized;
                reactVect += Vector3.up;
                rigid.AddForce(reactVect * 5, ForceMode.Impulse); //피격
            }


            Destroy(gameObject, 4f);
        }
    }
}
