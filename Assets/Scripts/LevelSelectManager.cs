using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("GameScene_2");
    }

    
}
