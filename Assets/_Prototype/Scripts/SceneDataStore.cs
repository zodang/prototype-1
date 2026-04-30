using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataStore : MonoBehaviour
{
    private const string DefaultGemSaveDataResourcePath = "GemSaveData";

    [SerializeField] private string playSceneName = "PlayScene";
    [SerializeField] private string resultSceneName = "ResultScene";
    [SerializeField] private string lobbySceneName = "LobbyScene";
    [SerializeField] private GemSaveData gemSaveData;

    private static SceneDataStore instance;
    private GemManager currentGemManager;
    private bool hasActiveRun;
    private bool hasCommittedCurrentRun;

    public static SceneDataStore Instance => instance;
    public int CurrentRunGemCount { get; private set; }
    public int CurrentRunEnemyCount { get; private set; }
    public float CurrentRunElapsedSeconds { get; private set; }
    public int TotalGemCount { get; private set; }

    public event Action<int> OnCurrentRunGemCountChanged;
    public event Action<int> OnCurrentRunEnemyCountChanged;
    public event Action<float> OnCurrentRunElapsedSecondsChanged;
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
        LoadSaveDataIfNeeded();
        TotalGemCount = gemSaveData != null ? gemSaveData.LoadTotalGemCount() : 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void Update()
    {
        if (!hasActiveRun || hasCommittedCurrentRun)
        {
            return;
        }

        CurrentRunElapsedSeconds += Time.deltaTime;
        OnCurrentRunElapsedSecondsChanged?.Invoke(CurrentRunElapsedSeconds);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        Enemy.OnAnyDeath -= HandleEnemyDeath;
        UnbindGemManager();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnbindGemManager();

        if (scene.name == playSceneName)
        {
            BeginRun();
            Enemy.OnAnyDeath -= HandleEnemyDeath;
            Enemy.OnAnyDeath += HandleEnemyDeath;
            BindGemManager();
            return;
        }

        Enemy.OnAnyDeath -= HandleEnemyDeath;

        if (scene.name == resultSceneName || scene.name == lobbySceneName)
        {
            CommitCurrentRun();
        }

        OnCurrentRunGemCountChanged?.Invoke(CurrentRunGemCount);
        OnCurrentRunEnemyCountChanged?.Invoke(CurrentRunEnemyCount);
        OnCurrentRunElapsedSecondsChanged?.Invoke(CurrentRunElapsedSeconds);
        OnTotalGemCountChanged?.Invoke(TotalGemCount);
    }

    private void BeginRun()
    {
        hasActiveRun = true;
        hasCommittedCurrentRun = false;
        CurrentRunGemCount = 0;
        CurrentRunEnemyCount = 0;
        CurrentRunElapsedSeconds = 0f;
        OnCurrentRunGemCountChanged?.Invoke(CurrentRunGemCount);
        OnCurrentRunEnemyCountChanged?.Invoke(CurrentRunEnemyCount);
        OnCurrentRunElapsedSecondsChanged?.Invoke(CurrentRunElapsedSeconds);
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

    private void HandleEnemyDeath(Enemy enemy)
    {
        if (!hasActiveRun || hasCommittedCurrentRun)
        {
            return;
        }

        CurrentRunEnemyCount++;
        OnCurrentRunEnemyCountChanged?.Invoke(CurrentRunEnemyCount);
    }

    private void CommitCurrentRun()
    {
        if (!hasActiveRun || hasCommittedCurrentRun)
        {
            return;
        }

        TotalGemCount += CurrentRunGemCount;
        hasCommittedCurrentRun = true;
        gemSaveData?.SaveTotalGemCount(TotalGemCount);
        OnTotalGemCountChanged?.Invoke(TotalGemCount);
    }

    private void LoadSaveDataIfNeeded()
    {
        if (gemSaveData != null)
        {
            return;
        }

        gemSaveData = Resources.Load<GemSaveData>(DefaultGemSaveDataResourcePath);
    }
}
