// NEATBattleManager.cs, controls the player vs AI mode

using UnityEngine;
using UnityEngine.UI;

public class PlayerVsAIBattleManager : MonoBehaviour, IGameModeManager
{
    public GameObject playerBirdPrefab;
    public GameObject aiBirdPrefab;
    public LogicScript logic;
    public Text Winner;
    public Button Easy;
    public Button Medium;
    public Button Hard;
    public GameObject BattleSelectionScreen;

    private string path = Application.dataPath;
    private BirdScript playerBird;
    private BirdAIScript aiBird;

    public void StartMode()
    {
        if (BattleSelectionScreen != null) 
            BattleSelectionScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ClearBirds()
    {
        playerBird = null;
        GameObject[] birds = GameObject.FindGameObjectsWithTag("Bird");
        foreach (GameObject b in birds)
            Destroy(b);
    }

    // ---- Loading Various Models -----
    public void EasyClick()
    {
        path = Application.dataPath + "/Models/EasyBird.json";
        if (BattleSelectionScreen != null) BattleSelectionScreen.SetActive(false);
        StartBattle(); 
    }

    public void MediumClick()
    {
        path = Application.dataPath + "/Models/MediumBird.json";
        if (BattleSelectionScreen != null) BattleSelectionScreen.SetActive(false);
        StartBattle(); 
    }

    public void HardClick()
    {
        path = Application.dataPath + "/Models/HardBird.json";
        if (BattleSelectionScreen != null) BattleSelectionScreen.SetActive(false);
        StartBattle(); 
    }

    public void StartBattle()
    {

        // --- Spawn Player Bird ---
        GameObject playerObj = Instantiate(playerBirdPrefab, Vector3.zero, Quaternion.identity);
        playerBird = playerObj.GetComponent<BirdScript>();
        playerBird.logic = Object.FindFirstObjectByType<LogicScript>();

        // --- Spawn AI Bird ---
        GameObject aiObj = Instantiate(aiBirdPrefab, Vector3.zero, Quaternion.identity);
        aiBird = aiObj.GetComponent<BirdAIScript>();

        if (aiBird.rb == null)
            aiBird.rb = aiBird.GetComponent<Rigidbody2D>();

        // Attempts to load brain
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            aiBird.brain = JsonUtility.FromJson<BirdBrain>(json);

            if (aiBird.brain == null || aiBird.brain.layers == null || aiBird.brain.layerSizes == null || aiBird.brain.layerSizes.Length == 0)
            {
                Debug.LogWarning("Loaded brain is invalid. Creating a new random brain.");
                aiBird.brain = new BirdBrain(new int[] { 4, 5, 1 });
            }
        }
        else
        {
            Debug.LogWarning("BestBird.json not found. Creating new brain.");
            aiBird.brain = new BirdBrain(new int[] { 4, 5, 1 });
        }

        // Initialize AI bird
        aiBird.rb.position = Vector3.zero;
        aiBird.rb.linearVelocity = Vector2.zero;
        aiBird.birdIsAlive = true;
        aiBird.fitness = 0f;
        
        // Start Time
        Time.timeScale = 1f;
    }


    void Update()
    {
        // Checks which bird dies first
        if (playerBird != null && aiBird != null)
        {
            string winnerTemplate = "";
            if (!playerBird.birdIsAlive && !aiBird.birdIsAlive) {
                Winner.text = winnerTemplate + "TIE";
                logic.gameBattleScreenEnd();
            }
            else if (!aiBird.birdIsAlive) {
                Winner.text = winnerTemplate + "Winner: PLayer";
                logic.gameBattleScreenEnd();
            }
            else if (!playerBird.birdIsAlive) {
                Winner.text = winnerTemplate + "Winner: AI";
                logic.gameBattleScreenEnd();
            }
        }
 
    }

    public void EndMode()
    {
        ClearBirds();
    }

    // Unused Interface Function

    public void SpawnBestBird()
    {
        Debug.Log("PlayerVsAI mode does not manually spawn best bird.");
    }
}
