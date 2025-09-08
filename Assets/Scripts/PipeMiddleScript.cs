// PipeMiddleScript.cs, used to check when the bird passes through the middle of pipes

using UnityEngine;

public class PipeMiddleScript : MonoBehaviour
{
    public LogicScript logic;
    public BirdAIScript bird;
    
    void Start()
    {
        logic = Object.FindFirstObjectByType<LogicScript>();
    }

    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bird = collision.GetComponent<BirdAIScript>();

        if (collision.gameObject.layer == 3)
        {
            logic.AddScore(1);
        }
        GetComponent<Collider2D>().enabled = false;
    }
}
