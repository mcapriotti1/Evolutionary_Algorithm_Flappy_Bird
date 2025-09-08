// PipeSpawnerScript.cs, spawns the pipes

using UnityEngine;

public class PipeSpawnerScript : MonoBehaviour
{
    public GameObject pipe;
    public float spawnRate = 2f;
    private float timer = 0f;
    public float heightOffset = 10f;
    public float firstPipeHeight = -9f; // fixed height for first pipe
    private bool firstPipeSpawned = false;

    void Start()
    {
        Time.timeScale = 0f;
        ResetSpawner();
        spawnPipe();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            spawnPipe();
            timer = 0f;
        }
    }

    // Randomly spawns the pipe prefab betwen the lowest and heighest point
    void spawnPipe()
    {
        float yPos;

        if (!firstPipeSpawned)
        {
            yPos = firstPipeHeight;
            firstPipeSpawned = true;
        }
        else
        {
            float lowestPoint = transform.position.y - heightOffset;
            float highestPoint = transform.position.y + heightOffset;
            yPos = Random.Range(lowestPoint, highestPoint);
        }

        Instantiate(pipe, new Vector3(transform.position.x, yPos, transform.position.z), transform.rotation);
    }

    // Resets the spawner
    public void ResetSpawner()
    {
        firstPipeSpawned = false;
        timer = 0f;

        // Destroys all existing pipes
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Pipe"))
            Destroy(p);
    }
}
