using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int hp = 100;    //플레이어 hp
    public int sp = 100;    //플레이어 sp
    public GameObject playerHpBar;  //플레어어 hp바
    public GameObject playerSpBar;  //플레이어 sp바
    public GameObject zombieHpBar;  //좀비 체력바

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerHpBar.GetComponent<EnergyBar>().SetValueCurrent(hp);
        playerSpBar.GetComponent<EnergyBar>().SetValueCurrent(sp);    
    }


    void Update()
    {
        
    }
}
