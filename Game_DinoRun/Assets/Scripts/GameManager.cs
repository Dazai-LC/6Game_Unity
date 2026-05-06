using UnityEngine;
using TMPro; // Khai báo thư viện UI mới
using UnityEngine.SceneManagement; // Quản lý Scene

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("trạng thái game")]
    public bool isGameOver = false;
    public bool isGameStarted = false;

    [Header("Hệ thống điểm số")]
    public float score;
    public TextMeshProUGUI scoreText; //Biến để chứa cái Text trên màn hình

    [Header("Hệ thống độ khó")]
    public float gameSpeedMultiplier = 1f; //Hệ số tốc độ gốc 1x
    public float speedIncreaseRate = 0.02f; //Tốc độ tăng dần đều(0.02 mỗi giây)

    [Header("Giao diện UI")]
    public GameObject gameOverPanel; // Hộp chứa GameOverPanel
    public GameObject startPanel;   // Hộp chứa StartPanel
    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //Kịch bản 1: game chưa bắt đầu
        if (!isGameStarted)
        {
            //Bắt sự kiện người chơi click chuột (PC) hoặc chạm màn hình( mobile)
            if(Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                isGameStarted = true;
                if (startPanel != null) startPanel.SetActive(false);
            }
            return; // Chặn luôn ở đây, không cho mây trôi hay tính điểm bên dươi
        }
        //Kịch bản 2: game over thì dừng
        if (isGameOver) return;
        //Kịch bản 3: Game đang chạy
        score += Time.deltaTime * 10f;
        if(scoreText != null)
        {
            scoreText.text = "Điểm: " + Mathf.FloorToInt(score).ToString();
        }
        gameSpeedMultiplier += speedIncreaseRate * Time.deltaTime;
    }
    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("GameManager đã khóa thế giới");
        //Bật Panel khi chết
        StartCoroutine(ShowGameOverPanelDelayed());
    }

    //Coroutine: Hàm đặc biệt cho phép tạm dừng dòng thời gian của code
    private System.Collections.IEnumerator ShowGameOverPanelDelayed()
    {
        yield return new WaitForSeconds(1.5f);
        //Sau khi hết 1.5 giây, mới thực hiện bật màn hình GameOverPanel
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    //Nút chơi lại
    public void RestartGame()
    {
        //Tải lại Scene hiện tại từ đầu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
