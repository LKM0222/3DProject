using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] GameObject enemyAPref;
    [SerializeField] Transform enemyAGroup;
    [SerializeField] GameObject enemyBPref;
    [SerializeField] Transform enemyBGroup;
    [SerializeField] GameObject enemyCPref;
    [SerializeField] Transform enemyCGroup;
    [SerializeField] GameObject enemyDPref;
    [SerializeField] Transform enemyDGroup;

    public Queue<GameObject> enemyAQueue = new Queue<GameObject>();
    public Queue<GameObject> enemyBQueue = new Queue<GameObject>();
    public Queue<GameObject> enemyCQueue = new Queue<GameObject>();
    public Queue<GameObject> enemyDQueue = new Queue<GameObject>();

    void Start()
    {
        for(int i = 0; i< 40; i++)
        {
            GameObject go = Instantiate(enemyAPref, enemyAGroup);
            go.SetActive(false);
            enemyAQueue.Enqueue(go);
        }
        for(int i = 0; i< 40; i++)
        {
            GameObject go = Instantiate(enemyBPref, enemyBGroup);
            go.SetActive(false);
            enemyBQueue.Enqueue(go);
        }
        for(int i = 0; i< 40; i++)
        {
            GameObject go = Instantiate(enemyCPref, enemyCGroup);
            go.SetActive(false);
            enemyCQueue.Enqueue(go);
        }
        for(int i = 0; i< 2; i++)
        {
            GameObject go = Instantiate(enemyDPref, enemyDGroup);
            go.SetActive(false);
            enemyDQueue.Enqueue(go);
        }
    }

    public void InsertQueue(GameObject _e)
    {   
        Enemy enemy = _e.GetComponent<Enemy>();
        
        switch(enemy.enemyType)
        {
            case Enemy.Type.A: 
                enemyAQueue.Enqueue(_e);
                _e.SetActive(false);
            break;

            case Enemy.Type.B: 
                enemyBQueue.Enqueue(_e);
                _e.SetActive(false);
            break;

            case Enemy.Type.C: 
                enemyCQueue.Enqueue(_e);
                _e.SetActive(false);
            break;
            
            default: 
                enemyDQueue.Enqueue(_e);
                _e.SetActive(false);
            break;
        }
        enemy.isChase = false;
    }

    public GameObject GetQueue(int index)
    {   
        GameObject resultObj;

        switch(index)
        {
            case 0:
                resultObj = enemyAQueue.Dequeue();
                resultObj.SetActive(true);
            break;

            case 1:
                resultObj = enemyBQueue.Dequeue();
                resultObj.SetActive(true);
            break;

            case 2:
                resultObj = enemyCQueue.Dequeue();
                resultObj.SetActive(true);
            break;

            default:
                resultObj = enemyDQueue.Dequeue();
                resultObj.SetActive(true);
            break;
        }

        return resultObj;
    }
}
