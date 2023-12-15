using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHandColl : MonoBehaviour
{
    private void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            GameManager.instance.hp--;
            GameManager.instance.playerHpBar.GetComponent<EnergyBar>().SetValueCurrent(GameManager.instance.hp);
        }
    }
}
