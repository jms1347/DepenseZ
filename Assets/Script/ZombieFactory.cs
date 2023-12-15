using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieFactory : MonoBehaviour
{
    public GameObject zombiePrefab;
    bool isZombieCreate = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(AriseZombie());
    }

    IEnumerator AriseZombie()
    {
        if (!isZombieCreate)
        {
            isZombieCreate = true;
            float x = Random.Range(-35, 35);
            float z = Random.Range(-35, 35);
            Instantiate(zombiePrefab, new Vector3(x, 0, z), Quaternion.identity);
            yield return new WaitForSeconds(3.0f);
            isZombieCreate = false;
        }
        
       
    }
}
