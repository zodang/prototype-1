using UnityEngine;
using UnityEngine.UI;

public class StartButtonUI : MonoBehaviour
{
    private enum TargetScene
    {
        Play,
        Lobby
    }

    [SerializeField] private Button startButton;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private TargetScene targetScene = TargetScene.Play;

    private void Awake()
    {
        if (startButton == null)
        {
            startButton = GetComponent<Button>();
        }
    }

    private void Start()
    {
        if (sceneLoader != SceneLoader.Instance)
        {
            sceneLoader = SceneLoader.Instance;
        }

        if (sceneLoader == null)
        {
            sceneLoader = GetComponent<SceneLoader>();
        }

        if (sceneLoader == null)
        {
            sceneLoader = FindFirstObjectByType<SceneLoader>();
        }

        if (startButton != null && sceneLoader != null)
        {
            startButton.onClick.AddListener(LoadTargetScene);
        }
    }

    private void OnDestroy()
    {
        if (startButton != null && sceneLoader != null)
        {
            startButton.onClick.RemoveListener(LoadTargetScene);
        }
    }

    private void LoadTargetScene()
    {
        if (targetScene == TargetScene.Lobby)
        {
            sceneLoader.LoadLobbyScene();
            return;
        }

        sceneLoader.LoadPlayScene();
    }
}
