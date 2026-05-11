using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Refereces")]
    public TextMeshProUGUI bestScoreText;
    public GameObject settingsPanel;
    public TextMeshProUGUI soundButtonText;

    private bool isMuted = false;
    private void Start()
    {
        //1. Hiển thị điểm cao nhất
        int highScore = PlayerPrefs.GetInt("SNAKE_HIGHSCORE", 0);
        if (bestScoreText != null) bestScoreText.text = "BEST SCORE:" + highScore;
        //2. Load cài đặt âm thanh cũ
        isMuted = PlayerPrefs.GetInt("SNAKE_MUTE", 0) == 1;
        UpdateSoundUI();
    }

    //Các ham cho nút bấm menu chính
    public void StartGame()
    {
        //Load sang màn chơi chính(đảm bảo chuỗi này khớp với tên scene)
        SceneManager.LoadScene("GameScene");
    }
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    //Các hàm cho bảng cài đặt
    public void ToggleSound()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("SNAKE_MUTE",isMuted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateSoundUI();
    }
    private void UpdateSoundUI()
    {
        //Tắt /bật âm thanh tổng của toàn bộ game ngay lập tức
        AudioListener.volume = isMuted ? 0f : 1f;
        if(soundButtonText != null)
        {
            soundButtonText.text = isMuted ? "SOUND : OFF " : "SOUND ON";
        }
    }

    //Hàm set độ khó: 0 = Easy , 1 = normal , 2 = hard
    public void SetDifficulty(int difficultyIndex)
    {
        PlayerPrefs.SetInt("SNAKE_DIFFICULTY", difficultyIndex);
        PlayerPrefs.Save();

        Debug.Log("Đã lưu độ khó: " + difficultyIndex);
        CloseSettings(); // Chọn xong tự đóng bảng
    }
}
