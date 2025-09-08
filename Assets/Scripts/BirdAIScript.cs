// BirdAIScript.cs, controls behavior of AI bird

using UnityEngine;

public class BirdAIScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float flapStrength = 5f;
    public BirdBrain brain;
    private Transform nextPipe;
    public float fitness = 0f;
    public float rotationStrength = 5f;
    public bool birdIsAlive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (brain == null || brain.layers == null || brain.layerSizes[0] != 4)
        {
            // No Brain Loaded
            Debug.Log("RANDOM NEW BRAIN");
            brain = new BirdBrain(new int[] { 4, 5, 1 });
        }
    }
    
    // Max/Mins for each input
    float minY = -60f, maxY = 10f;
    float minVelY = -110f, maxVelY = 10f;
    float minPipeX = -1f, maxPipeX = 25f;
    float minPipeY = -10f, maxPipeY = 55f;

    // -1 to 1 normalization
    public float Normalize(float value, float min, float max)
    {
        if (max - min == 0f) return 0f;
        return ((value - min) / (max - min)) * 2f - 1f;
    }

    void Update()
    {
        if (!birdIsAlive) return;

        if (nextPipe == null || nextPipe.position.x < transform.position.x)
        {
            nextPipe = GetNextPipe();
        }

        if (nextPipe != null)
        {
            float[] inputs = {
            Normalize(rb.position.y, minY, maxY),
            Normalize(rb.linearVelocity.y, minVelY, maxVelY),
            Normalize(nextPipe.position.x - rb.position.x, minPipeX, maxPipeX),
            Normalize(nextPipe.position.y - rb.position.y, minPipeY, maxPipeY)
            };
        
            float[] activations = brain.FeedForward(inputs);

            float decision = activations[0];

            if (decision > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * flapStrength, ForceMode2D.Impulse);
            }
            
            // Fitness based on time
            fitness += Time.deltaTime * 0.1f;
            
            float angle = Mathf.Clamp(rb.linearVelocity.y * rotationStrength, -90f, 90f);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
    }
    
    // Kill bird if collides 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        birdIsAlive = false;
    }

    // Finds all pipes and gets the closest pipe not already passed
    Transform GetNextPipe()
    {
        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        
        foreach (GameObject pipe in pipes)
        {
            float distance = pipe.transform.position.x - transform.position.x;

            if (distance > 0 && distance < minDistance)
            {
                minDistance = distance;
                nearest = pipe.transform;
            }
        }
        return nearest;
    }


}
