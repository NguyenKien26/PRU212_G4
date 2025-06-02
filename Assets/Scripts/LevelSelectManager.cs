using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadLevel1()
    {
        SceneManager.LoadScene("GameScene_Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("GameScene");
    }

    
}
