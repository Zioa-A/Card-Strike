using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public bool isGameOver = false;

    [Header("UI")]
    public GameObject stageClearPanel;
    public GameObject losePanel;

    void Start()
    {
        // Hide panels at the start of the stage
        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }

    public void StageCleared()
    {
        isGameOver = true;

        Debug.Log("Stage cleared!");

        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(true);
        }

        StartCoroutine(GoToNextStageAfterDelay());
    }

    public void PlayerLoses()
    {
        isGameOver = true;

        Debug.Log("Player lost!");

        if (losePanel != null)
        {
            Debug.Log("Activating lose panel.");
            losePanel.SetActive(true);
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void RestartStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextStage()
    {
        // For now this just restarts the same scene.
        // Later, we can change this to load the next level.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //this corroutine waits for 2 seconds before loading the next stage
    private IEnumerator GoToNextStageAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        LoadNextStage();
    }
   


}