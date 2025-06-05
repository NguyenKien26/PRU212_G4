using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject instructionsPanel;
    public GameObject instructionButton;
    public GameObject settingPopup;

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
    public void ShowSettingPopup()
    {
        settingPopup.SetActive(true);
    }
    public void QuitGame()
    {
      Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}