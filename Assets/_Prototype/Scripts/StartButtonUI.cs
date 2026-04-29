using UnityEngine;
using UnityEngine.UI;

public class StartButtonUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private SceneLoader sceneLoader;

    private void Awake()
    {
        if (startButton == null)
        {
            startButton = GetComponent<Button>();
        }

        if (sceneLoader == null)
        {
            sceneLoader = GetComponent<SceneLoader>();
        }

        if (startButton != null && sceneLoader != null)
        {
            startButton.onClick.AddListener(sceneLoader.LoadScene);
        }
    }

    private void OnDestroy()
    {
        if (startButton != null && sceneLoader != null)
        {
            startButton.onClick.RemoveListener(sceneLoader.LoadScene);
        }
    }
}
