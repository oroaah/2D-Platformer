using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemymove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spr;
    BoxCollider2D col;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("Think", 4);
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }


    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);


        //플랫폼 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }


    void Think()
    {
        nextMove = Random.Range(-1, 2); //최댓값이 랜덤범위에 포함이 안되므로 2를 넣음
        float ttime = Random.Range(2, 5);

        anim.SetInteger("walkspeed", nextMove);
        if (nextMove != 0)
        {
            spr.flipX = nextMove == 1; //몹이 오른쪽을 볼때 flipX체크됨(bool형)
        }

        Invoke("Think", ttime);
    }

    void Turn()
    {
        nextMove *= -1;   // 넥스트무브의 방향을 바꿈
        spr.flipX = nextMove == 1;
        CancelInvoke("Think");
        Invoke("Think", 4);

    }

    public void OnDamaged()
    {
        //몬스터 죽음

        spr.color = new Color(1, 1, 1, 0.4f);
        spr.flipY = true;
        col.enabled = false;
        rigid.AddForce(Vector2.up * 5,ForceMode2D.Impulse);
        Invoke("Deactive", 4);
    }

    void Deactive()
    {
        gameObject.SetActive(false);
    }
}
