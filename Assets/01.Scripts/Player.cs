using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    public Camera followCamera;
    public GameManager manager;

    //sound
    public AudioSource jumpSound;

    public int ammo;
    public int coin;
    public int health;
    public int score;
    public int exp;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;
    public int maxExp;


    float hAxis;
    float vAxis;

    bool wDown; //걷기
    bool jDown; //점프
    bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge; //회피
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;
    bool isShop;
    bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs; //플레이어 오브젝트가 여러개로 이뤄져있기때문에 배열 사용

    [SerializeField] GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDeley;

    [Header("Effect")]
    public float coinMulti = 1f;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        //PlayerPrefs.SetInt("MaxScore", 112500);
        Debug.Log($"{PlayerPrefs.GetInt("MaxScore")}");
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interation();
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void GetInput()
    {
        //Input
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        //Move
        moveVec = new Vector3(hAxis, 0f, vAxis).normalized; //nomalized는 항상 단위백터 1로 보정시켜준다.
        
        if(isDodge) moveVec = dodgeVec;
        if(isSwap || isReload || !isFireReady || isDead) moveVec = Vector3.zero;

        if(!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        //Animation
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); //넣어둔 벡터를 바라보게함.

        // 마우스에 의한 회전
        if(fDown && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit rayHit, 100f))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if(jDown && moveVec == Vector3.zero&& !isJump && !isDodge && !isSwap && !isDead)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            //Sound
            // jumpSound.Play();
        }
    }

    void Grenade()
    {
        if(hasGrenades == 0) return;

        if(gDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit rayHit, 100f))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse); // 던지는 방향
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse); // 회전 

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }

    void Attack()
    {
        if(equipWeapon == null) return;

        fireDeley += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDeley;

        if(fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDeley = 0;
        }
    }

    void Reload()
    {
        if(equipWeapon == null) return;
        if(equipWeapon.type == Weapon.Type.Melee) return;
        if(ammo == 0) return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 3f); // 장전시간
        }
    }
    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop && !isDead)
        {   
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f); // 시간을 두고 실행
        }
    }

    void DodgeOut()
    {
        isDodge = false;
        speed *= 0.5f;
    }

    void Swap()
    {
        if(sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) return;
        if(sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) return;    
        if(sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) return;

        int weaponIndex = -1;
        if(sDown1) weaponIndex = 0;
        if(sDown2) weaponIndex = 1;
        if(sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead)
        {   
            if(equipWeapon != null) equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge && !isShop && !isDead)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; //물리회전속도
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    public void GetEXP(int _exp)
    {
        exp += _exp;
        
        if(exp > maxExp)
        {
            Time.timeScale = 0f;

            exp = maxExp;
            
            manager.effectPanel.SetActive(true);
            manager.effectManager.Initialized();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump",false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Ammo:
                {
                    ammo += item.value;
                    if(ammo > maxAmmo) ammo = maxAmmo;
                }
                break;

                case Item.Type.Coin:
                {
                    coin += (int)(item.value * coinMulti);
                    if(coin > maxCoin) coin = maxCoin;
                }
                break;
                case Item.Type.Heart:
                {
                    health += item.value;
                    if(health > maxHealth) health = maxHealth;
                }
                break;
                case Item.Type.Grenade:
                {
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if(hasGrenades > maxHasGrenades) hasGrenades = maxHasGrenades;
                }
                break;
            }
            
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }
            
            if(other.GetComponent<Rigidbody>() != null) Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = null;
        }
        else if(other.tag == "Shop")
        {   
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            nearObject = null;
            isShop = false;
        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        if(isBossAtk) rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        if(health <= 0 && !isDead) OnDie();

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if(isBossAtk) rigid.velocity = Vector3.zero;
    }
}
