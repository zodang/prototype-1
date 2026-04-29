using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName = "PlayScene";

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
