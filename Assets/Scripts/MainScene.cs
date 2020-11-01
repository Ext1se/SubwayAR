using UnityEngine;

public class MainScene : MonoBehaviour
{
    public void OpenScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
