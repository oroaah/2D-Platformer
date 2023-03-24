using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class man : MonoBehaviour
{

    public int total;
    public int point;
    public int lv;
    public int hp;
    public playermove uuu;
    public GameObject[] Stages;

    public UnityEngine.UI.Image[] UIhelath;
    public UnityEngine.UI.Text UIpoint;
    public GameObject retry;
    

    

    private void Update()
    {
        UIpoint.text = (total + point).ToString();
    }
    public void Next()
    {
        if (lv < Stages.Length-1)
        {
            Stages[lv].SetActive(false);
            lv++;
            Stages[lv].SetActive(true);
            total += point;
            point = 0;
            uuu.transform.position = new Vector3(-9, 3, -1);
        }
        else
        {
            Time.timeScale = 0;
            
            UnityEngine.UI.Text btnText = retry.GetComponentInChildren<UnityEngine.UI.Text>();
            btnText.text = "Game Clear";
            retry.SetActive(true);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (hp > 1)
            {
                HealthDown();
                collision.attachedRigidbody.velocity = Vector2.zero;
                collision.transform.position = new Vector3(-9, 3, -1);
            }
            else
            {
                HealthDown();
            }
        }
    }
    public void HealthDown()
    {
        uuu.PlaySound("DAMAGED");
        if (hp > 1)
        {
            hp--;
            UIhelath[hp].color = new Color(1, 0, 0, 0.2f);
        }
        else
        {
            UIhelath[0].color = new Color(1, 0, 0, 0.2f);
        
            uuu.die();
            Debug.Log("ав╬З╫ю╢о╢ы!");
            retry.SetActive(true);

        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
