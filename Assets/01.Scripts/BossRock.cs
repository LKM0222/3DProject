using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isshoot;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }
    // Start is called before the first frame update
    void Start()
    {
    }


    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isshoot = true;
    }

    IEnumerator GainPower()
    {
        while(!isshoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;

            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);

            yield return null;

        }
    }
}
