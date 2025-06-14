using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    NavMeshAgent nav;
    Animator anim;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2f);
    }

    void Update()
    {   
        if(isChase)
        nav.SetDestination(target.position);
    }

    void FixedUpdate() {
        FreezeVelocity();
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

    void FreezeVelocity()
    {   
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero; //물리회전속도
        }
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
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
            isChase = false;
            nav.enabled = false; // 꺼 줘야 Die모션이 활성화됨.
            anim.SetTrigger("doDie");

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
