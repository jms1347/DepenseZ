using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
     enum state
    {
        arise = 0,
        idle,       //기본상태
        walk,           //걷기상태
        attack,         //공격상태
        run,            //뛰는상태
        down1,           //다운상태
        down2,           //다운2상태
        death,           //죽음
        crawlidle,          //crawl 좀비
        crawlrun,          //crawl 좀비
        crawlattack,          //crawl 좀비
        crawlhit,          //crawl 좀비
        hit,            //맞는 상태
    }
     state zombieState = state.arise;
    
    float speed = 3.5f;        //이동속도
    float rotSpeed = 3.5f;     //회전속도
    float attackSpeed = 2.0f;  //공격속도
    int hp = 10;                 //좀비 hp

    float eyesight = 10.0f;     //시야(공격 유효 범위)
    float attackRange = 1.2f;   //공격사정거리

    //플레이어(타켓 설정)
    GameObject target;  //좀비의 타켓(플레이어)

    //좀비 애니메이션
    public Animator zombieAni;
    CharacterController zombieController;
    Vector3 orignPos;   //처음 생성위치 저장
    

    void Awake()
    {
        zombieAni = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player");
        zombieController = GetComponent<CharacterController>();
        orignPos = this.transform.position;

        
    }
    void Start()
    {
        Invoke("ZombieIdle", 5.0f);
    }

    //좀비 초기화시키기
    public void ZombieIdle()
    {
        zombieAni.SetBool("Idle", true);
        zombieAni.SetBool("Attack2", false);
        zombieAni.SetBool("Attack1", false);
        zombieAni.SetBool("Run", false);
        zombieAni.SetBool("Down1", false);
        zombieAni.SetBool("StandUp", false);
        zombieState = state.idle;        
    }
    void Update()
    {
        ZombieState();
        Debug.Log("좀비hp : " + hp);
    }

    void ZombieState()
    {
        switch (zombieState)
        {
            case state.idle:
                {
                    float distance = Vector3.Distance(target.transform.position, this.transform.position);
                    if(distance <= eyesight)
                    {
                        zombieAni.SetBool("Run", true);
                        zombieAni.SetBool("Idle", false);
                        zombieState = state.run;                        
                    }
                    else if(distance <= attackRange)
                    {                        
                        int randomNo = Random.Range(0, 2);
                        if (randomNo == 0)
                        {
                            zombieAni.SetBool("Attack1", true);
                            zombieAni.SetBool("Run", false);
                        }
                        else if(randomNo == 1)
                        {
                            zombieAni.SetBool("Attack2", true);
                            zombieAni.SetBool("Run", false);
                        }
                        zombieState = state.attack;
                    }                
                }
                break;          
            case state.run:
                {
                    float distance = Vector3.Distance(target.transform.position, this.transform.position);
                    Vector3 dir = target.transform.position - this.transform.position;
                    dir.y = 0;
                    dir.Normalize();    //거리 평준화 함수
                   
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);  //회전
                    zombieController.SimpleMove(dir * speed);   //이동함수
                    if (distance > eyesight)
                    {                       
                        zombieAni.SetBool("Idle", true);
                        zombieAni.SetBool("Run", false);
                        zombieState = state.idle;
                    }
                    else if(distance <= attackRange)
                    {                      
                        int randomNo = Random.Range(0, 2);
                        if (randomNo == 0)
                        {
                            zombieAni.SetBool("Attack1", true);
                            zombieAni.SetBool("Run", false);
                        }
                        else if (randomNo == 1)
                        {
                            zombieAni.SetBool("Attack2", true);
                            zombieAni.SetBool("Run", false);
                        }
                        zombieState = state.attack;
                    }
                }
                break;
            case state.attack:
                {
                    float distance = Vector3.Distance(target.transform.position, this.transform.position);
                    Vector3 dir = target.transform.position - this.transform.position;
                    dir.y = 0;
                    dir.Normalize();    //거리 평준화 함수

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);  //회전
                    if (distance > attackRange)
                    {                      
                        zombieAni.SetBool("Run", true);
                        zombieAni.SetBool("Attack1", false);
                        zombieAni.SetBool("Attack2", false);
                        zombieState = state.run;
                    }
                    
                }
                break;
            case state.hit:
                {
                    if (hp > 0)
                    {
                        eyesight = 20.0f;
                        zombieState = state.idle;
                    }
                    else if (hp <= 0)
                    {
                        zombieAni.SetBool("Idle", false);
                        zombieAni.ResetTrigger("Hit");
                        int randomNo = Random.Range(2, 3);
                        if (randomNo == 0)   //죽음
                        {
                            zombieState = state.death;                           
                        }
                        else if (randomNo == 1)  //일어서기
                        {
                            zombieState = state.down1;

                           
                            zombieAni.SetBool("Down1", true);
                        }
                        else if (randomNo == 2) //crawl좀비로 부활
                        {
                            
                            zombieState = state.down2;
                            zombieAni.SetBool("Down2", true);
                        }
                        GameManager.instance.zombieHpBar.SetActive(false);
                    }
                    
                }
                break;

            case state.down1:
                {
                    StartCoroutine(StandUp());
                 }
                break;

            case state.down2:
                {

                    StartCoroutine(CrowlArise());
                }
                break;
            case state.death:
                {
                    Instantiate(EffectManager.instance.deathEffect,this.transform.position, this.transform.rotation);
                    Destroy(this.gameObject);
                }
                break;
            case state.crawlidle:
                {
                    
                    float distance = Vector3.Distance(target.transform.position, this.transform.position);
                    if (distance <= eyesight)
                    {
                        zombieAni.SetBool("CrawlMove", true);
                        zombieAni.SetBool("CrawlIdle", false);
                        zombieState = state.crawlrun;
                    }
                    else if (distance <= attackRange)
                    {                        
                        zombieAni.SetBool("CrawlAttack", true);
                        zombieAni.SetBool("CrawlMove", false);                        
                        zombieState = state.crawlattack;
                    }
                }
                break;
            case state.crawlrun:
                {
                    float distance = Vector3.Distance(target.transform.position, this.transform.position);
                    Vector3 dir = target.transform.position - this.transform.position;
                    dir.y = 0;
                    dir.Normalize();    //거리 평준화 함수
                    zombieController.SimpleMove(dir * speed);   //이동함수
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);  //회전
                    if(distance > eyesight)
                    {                       
                        zombieAni.SetBool("CrawlIdle", true);
                        zombieAni.SetBool("CrawlMove", false);
                        zombieState = state.crawlidle;
                    }
                    else if (distance <= attackRange)
                    {
                        zombieAni.SetBool("CrawlAttack", true);
                        zombieAni.SetBool("CrawlMove", false);
                        zombieState = state.crawlattack;
                    }
                }
                break;
            case state.crawlattack:
                {
                    float distance = Vector3.Distance(target.transform.position, this.transform.position);
                    Vector3 dir = target.transform.position - this.transform.position;
                    dir.y = 0;
                    dir.Normalize();    //거리 평준화 함수

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);  //회전
                    if (distance > attackRange)
                    {
                        zombieAni.SetBool("CrawlMove", true);
                        zombieAni.SetBool("CrawlAttack", false);
                        zombieState = state.crawlrun;
                    }

                }
                break;
            case state.crawlhit:
                {
                    if (hp > 0)
                    {
                        zombieState = state.crawlidle;
                    }
                    else if (hp <= 0)
                    {
                        GameManager.instance.zombieHpBar.SetActive(false);
                        zombieState = state.death;
                    }
                }
                break;
            case state.walk:
                {

                }
                break;
        }
    }
    //크라울 좀비 능력치 초기화
    void CrawlInit() {
        speed = 1.0f;
        rotSpeed = 1.0f;
        attackRange = 1.2f;
        zombieState = state.crawlidle;
        zombieAni.SetBool("CrawlIdle", true);
        zombieController.height = 1.0f;
        zombieController.center = new Vector3(-0.05f, 0.3f, 0);
    }
    IEnumerator CrowlArise()
    {
        zombieAni.SetBool("Idle", false);
        zombieAni.SetBool("Attack1", false);
        zombieAni.SetBool("Attack2", false);
        zombieAni.SetBool("Run", false);
        zombieAni.ResetTrigger("Hit");
        yield return new WaitForSeconds(1.0f);
        zombieAni.SetBool("CrawlIdle", true);
        zombieAni.SetBool("Down2", false);
        zombieState = state.crawlidle;
        this.gameObject.tag = "Crawl";
        hp = 10;
        CrawlInit();
    }
    IEnumerator StandUp()
    {
        yield return new WaitForSeconds(1.0f);
        zombieAni.SetBool("StandUp", true);
        zombieAni.SetBool("Down1", false);
        hp = 10;
        yield return new WaitForSeconds(3.0f);
        ZombieIdle();
    }
    private void OnCollisionEnter(Collision coll)
    {
        Vector3 rndRot = Vector3.right * Random.Range(200, 300);
        if (coll.gameObject.tag == "MissileN" && hp>0 && this.gameObject.tag =="Enemy")
        {
            zombieState = state.hit;
            zombieAni.SetTrigger("Hit");
            GameObject obj = Instantiate(EffectManager.instance.bloodEffect, coll.transform.position, coll.transform.rotation);          
            obj.transform.localRotation = Quaternion.Euler(rndRot);

            --hp;
            GameManager.instance.zombieHpBar.SetActive(true);   //좀비 hp바 보여주기
            GameManager.instance.zombieHpBar.GetComponent<EnergyBar>().SetValueCurrent(hp);

        }
        else if(coll.gameObject.tag == "PlayerKick" && hp> 0 && this.gameObject.tag == "Enemy")
        {
            zombieState = state.hit;
            zombieAni.SetTrigger("KickHit");
            GameManager.instance.zombieHpBar.SetActive(true);   //좀비 hp바 보여주기
        }
        else if (coll.gameObject.tag == "MissileN" && hp > 0 && this.gameObject.tag == "Crawl")
        {
            
            
            GameObject obj = Instantiate(EffectManager.instance.bloodEffect, coll.transform.position, coll.transform.rotation);
            obj.transform.localRotation = Quaternion.Euler(rndRot);
            
            zombieAni.SetTrigger("CrawlHit");
            zombieState = state.crawlhit;
            --hp;
            GameManager.instance.zombieHpBar.SetActive(true);   //좀비 hp바 보여주기
            GameManager.instance.zombieHpBar.GetComponent<EnergyBar>().SetValueCurrent(hp);
            
        }
        else if(coll.gameObject.tag == "PlayerKick" && hp > 0 && this.gameObject.tag == "Crawl")
        {
            zombieAni.SetTrigger("CrawlHit");
            zombieState = state.crawlhit;
            GameManager.instance.zombieHpBar.SetActive(true);   //좀비 hp바 보여주기;
            GameManager.instance.zombieHpBar.GetComponent<EnergyBar>().SetValueCurrent(hp);
           
        }
    }
}
