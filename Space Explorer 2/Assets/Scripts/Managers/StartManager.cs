using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void StartGame()
    {
        GameStateManager.instance.ResetGameState();
        SceneManager.LoadScene("GameScene");
    }


}