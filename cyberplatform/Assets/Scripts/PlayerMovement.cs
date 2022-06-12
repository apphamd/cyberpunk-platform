using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float dirX = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(dirX * 7f, rb.velocity.y); // Multiply by current dirX value, so if not moving it will sto



        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x, 14f);
            
        }


    }

    
}
