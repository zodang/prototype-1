using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string lobbySceneName = "LobbyScene";
    [SerializeField] private string playSceneName = "PlayScene";
    [SerializeField] private string resultSceneName = "ResultScene";
    [SerializeField] private float resultTransitionDelay = 2f;

    private static SceneLoader instance;
    private PlayerHp currentPlayerHp;
    private bool isGameOver;

    public static SceneLoader Instance => instance;

    private void OnValidate()
    {
        resultTransitionDelay = Mathf.Max(0f, resultTransitionDelay);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        BindPlayerHpIfNeeded();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnbindPlayerHp();
    }

    public void LoadScene()
    {
        LoadScene(playSceneName);
    }

    public void LoadLobbyScene()
    {
        LoadScene(lobbySceneName);
    }

    public void LoadPlayScene()
    {
        LoadScene(playSceneName);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene(sceneName);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isGameOver = false;
        BindPlayerHpIfNeeded();
    }

    private void BindPlayerHpIfNeeded()
    {
        UnbindPlayerHp();

        currentPlayerHp = FindFirstObjectByType<PlayerHp>();
        if (currentPlayerHp == null)
        {
            return;
        }

        currentPlayerHp.OnHpEnded += HandlePlayerHpEnded;

        if (!currentPlayerHp.IsAlive)
        {
            HandlePlayerHpEnded();
        }
    }

    private void UnbindPlayerHp()
    {
        if (currentPlayerHp == null)
        {
            return;
        }

        currentPlayerHp.OnHpEnded -= HandlePlayerHpEnded;
        currentPlayerHp = null;
    }

    private void HandlePlayerHpEnded()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        Time.timeScale = 0f;
        StartCoroutine(LoadResultSceneRoutine());
    }

    private IEnumerator LoadResultSceneRoutine()
    {
        yield return new WaitForSecondsRealtime(resultTransitionDelay);

        LoadScene(resultSceneName);
    }
}
