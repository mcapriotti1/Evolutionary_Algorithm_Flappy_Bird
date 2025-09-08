// PipeMoveScript.cs, moves the pipes along the screen

using UnityEngine;

public class PipeMoveScript : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float deadZone = -25;

    // Update is called once per fram
    void Update()
    {
        if (Time.timeScale == 0f) return; // freeze movement if game is paused

        // Destroys the pipes once offscreen
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }

        transform.position += Vector3.left * moveSpeed * Time.deltaTime; 
    }
}
