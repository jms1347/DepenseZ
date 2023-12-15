using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    //플레이어 상태
    public enum playerState
    {
        idle = 0,           //기본상태
        move = 1,           //이동상태              (현재 안씀)
        kick = 2,           //킥
        jump = 3,           //점프상태
        attack = 4,         //미사일 공격중인 상태
        damage = 5          //공격받는 상태
    }
    public playerState ps = playerState.idle;  //기본상태

    //플레이어 무기 소지 상태(총 위치 값만 변경해서 눈속임)
    bool isGun = true; //총을 들고 있는지 여부 체크;
    Transform leftGun;  //왼쪽무기
    Transform rightGun; //오른쪽무기
    public Transform leftGunCrowding;  //왼쪽총집
    public Transform rightGunCrowding; //오른쪽총집
    public Transform leftGunHanding;  //왼쪽총들기
    public Transform rightGunHanding; //오른쪽총들기

    Transform tr;                         //자기 자신
    public Transform cameraPos;         //카메라 위치
    public Transform skPoint_l;          //스킬이나 미사일이 나가는 점
    public Transform skPoint_r;          //스킬이나 미사일이 나가는 점

    float moveSpeed = 6.0f;    //이동 속도
    float backmoveSpeed = 5.0f;    //뒤로가는 속도
    float jumpPower = 10.0f;    //점프 파워
    float gravity = 9.8f;       //중력 적용
    float playerGravity = 0;          //플레이어 중력적용에 필요한 변수

    Vector3 moveDir = Vector3.zero; //플레이어 이동방향

    CharacterController playerController;

    //미사일 발사
    public GameObject MissileN;        //마사일 프리팹
    float csSpeed = 0.2f;       //연사 속도
    bool isFire = false;        //미사일발사 제어변수

    //플레이어 애니메이션
    Animator playerAni;
    //public Camera shakeCamera;
    //Vector3 shakeCameraOrignPos;
    void Awake()
    {
        instance = this;    //싱글턴

        tr = transform; //자기 자신 직접 참조
        playerAni = GetComponent<Animator>();
        playerController = GetComponent<CharacterController>();
        leftGun = this.transform.GetChild(3);
        rightGun = this.transform.GetChild(4);
        //shakeCameraOrignPos = shakeCamera.transform.position;   //쉐이크 카메라 최초 위치 저장
    }

    void Update()
    {    
        PlayerState();
        PlayerJump();
        PlayerMove();
        if (Input.GetMouseButton(0)) if (isGun) StartCoroutine(MissileFire());  //총들고있을 때 미사일발사

        if (Input.GetKeyDown(KeyCode.Alpha1))  //총들고있을때 총 감추기
        {
            if (isGun)
            {
                //leftGun.position = Vector3.MoveTowards(leftGun.position, leftGunCrowding.transform.position, 100 * Time.smoothDeltaTime);
               // rightGun.position = Vector3.MoveTowards(rightGun.position, rightGunCrowding.transform.position, 100 * Time.smoothDeltaTime);
                leftGun.gameObject.SetActive(false);;
                rightGun.gameObject.SetActive(false);
                isGun = false;
            }          
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))  //총들고있을 때 총 들기
        {
            if (!isGun)
            {
                //leftGun.position = Vector3.MoveTowards(leftGun.position, leftGunHanding.transform.position, 100 * Time.smoothDeltaTime);
                //rightGun.position = Vector3.MoveTowards(rightGun.position, rightGunHanding.transform.position, 100 * Time.smoothDeltaTime);
                leftGun.gameObject.SetActive(true);
                rightGun.gameObject.SetActive(true);
                isGun = true;
            }
        }
    }

    void LateUpdate()
    {
        Camera.main.transform.position = cameraPos.transform.position;
    }


    void PlayerState()
    {
        switch (ps)
        {
            case playerState.idle:  //기본상태
                {
                    if (Input.GetKeyDown("q") && !isGun)    //총 안들때만 가능
                    {
                        ps = playerState.kick;
                        playerAni.SetBool("Kick", true);
                        playerAni.SetBool("Idle", false);
                        if (ps == playerState.kick && Input.GetKey("q"))
                        {
                            playerAni.SetBool("TurnKick", true);
                        }
                    }
                }
                break;
            case playerState.jump:  //점프상태
                {
                    if (playerController.isGrounded)
                    {
                        ps = playerState.idle;
                    }
                }
                break;

            case playerState.kick:  //발차기상태
                {
                    if (!Input.GetKey("q"))
                    {
                        ps = playerState.idle;
                        playerAni.SetBool("Kick", false);
                        playerAni.SetBool("TurnKick", false);
                        playerAni.SetBool("Idle", true);
                    }
                }
                break;
            case playerState.damage:  //공격받는 상태
                {
                   
                }
                break;

        }
    }


    IEnumerator MissileFire()
    {
        if (!isFire)
        {
            isFire = true;
            Instantiate(MissileN,skPoint_l.transform.position, skPoint_l.transform.rotation);
            Instantiate(MissileN,skPoint_r.transform.position, skPoint_r.transform.rotation);
            float x = Random.Range(-0.02f, 0.02f); //x축 반동
            float y = Random.Range(-0.02f, 0.02f); //y축 반동
            float z = Random.Range(-0.02f, 0.02f); //z축 반동

            //쉐이크 카메라 x축 -수직 ,y축 - 수평

            EffectManager.instance.left_muzzleEffect.SetActive(true);
            EffectManager.instance.right_muzzleEffect.SetActive(true);
            
            leftGun.position += new Vector3(x, y, z);   //반동효과
            rightGun.position += new Vector3(x, y, z);  //반동효과
            ps = playerState.attack;
            yield return new WaitForSeconds(csSpeed);
            isFire = false;
            leftGun.position = leftGunHanding.position;     //벗어나지 않게 원래 위치 복귀
            rightGun.position = rightGunHanding.position;   //벗어나지 않게 원래 위치 복귀

            EffectManager.instance.left_muzzleEffect.SetActive(false);
            EffectManager.instance.right_muzzleEffect.SetActive(false);
            ps = playerState.idle;
        }
    }

    //플레이어 점프 - 스테미너로 체크, 또한 지면가 떨어질 때 y축 카메라 조절
    void PlayerJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            playerGravity = jumpPower;
            ps = playerState.jump;
        }
    }

    //플레이어 이동 - 중력까지 따로 적용
    void PlayerMove()
    {
        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        moveDir.x = x;
        moveDir.y = 0;
        moveDir.z = z;

        moveDir = tr.TransformDirection(moveDir);

        if (z >= 0)
        {
            moveDir *= moveSpeed;
        }else if (z < 0)
        {
            moveDir *= backmoveSpeed;
        }
       
        playerGravity -= gravity * Time.deltaTime;
        moveDir.y = playerGravity;

        playerController.Move(moveDir * Time.deltaTime);    //캐릭터 컨트롤러에 있는 이동 함수 호출
        tr.transform.rotation = Camera.main.transform.rotation;
        
    }
    private void OnCollisionEnter(Collision coll)
    {
         if(coll.gameObject.tag == "EnemyHand")
        {
            ps = playerState.damage;
        }else
        {
            ps = playerState.idle;
        }
    }
}
