using UnityEngine;

[CreateAssetMenu(fileName = "GemSaveData", menuName = "Prototype/Save Data/Gem Save Data")]
public class GemSaveData : ScriptableObject
{
    [SerializeField] private string totalGemCountKey = "Prototype.TotalGemCount";
    [SerializeField] private int defaultTotalGemCount = 0;

    public int LoadTotalGemCount()
    {
        return Mathf.Max(0, PlayerPrefs.GetInt(totalGemCountKey, defaultTotalGemCount));
    }

    public void SaveTotalGemCount(int totalGemCount)
    {
        PlayerPrefs.SetInt(totalGemCountKey, Mathf.Max(0, totalGemCount));
        PlayerPrefs.Save();
    }

    public void ResetTotalGemCount()
    {
        SaveTotalGemCount(defaultTotalGemCount);
    }
}
