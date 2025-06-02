using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject instructionsPanel; // Gán từ Inspector
    public GameObject instructionButton; 

    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
        instructionButton.SetActive(false);
    }

    public void HideInstructions()
    {
        instructionsPanel.SetActive(false);
        instructionButton.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //stop play mode
#endif
    }
}