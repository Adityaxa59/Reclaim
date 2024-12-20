using UnityEngine;
using UnityEngine.InputSystem;
public class flying : MonoBehaviour
{

    [SerializeField] public float _velocity = 0f;
   
     public Rigidbody2D rb;
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            rb.linearVelocity = Vector2.up * _velocity;
        if (Mouse.current.rightButton.wasPressedThisFrame)
            rb.linearVelocity = Vector2.down * _velocity;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        gameover.instance.Gameover();
        Debug.Log ("hit");
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        gameover.instance.Point();
    }
}
