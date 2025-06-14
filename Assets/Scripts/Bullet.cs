using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor") //탄피
        {
            Destroy(gameObject, 3f);
        }
        else if(collision.gameObject.tag == "Wall") //총알
        {
            Destroy(gameObject);
        }
    }
}
