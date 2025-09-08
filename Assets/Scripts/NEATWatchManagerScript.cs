// NEATWatchingManager.cs, controls watching the AI bird

using UnityEngine;
using System.Collections.Generic;

public class NEATWatchManager : MonoBehaviour, IGameModeManager
{
    public GameObject birdPrefab;
    public List<BirdAIScript> birds = new List<BirdAIScript>();
    private bool watchingMode = false;

    public void StartMode() => StartWatching();

    public void StartWatching()
    {
        watchingMode = true;
        SpawnBestBird();
        Time.timeScale = 1f;
    }

    // Spawns the best bird model
    public void SpawnBestBird()
    {
        watchingMode = true;
        string path = Application.streamingAssetsPath + "/models/HardBird.json";
        BirdBrain brain = LoadBird(path);

        GameObject b = Instantiate(birdPrefab, Vector3.zero, Quaternion.identity);
        BirdAIScript ai = b.GetComponent<BirdAIScript>();
        ai.brain = brain;
        ai.rb.position = Vector3.zero;
        ai.rb.linearVelocity = Vector2.zero;
        ai.birdIsAlive = true;
        ai.fitness = 0f;

        birds.Add(ai);
    }

    void Update()
    {
        if (!watchingMode) return;
        if (birds.Count == 0) return;

        if (birds.TrueForAll(b => !b.birdIsAlive))
        {
            EndMode();
        }
    }

    public void EndMode()
    {
        foreach (var b in birds) if (b != null) Destroy(b.gameObject);
        birds.Clear();
        ClearBirds();

        LogicScript logic = Object.FindFirstObjectByType<LogicScript>();
        if (logic != null && logic.watchScoreText != null)
        {
            logic.watchScoreText.text = logic.playerScore.ToString();
            logic.ShowWatchScreen(logic.playerScore);
        }
        else
        {
            Debug.LogWarning("LogicScript or watchScoreText is missing!");
        }
        watchingMode = false;
    }

    public void ClearBirds()
    {
        foreach (var b in birds) if (b != null) Destroy(b.gameObject);
        birds.Clear();
    }

    BirdBrain LoadBird(string path)
    {
        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<BirdBrain>(json);
    }
}
