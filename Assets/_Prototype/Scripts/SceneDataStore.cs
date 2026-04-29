using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataStore : MonoBehaviour
{
    [SerializeField] private string playSceneName = "PlayScene";
    [SerializeField] private string resultSceneName = "ResultScene";
    [SerializeField] private string lobbySceneName = "LobbyScene";

    private static SceneDataStore instance;
    private GemManager currentGemManager;
    private bool hasActiveRun;
    private bool hasCommittedCurrentRun;

    public static SceneDataStore Instance => instance;
    public int CurrentRunGemCount { get; private set; }
    public int TotalGemCount { get; private set; }

    public event Action<int> OnCurrentRunGemCountChanged;
    public event Action<int> OnTotalGemCountChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnbindGemManager();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnbindGemManager();

        if (scene.name == playSceneName)
        {
            BeginRun();
            BindGemManager();
            return;
        }

        if (scene.name == resultSceneName || scene.name == lobbySceneName)
        {
            CommitCurrentRun();
        }

        OnCurrentRunGemCountChanged?.Invoke(CurrentRunGemCount);
        OnTotalGemCountChanged?.Invoke(TotalGemCount);
    }

    private void BeginRun()
    {
        hasActiveRun = true;
        hasCommittedCurrentRun = false;
        CurrentRunGemCount = 0;
        OnCurrentRunGemCountChanged?.Invoke(CurrentRunGemCount);
    }

    private void BindGemManager()
    {
        currentGemManager = FindFirstObjectByType<GemManager>();
        if (currentGemManager == null)
        {
            return;
        }

        currentGemManager.OnGemCountChanged += HandleGemCountChanged;
        HandleGemCountChanged(currentGemManager.GemCount);
    }

    private void UnbindGemManager()
    {
        if (currentGemManager == null)
        {
            return;
        }

        currentGemManager.OnGemCountChanged -= HandleGemCountChanged;
        currentGemManager = null;
    }

    private void HandleGemCountChanged(int gemCount)
    {
        CurrentRunGemCount = Mathf.Max(0, gemCount);
        OnCurrentRunGemCountChanged?.Invoke(CurrentRunGemCount);
    }

    private void CommitCurrentRun()
    {
        if (!hasActiveRun || hasCommittedCurrentRun)
        {
            return;
        }

        TotalGemCount += CurrentRunGemCount;
        hasCommittedCurrentRun = true;
        OnTotalGemCountChanged?.Invoke(TotalGemCount);
    }
}
