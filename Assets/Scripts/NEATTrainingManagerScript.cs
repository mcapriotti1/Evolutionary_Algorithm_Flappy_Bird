// NEATTrainingManager.cs, controls the training of the AI bird

using UnityEngine;
using System.Collections.Generic;

public class NEATTrainingManager : MonoBehaviour, IGameModeManager
{
    public GameObject birdPrefab;
    public int populationSize = 50;
    public float fitnessThreshold = 1;
    public int evaluationsPerBird = 10; // Number of cycles between evolutions
    public float speed = 20f;
    public float maxFitness = 5f;

    public GameObject redoButton;
    public List<BirdAIScript> birds = new List<BirdAIScript>();

    private int generationNumber = 0;
    private bool trainingMode = false;
    private int currentEvaluation = 0;

    // stores cumulative fitness across runs
    private Dictionary<BirdAIScript, float> cumulativeFitness = new Dictionary<BirdAIScript, float>();

    public void StartMode() => StartTraining();

    public void ClearBirds()
    {
        foreach (var b in birds)
            if (b != null) Destroy(b.gameObject);
        birds.Clear();
    }

    public void StartTraining()
    {
        redoButton.SetActive(false);
        trainingMode = true;
        Time.timeScale = speed;

        Debug.Log("Training mode started!");
        currentEvaluation = 0;
        SpawnPopulation();
    }

    // Initil Spawn of birds
    void SpawnPopulation()
    {
        birds.Clear();
        cumulativeFitness.Clear();

        for (int i = 0; i < populationSize; i++)
        {
            GameObject b = Instantiate(birdPrefab, Vector3.zero, Quaternion.identity);
            BirdAIScript birdScript = b.GetComponent<BirdAIScript>();

            // Each bird gets randomized brain
            birdScript.brain = new BirdBrain(new int[] { 4, 5, 1 });

            birds.Add(birdScript);
            cumulativeFitness[birdScript] = 0f;
        }
    }

    void Update()
    {
        if (!trainingMode) return;

        LogicScript logic = Object.FindFirstObjectByType<LogicScript>();
        if (logic == null) return;
        if (birds.Count == 0) return;

        float bestFitness = 0f;
        foreach (var b in birds)
            if (b.fitness > bestFitness) bestFitness = b.fitness;

        if (bestFitness >= maxFitness)
        {
            Debug.Log($"Reached fitness {bestFitness}, evolving early!");
            
            // Kill all birds immediately (otherwise training may never end)
            foreach (var b in birds)
                b.birdIsAlive = false;
        }

        logic.UpdateTrainingUI(populationSize, generationNumber, bestFitness, fitnessThreshold);

        bool allDead = birds.TrueForAll(b => !b.birdIsAlive);

        if (allDead)
        {
            // Add this round's fitness to cumulative
            foreach (var b in birds)
                cumulativeFitness[b] += b.fitness;

            currentEvaluation++;

            if (currentEvaluation < evaluationsPerBird)
            {
                // Reset and run another evaluation
                logic.ResetPipes();
                ResetBirds();
                logic.playerScore = 0;
                logic.scoreText.text = "0";
            }
            else
            {
                // Average fitness after all evaluations
                foreach (var b in birds)
                    b.fitness = cumulativeFitness[b] / evaluationsPerBird;

                // Pick best bird
                BirdAIScript bestBird = birds[0];
                foreach (var b in birds) if (b.fitness > bestBird.fitness) bestBird = b;

                Debug.Log($"Best bird average fitness this generation: {bestBird.fitness:F2}");

                if (bestBird.fitness >= fitnessThreshold)
                {
                    SaveBird(bestBird);
                    logic.ResetPipes();
                    EndMode();
                }
                else
                {
                    logic.ResetPipes();
                    logic.playerScore = 0;
                    logic.scoreText.text = "0";
                    EvolveNextGeneration();
                }
            }
        }
    }

    void ResetBirds()
    {
        foreach (var b in birds)
        {
            b.rb.position = Vector3.zero;
            b.rb.linearVelocity = Vector2.zero;
            b.birdIsAlive = true;
            b.fitness = 0f;
        }
    }

    void SaveBird(BirdAIScript bird)
    {
        string path = Application.dataPath + "/Models/BestBird.json";
        Debug.Log(bird.brain);
        string json = JsonUtility.ToJson(bird.brain);
        System.IO.File.WriteAllText(path, json);
        Debug.Log("Saved best bird to " + path);
    }

    void EvolveNextGeneration()
    {
        float changeFrequency = 0.2f;
        float changeAmount = 0.2f;

        birds.Sort((a, b) => b.fitness.CompareTo(a.fitness));
        int eliteCount = Mathf.CeilToInt(populationSize * 0.2f);

        List<BirdBrain> newBrains = new List<BirdBrain>();

        // keep elites
        for (int i = 0; i < eliteCount; i++)
            newBrains.Add(birds[i].brain.Clone());

        // breed/mutate
        while (newBrains.Count < populationSize)
        {
            BirdBrain parent = newBrains[Random.Range(0, eliteCount)];
            BirdBrain child = parent.Clone();

            for (int l = 0; l < child.layers.Length; l++)
            {
                // mutate weights
                for (int i = 0; i < child.layers[l].weights.Length; i++)
                {
                    for (int j = 0; j < child.layers[l].weights[i].values.Length; j++)
                    {
                        if (Random.value < changeFrequency)
                            child.layers[l].weights[i].values[j] += Random.Range(-changeAmount, changeAmount);
                    }
                }

                // mutate biases
                for (int j = 0; j < child.layers[l].biases.Length; j++)
                {
                    if (Random.value < changeFrequency)
                        child.layers[l].biases[j] += Random.Range(-changeAmount, changeAmount);
                }
            }

            newBrains.Add(child);
        }

        // assign new brains
        for (int i = 0; i < populationSize; i++)
        {
            birds[i].brain = newBrains[i];
            birds[i].rb.position = Vector3.zero;
            birds[i].rb.linearVelocity = Vector2.zero;
            birds[i].birdIsAlive = true;
            birds[i].fitness = 0f;
            cumulativeFitness[birds[i]] = 0f;
        }

        currentEvaluation = 0;
        generationNumber++;
    }

    public void EndMode()
    {
        Debug.Log("Ending training mode.");
        ClearBirds();
        LogicScript logic = Object.FindFirstObjectByType<LogicScript>();
        logic?.ResetPipes();
        trainingMode = false;
        Time.timeScale = 0f;

        if (redoButton != null)
            redoButton.SetActive(true);
    }

    public void RedoTraining()
    {
        Debug.Log("Redo training clicked!");
        
        generationNumber = 0;
        ClearBirds();

        LogicScript logic = Object.FindFirstObjectByType<LogicScript>();
        if (logic != null)
        {
            logic.playerScore = 0;
            logic.scoreText.text = "0";
            logic.ResetPipes();
        }

        StartTraining();
    }

    // Unused
    public void SpawnBestBird()
    {
        Debug.Log("Training mode does not spawn best bird manually.");
    }
}
