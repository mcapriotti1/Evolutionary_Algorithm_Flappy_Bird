// LogicManagerScript.cs, controls UI and managers

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    // --- Game State & UI ---
    [Header("Score/UI")]
    public int playerScore;
    public Text scoreText;
    public GameObject gameBattleScreen;
    public GameObject gameStartScreen;
    public GameObject gameInScreen;
    public GameObject gameTrainScreen;
    public GameObject gameWatchScreen;
    public GameObject gameBattleEndScreen;
    public Text watchScoreText;

    [Header("Training UI")]
    public Text populationText;
    public Text generationText;
    public Text currentFitnessText;
    public Text fitnessThresholdText;

    private int lastScoreFrame = -1;

    // --- Active Game Mode ---
    public IGameModeManager activeManager;

    // ---------------------- Score Methods ----------------------
    public void AddScore(int scoreToAdd)
    {
        if (Time.frameCount == lastScoreFrame) return;

        playerScore += scoreToAdd;
        if (scoreText != null)
            scoreText.text = playerScore.ToString();

        lastScoreFrame = Time.frameCount;
    }

    public void gameBattleScreenEnd()
    {
        Time.timeScale = 0f;
        ResetForAI();
        Debug.Log("GAME OVER");
        if (gameBattleScreen != null) gameBattleScreen.SetActive(true);
        if (gameBattleEndScreen != null) gameBattleEndScreen.SetActive(true);
    }

    // ---------------------- Reset Methods ----------------------
    public void ResetForAI()
    {
        playerScore = 0;
        scoreText.text = playerScore.ToString();

        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");
        activeManager?.ClearBirds();
        foreach (GameObject p in pipes)
            Destroy(p);
    }

    public void ResetPipes()
    {
        PipeSpawnerScript pipeSpawner = FindAnyObjectByType<PipeSpawnerScript>();
        if (pipeSpawner != null)
            pipeSpawner.ResetSpawner();
            
        playerScore = 0;
        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");
        foreach (GameObject p in pipes)
            Destroy(p);

    }

    // ---------------------- UI Methods ----------------------
    public void UpdateTrainingUI(int population, int generation, float currentFitness, float fitnessThreshold)
    {
        if (populationText != null) populationText.text = "Population: " + population;
        if (generationText != null) generationText.text = "Generation: " + generation;
        if (currentFitnessText != null) currentFitnessText.text = "Current Fitness: " + currentFitness.ToString("F2");
        if (fitnessThresholdText != null) fitnessThresholdText.text = "Threshold: " + fitnessThreshold.ToString("F2");
    }

    public void ShowWatchScreen(int score)
    {
        if (gameWatchScreen != null)
            gameWatchScreen.SetActive(true);

        if (watchScoreText != null)
            watchScoreText.text = "Score: " + score;

        Time.timeScale = 0f; // pause game
    }

    public void HideWatchScreen()
    {
        if (gameWatchScreen != null)
            gameWatchScreen.SetActive(false);
        if (gameStartScreen != null)
            gameStartScreen.SetActive(false);
        if (gameBattleScreen != null)
            gameBattleScreen.SetActive(false);
        if (gameInScreen != null)
            gameInScreen.SetActive(true);

        Time.timeScale = 1f; // resume game
    }

    // ---------------------- Menu & Button Methods ----------------------
    public void OnTrainClicked()
    {
        if (gameStartScreen != null) gameStartScreen.SetActive(false);
        if (gameWatchScreen != null) gameWatchScreen.SetActive(false);
        if (gameInScreen != null) gameInScreen.SetActive(true);
        if (gameTrainScreen != null) gameTrainScreen.SetActive(true);

        activeManager = (IGameModeManager)FindAnyObjectByType<NEATTrainingManager>();
        activeManager?.StartMode();
    }

    public void OnWatchClicked()
    {
        if (gameWatchScreen != null) gameWatchScreen.SetActive(false);
        ResetPipes();
        if (gameStartScreen != null) gameStartScreen.SetActive(false);
        if (gameInScreen != null) gameInScreen.SetActive(true);

        activeManager = (IGameModeManager)FindAnyObjectByType<NEATWatchManager>();
        activeManager?.StartMode();
    }

    public void OnPlayVsAIClicked()
    {
        if (gameStartScreen != null) gameStartScreen.SetActive(false);
        if (gameBattleEndScreen != null) gameBattleEndScreen.SetActive(false);
        if (gameInScreen != null) gameInScreen.SetActive(true);

        activeManager = (IGameModeManager)FindAnyObjectByType<PlayerVsAIBattleManager>();

        if (gameBattleScreen != null) gameBattleScreen.SetActive(true);

        activeManager?.StartMode();
    }

    public void OnRespawnClicked()
    {
        playerScore = 0;
        if (scoreText != null) scoreText.text = playerScore.ToString();
        if (watchScoreText != null) watchScoreText.text = "Score: 0";
        HideWatchScreen();
        ResetForAI();
        activeManager?.SpawnBestBird();
    }

    public void OnPlayAgainClicked()
    {
        playerScore = 0;
        if (scoreText != null) scoreText.text = playerScore.ToString();
        HideWatchScreen();
        ResetForAI();
        if (gameBattleEndScreen != null) gameBattleEndScreen.SetActive(false);
        if (gameBattleScreen != null) gameBattleScreen.SetActive(true);
        activeManager?.StartMode();
    }

    public void OnMainMenuClicked()
    {
        if (activeManager != null)
        {
            activeManager.EndMode();
            activeManager = null;
        }
        ResetForAI();
        Time.timeScale = 0f;

        if (gameStartScreen != null) gameStartScreen.SetActive(true);
        if (gameInScreen != null) gameInScreen.SetActive(false);
        if (gameTrainScreen != null) gameTrainScreen.SetActive(false);
        if (gameBattleScreen != null) gameBattleScreen.SetActive(false);
        if (gameWatchScreen != null) gameWatchScreen.SetActive(false);
    }

    public void OnExitClicked()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
