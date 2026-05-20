using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelConfig", menuName = "MemoryMatch/Level Config")]
public class LevelConfigSO : ScriptableObject
{
    [Header("Thông tin Level")]
    // 🔥 Bắt buộc phải viết exacly: levelID (chữ l thường, ID viết hoa)
    public int levelID;

    [Header("Kích thước lưới")]
    public int columns = 2;
    public int rows = 3;

    [Header("Cơ chế")]
    public float timeLimit = 60f;
}