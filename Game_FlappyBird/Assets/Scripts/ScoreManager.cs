using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("UI - In Game")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI noticeText; // Kéo Text thông báo (Combo/Achievement) vào đây

    [Header("UI - Menus")]
    public TextMeshProUGUI menuHighScoreText; // Kéo Text HighScore ở MainMenu
    public TextMeshProUGUI overScoreText;     // Kéo Text Score ở GameOver
    public TextMeshProUGUI overHighScoreText; // Kéo Text HighScore ở GameOver

    public int score { get; private set; } = 0;
    public int highScore { get; private set; } = 0;
    private int comboCounter = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Load HighScore từ bộ nhớ máy
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateMenuHighScore();

        if (noticeText != null) noticeText.gameObject.SetActive(false);
        ResetScore();
    }

    public void AddScore()
    {
        score++;
        comboCounter++;

        // --- CƠ CHẾ COMBO (Module 2) ---
        if (comboCounter >= 5)
        {
            score++; // Bonus thêm 1 điểm
            ShowNotice("COMBO x5! Bonus +1");
            comboCounter = 0;
        }

        // --- CƠ CHẾ ACHIEVEMENT (Module 2) ---
        if (score == 10) ShowNotice("ACHIEVEMENT: 10 PTS!");
        else if (score == 50) ShowNotice("ACHIEVEMENT: 50 PTS!");

        scoreText.text = score.ToString();
        AudioManager.instance.PlayScore();
    }

    public void ResetScore()
    {
        score = 0;
        comboCounter = 0;
        scoreText.text = "0";
        scoreText.gameObject.SetActive(true);
    }

    // Gọi khi thua game để cập nhật điểm kỷ lục
    public void SaveAndShowScores()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        // Hiển thị lên UI GameOver
        if (overScoreText != null) overScoreText.text = "Score:" + score;
        if (overHighScoreText != null) overHighScoreText.text = highScore.ToString();

        // Cập nhật luôn cho MainMenu lần sau
        UpdateMenuHighScore();
    }

    public void UpdateMenuHighScore()
    {
        if (menuHighScoreText != null) menuHighScoreText.text = highScore.ToString();
    }

    private void ShowNotice(string msg)
    {
        if (noticeText == null) return;
        noticeText.text = msg;
        noticeText.gameObject.SetActive(true);
        CancelInvoke("HideNotice");
        Invoke("HideNotice", 1.2f);
    }

    private void HideNotice()
    {
        if (noticeText != null) noticeText.gameObject.SetActive(false);
    }
}