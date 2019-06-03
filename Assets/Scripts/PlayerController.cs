using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Vector2 moveVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveVelocity = moveInput * speed;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Shop Scene Switch")
        {
            SceneManager.LoadScene("Shop");
        }
        else if(collision.gameObject.tag == "Battle Scene Switch")
        {
            SceneManager.LoadScene("SampleScene");
        }
        else if(collision.gameObject.tag == "Town Scene Switch")
        {
            SceneManager.LoadScene("Town");
        }
    }


}
