using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject left_muzzleEffect;
    public GameObject right_muzzleEffect;
    public GameObject bloodEffect;
    public GameObject deathEffect;
    public GameObject crawlbloodEffect;
    public GameObject crawldeathEffect;
    public GameObject groundCollEffect;

    public static EffectManager instance;   //싱글톤

    private void Awake()
    {
        instance = this;    
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
