using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodEffect : MonoBehaviour
{
    public static Image img;
    public Sprite bloodImg1;
    public Sprite bloodImg2;

    public Color startColor = new Color(1,1,1,0);
    public Color endColor = new Color(1, 1, 1, 1);


    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        if(Player.instance.ps == Player.playerState.damage)
        {
            if(GameManager.instance.hp >= 20)
            {
                StartCoroutine(FadeEffect());
                
            }
            if (GameManager.instance.hp <= 20)
            {
                img.sprite = bloodImg2;
                img.color = Color.Lerp(endColor, startColor, Mathf.PingPong(Time.time, 1.0f));
            }
        }   
    }

    IEnumerator FadeEffect()
    {
        img.sprite = bloodImg1;
        img.color = startColor;

        for (float i = 1f; i >= -0.1f; i -= 0.02f)
        {
            Color color = new Vector4(1, 1, 1, i);
            img.color = color;
            yield return new WaitForEndOfFrame();
        }

    }
}
