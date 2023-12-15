using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileN : MonoBehaviour
{
    float speed = 30.0f;    //미사일 속도

    void Start()
    {
        Destroy(this.gameObject, 5.0f);
    }

    void Update()
    {
        MissileMove();
    }
    
    void MissileMove()
    {
        float move = speed * Time.smoothDeltaTime;

        this.transform.Translate(Vector3.forward * move);
    }

    private void OnCollisionEnter(Collision coll)
    {
        int collisionLayer = coll.gameObject.layer;

        Vector3 rndRot = Vector3.right * Random.Range(200, 300);

        Destroy(this.gameObject);
        if(collisionLayer == LayerMask.NameToLayer("Ground"))
        {
            GameObject obj = Instantiate(EffectManager.instance.groundCollEffect,this.transform.position, this.transform.rotation);
            obj.transform.localRotation = Quaternion.Euler(rndRot);
        }
    }
}
