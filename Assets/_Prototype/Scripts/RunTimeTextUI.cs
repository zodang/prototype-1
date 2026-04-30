using TMPro;
using UnityEngine;

public class RunTimeTextUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    private void Awake()
    {
        if (timeText == null)
        {
            timeText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        SubscribeAndUpdate();
    }

    private void Start()
    {
        SubscribeAndUpdate();
    }

    private void OnDisable()
    {
        if (SceneDataStore.Instance == null)
        {
            return;
        }

        SceneDataStore.Instance.OnCurrentRunElapsedSecondsChanged -= UpdateTime;
    }

    private void SubscribeAndUpdate()
    {
        if (SceneDataStore.Instance == null)
        {
            return;
        }

        SceneDataStore.Instance.OnCurrentRunElapsedSecondsChanged -= UpdateTime;
        SceneDataStore.Instance.OnCurrentRunElapsedSecondsChanged += UpdateTime;
        UpdateTime(SceneDataStore.Instance.CurrentRunElapsedSeconds);
    }

    private void UpdateTime(float elapsedSeconds)
    {
        if (timeText == null)
        {
            return;
        }

        int totalSeconds = Mathf.FloorToInt(Mathf.Max(0f, elapsedSeconds));
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}
