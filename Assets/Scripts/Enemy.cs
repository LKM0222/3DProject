using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };

    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    protected Rigidbody rigid;
    protected BoxCollider boxCollider;
    protected MeshRenderer[] meshs;
    protected NavMeshAgent nav;
    protected Animator anim;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D) Invoke("ChaseStart", 2f);
    }

    void Update()
    {   
        if(nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FixedUpdate() 
    {
        Targeting();
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

    void Targeting()
    {   
        if(!isDead && enemyType != Type.D)
        {
            float targetRadius = 0; 
            float targetRange = 0; //적 감지 범위

            switch(enemyType)
            {
                case Type.A:
                {
                    targetRadius = 1.5f;
                    targetRange = 3f;
                }
                break;

                case Type.B:
                {
                    targetRadius = 1f;
                    targetRange = 12f;
                }
                break;
                case Type.C:
                {
                    targetRadius = 0.5f;
                    targetRange = 25f;
                }
                break;
            }
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
            
            if(rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }

    }

    IEnumerator Attack()
    {   
        //먼저 정시를 한 다음, 애니메이션과 함께 공격범위 활성화
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch(enemyType)
        {
            case Type.A:
            {
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
            }
            break;

            case Type.B:
            {
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
            }
            break;

            case Type.C:
            {
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20f;

                yield return new WaitForSeconds(2f);
            }
            break;
        }

        
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    IEnumerator OnDamage(Vector3 reactVect, bool isGrenade)
    {   
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        
        if(curHealth > 0) 
        {
            foreach(MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach(MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }
            gameObject.layer = 12;
            isDead = true;
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

            if(enemyType != Type.D) Destroy(gameObject, 4f);
        }
    }
}
