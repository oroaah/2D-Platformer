using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermove : MonoBehaviour
{
    SpriteRenderer spr;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    Animator anim;
    BoxCollider2D col;

    public man mana;
    public AudioClip aJump;
    public AudioClip aCoin;
    public AudioClip aDamage;
    public AudioClip aPause;
    public AudioClip aAttack;
    AudioSource audioSource;
    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Jump

        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
            PlaySound("JUMP");
        }

        //Stop SPeed
       if(Input.GetButtonUp("Horizontal"))
        {
            
            rigid.velocity = new Vector2(rigid.velocity.normalized.x *0.5f, rigid.velocity.y);
        }

        //애니메이션 방향전환
        if (Input.GetButton("Horizontal"))
            spr.flipX = Input.GetAxisRaw("Horizontal") == -1;
        //flipX는 bool값이므로 수평속도가 -1일때 참이되어 flipX가 적용됨


        //걷기 애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalk", false);
        else
            anim.SetBool("isWalk", true);

    }
    void FixedUpdate()
    {
        //좌우 움직임
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) //오른쪽 최대속력
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))   //왼쪽 최대속력이므로 -1 곱해줌
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);


        if(rigid.velocity.y < 0) { 
        //착지
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        //플레이어위치(rigidpos)에서 아래로 레이를 쏨

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1,LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)   //빔을 쏴서 맞은 것이 있으면
            {
                if (rayHit.distance < 0.53f)
                    Debug.Log(rayHit.collider.name);
                anim.SetBool("isJump", false);
            }
        }
    }


    public void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = aJump;
                audioSource.Play();
                break;
            case "ATTACK":
                audioSource.clip = aAttack;
                audioSource.Play();
                break;
            case "DAMAGED":
                audioSource.clip = aDamage;
                audioSource.Play();
                break;
            case "ITEM":
                audioSource.clip = aCoin;
                audioSource.Play();
                break;
            case "PAUSE":
                audioSource.clip = aPause;
                audioSource.Play();
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)  //collision은 부딪힌 대상
    {
        if(collision.gameObject.tag == "gasi" || collision.gameObject.tag == "gas")
        {
            //어택
            if (rigid.velocity.y < 0 && transform.position.y+0.3f > collision.transform.position.y&& 
                collision.gameObject.tag == "gasi")
            {
                OnAttack(collision.transform);
                PlaySound("ATTACK");

            }
            else //damaged
                OnDamaged(collision.transform.position);
            
            
        }
    }

    private void OnAttack(Transform enemy)
    {

        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        enemymove eem = enemy.GetComponent<enemymove>();
        eem.OnDamaged();
        mana.point += 100;
    }

    void OnDamaged(Vector2 targetPos)
    {
        
        gameObject.layer = 9;
        mana.HealthDown();
        spr.color = new Color(1, 1, 1, 0.4f);
        //튕겨나감
        int dirc = transform.position.x - targetPos.x > 0 ? 1 :-1;
        rigid.AddForce(new Vector2(dirc,1.3f)*5,ForceMode2D.Impulse);
        
        Invoke("OffDamaged", 3);


        //애니메이션
        anim.SetTrigger("dmged");
    
    }
    void OffDamaged()
    {
        gameObject.layer = 8;
        spr.color = new Color(1, 1, 1, 1);
       
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "item")
        {
            PlaySound("ITEM");
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
            {

                mana.point += 50;
            }

            else if (isSilver)
            {


                mana.point += 100;
            }


            else if (isGold)
            {

                mana.point += 300;
            }
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            collision.gameObject.SetActive(false);

            mana.Next();
        }
    }

    public void die()
    {
        PlaySound("PAUSE");
        spr.color = new Color(1, 1, 1, 0.4f);
        spr.flipY = true;
        col.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
      
    }

}
