using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Khai báo dùng Text UI

public class Board : MonoBehaviour
{
    public int width = 10;
    public int height = 20;
    public Transform[,] grid;
    public GameObject gameOverPanel;

    // Biến UI
    public int score = 0;
    public TextMeshProUGUI scoreText;
    // BIẾN MỚI CHO ĐIỂM CAO NHẤT
    public TextMeshProUGUI maxScoreText;
    private int highScore = 0;
    //Biến Level
    [Header("Level Settings")]
    public int totalLinesCleared = 0;
    public int level = 1;
    public float currentFallSpeed = 1f; // Tốc độ rơi khởi điểm
    public TextMeshProUGUI levelText;   // Nơi hiển thị level lên UI

    private bool isPaused = false;

    void Awake()
    {
        grid = new Transform[width, height];

        // 1. Vừa mở game: Tải Kỷ lục cũ từ bộ nhớ máy lên
        highScore = PlayerPrefs.GetInt("HighScore", 0); // 0 là điểm mặc định nếu chưa chơi bao giờ
        if(levelText != null)
        {
            levelText.text.ToString();
        }
        if (maxScoreText != null)
        {
            maxScoreText.text = highScore.ToString();
        }

        UpdateScore(0); // Set điểm hiện tại về 0
    }

    // --- 4 HÀM BỊ ÔNG LỠ TAY XÓA ĐÃ ĐƯỢC PHỤC HỒI ---
    public bool IsValidPosition(Transform piece)
    {
        foreach (Transform child in piece)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            if (pos.x < 0 || pos.x >= width || pos.y < 0) return false;
            if (pos.y < height && grid[pos.x, pos.y] != null) return false;
        }
        return true;
    }

    private bool IsLineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null) return false;
        }
        return true;
    }

    private void ClearLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject);
            AudioManager.instance.PlaySFX(AudioManager.instance.clearLineSound);
            grid[x, y] = null;
        }
    }

    private void MoveAllRowsDown(int clearedY)
    {
        for (int y = clearedY; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }
    }
    // ------------------------------------------------

    // Hàm Tổng Quản Tính Điểm
    public void CheckForLines(bool isTSpin = false)
    {
        int linesCleared = 0;
        int y = 0;

        while (y < height)
        {
            if (IsLineFull(y))
            {
                ClearLine(y);
                MoveAllRowsDown(y + 1);
                linesCleared++;
            }
            else
            {
                y++;
            }
        }

        if (linesCleared > 0)
        {
            //---Logic tăng level---
            totalLinesCleared += linesCleared;
            //Cứ xóa được 10 dòng thì lên 1 level
            int newLevel = (totalLinesCleared / 10) + 1;
            if(newLevel > level)
            {
                level = newLevel;
                //Công thức ép xung: Mỗi level giảm 10% thời gian rơi
                currentFallSpeed = Mathf.Max(0.1f, 1f - ((level - 1) * 0.1f));
                if(levelText != null)
                {
                    levelText.text = level.ToString();
                    Debug.Log("Level up! Tốc độ mới: " + currentFallSpeed);
                }
            }
            if (linesCleared == 1) UpdateScore(100);
            else if (linesCleared == 2) UpdateScore(300);
            else if (linesCleared == 3) UpdateScore(500);
            else if (linesCleared >= 4) UpdateScore(800);
        }
    }

    private void UpdateScore(int pointsToAdd)
    {
        // Cộng điểm hiện tại
        score += pointsToAdd;
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        // KIỂM TRA PHÁ KỶ LỤC
        if (score > highScore)
        {
            highScore = score; // Cập nhật kỷ lục mới

            // Lưu ngay vào bộ nhớ máy!
            PlayerPrefs.SetInt("HighScore", highScore);

            // Hiện lên màn hình
            if (maxScoreText != null)
            {
                maxScoreText.text = highScore.ToString();
            }
        }
    }

    public void OnPauseButtonClicked()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
    
    public void GameOver()
    {
        //Bật cái bảng Game Over lên che màn hình
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            AudioManager.instance.StopBGM();
            AudioManager.instance.PlaySFX(AudioManager.instance.gameOverSound);
            //Đóng băng thời gian:
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        //Quan trọng nhất: Phải rã đông thời gian trước khi load lại, nếu không game mới cũng bị đóng băng!
        Time.timeScale = 1f;
        //Tải lại chính cái Scene hiện tại đang chơi:
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        // luôn giã đông thời gian phòng trường hợp người chơi thoát lúc gameOVer
        Time.timeScale = 1f;
        // Trở về MainMenuScene
        SceneManager.LoadScene("MainMenuScene");

    }
}