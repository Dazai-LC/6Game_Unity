using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Cú pháp Singleton: Giúp các script khác(như snake) gọi GameManager cực dễ mà không cần Find
    public static GameManager Instance { get; private set; }
    [Header("Game Stats")]
    public int score = 0;
    public int level = 1;
    public int highScore = 0;

    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Kéo UI ScoreText vào đây
    public TextMeshProUGUI highScoreText; // Kéo UI HighScore vào đây
    public TextMeshProUGUI levelText; // Kéo UI LevelText vào đây

    //2 biến object chứa panel
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Game References")]
    public Snake snakeScript; // sẽ kéo thả đầu rắn vào đây

    [Header("Audio References")]
    public AudioSource audioSource; // kéo loa vào đây
    public AudioClip eatSound; // kéo file tiếng ăn mồi
    public AudioClip dieSound; // kéo file tiếng game over

    private void Awake()
    {
        //Khởi tạo Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //Lấy điểm cáo từ bộ nhớ : nếu chưa có thì mặc định là 0
        highScore = PlayerPrefs.GetInt("SNAKE_HIGHSCORE", 0);

        UpdateUI(); //Cập nhật chữ lên màn hình ngay khi vào game

        Time.timeScale = 1f;

        // =======đọc cài đặt từ menu=======
        //1.Áp dụng cài đặt âm thanh
        bool isMuted = PlayerPrefs.GetInt("SNAKE_MUTE",0 )==1;
        AudioListener.volume = isMuted ? 0f : 1f;
        //2.Áp dụng độ khó(Dynamic Base Speed)
        int difficulty = PlayerPrefs.GetInt("SNAKE_DIFFICULTY", 1); // mặc định là thường
        float baseSpeed = 10f; // tốc độ Normal

        if (difficulty == 0) baseSpeed = 7f; //Easy : bò chậm
        else if (difficulty == 2) baseSpeed = 14f; // Hard : bò cực nhanh
        //Truyền tốc độ ban đầu cho rắn
        if (snakeScript != null) snakeScript.ChangeSpeed(baseSpeed);

    }
    public void AddScore(int points)
    {
        score += points;

        //Kiểm tra và lưu điểm cao mới
        if(score > highScore)
        {
            highScore = score;
            //Lưu ngay vào bộ nhớ thiết bị PlayerPres
            PlayerPrefs.SetInt("SNAKE_HIGHSCORE", highScore);
            PlayerPrefs.Save(); //Bắt buộc gọi Save để an toàn dữ liệu
        }

        //Phát tiếng ăn mồi
        if(audioSource != null)
        {
            audioSource.PlayOneShot(eatSound);
        }

        //Logic lên cấp : Cứ được 50 điểm thì lên 1 level
        int newLevel = (score / 50) + 1;
        if(newLevel > level)
        {
            level = newLevel;
            IncreaseDifficulty();
        }

        UpdateUI(); // Cập nhật lại giao diện
    }

    private void IncreaseDifficulty()
    {
        //Dynamic Difficulty: Cứ lên 1 level , rắn bò nhanh hơn một chút
        //Tốc độ mặc định là 10 , mỗi levle + 1
        float newBaseSpeed = 10f + (level - 1) * 1.5f;
        snakeScript.ChangeSpeed(newBaseSpeed);

        Debug.Log("🎉 LÊN CẤP! Rắn đã tăng tốc lên: " + newBaseSpeed);
    }

    public void GameOver()
    {
        Debug.Log("💀 GAME OVER! Tổng điểm của bạn: " + score);
        Time.timeScale = 0f; //Dừng con rắn ngay lập tức

        //Phát tiếng game over
        if(audioSource != null)
        {
            audioSource.PlayOneShot(dieSound);
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    //Các hàm mới cho nút bấm UI
    public void PauseGame()
    {
        Time.timeScale = 0f; //Đóng băng thời gian
        if (pausePanel != null) pausePanel.SetActive(true); // Hiện bảng Pause
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // cho thời gian chạy lại
        if (pausePanel != null) pausePanel.SetActive(false); //Ản bảng Pause
    }

    public void RestartGame()
    {
        //Quan trọng: phải trả timeScale về 1 trước khi load scene, nếu không scene mới sẽ đứng im vĩnh viễn
        Time.timeScale = 1f;
        //Load lại đúng Scene hiện tại đang chơi
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Hàm chuyên dụng để làm mới các dòng text trên màn hình
    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (highScoreText != null) highScoreText.text = "Best: " + highScore;
        if (levelText != null) levelText.text = "Level: " + level;
    }

    public void GoToMainMenu()
    {
        //Quan trọng : phải trả thời gian về 1.0 trước khi thoát
        //Nếu không, khi quay lại Menu rồi vào lại Game,mọi thứ sẽ bị đóng băng
        Time.timeScale = 1f;
        //Load về Scene Menu(đảm bảo tên "MainMenu" khớp với từ khóa tìm kiếm
        SceneManager.LoadScene("MainMenu");
    }
}
