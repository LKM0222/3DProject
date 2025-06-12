using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Input
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");

        //Move
        moveVec = new Vector3(hAxis, 0f, vAxis).normalized; //nomalized는 항상 단위백터 1로 보정시켜준다.
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        //Animation
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        //Rotation
        transform.LookAt(transform.position + moveVec); //넣어둔 벡터를 바라보게함.
    }
}
