using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoor : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    //发出子弹
    public GameObject Take()
    {
        // if (pool.Count>0)
        // {
        //     GameObject instanceToReuse = pool.Dequeue();
        //     instanceToReuse.SetActive(true);
        //     return instanceToReuse;
        // }

        return Instantiate(bulletPrefab);
    }

    //回收子弹
    public void Back(GameObject objToPool)
    {
        // pool.Enqueue(objToPool);
        // objToPool.transform.SetParent(transform);
        Destroy(objToPool);
    }
}