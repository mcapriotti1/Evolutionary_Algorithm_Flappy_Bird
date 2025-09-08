// BirdScript.cs, controls behavior of player bird

using UnityEngine;
using UnityEngine.InputSystem;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float flapStrength = 5f;
    public InputActionReference jump;
    public bool birdIsAlive = true;
    public float rotationStrength = 5f;
    public LogicScript logic;


    void OnEnable()
    {
        if (jump != null && jump.action != null)
            jump.action.Enable();
    }

    void Update()
    {
        // Controls set in Unity, Spacebar is jump
        if (jump != null && jump.action != null && jump.action.triggered && birdIsAlive)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * flapStrength, ForceMode2D.Impulse);
        }

        float angle = Mathf.Clamp(rb.linearVelocity.y * rotationStrength, -90f, 90f);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDisable()
    {
        if (jump != null && jump.action != null)
            jump.action.Disable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        birdIsAlive = false;
    }
}
