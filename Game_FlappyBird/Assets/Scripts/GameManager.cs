using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Waiting,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState state;

    [Header("UI Panels")]
    public GameObject mainMenuUI; // Thay thế cho readyUI cũ
    public GameObject gameOverUI;
    public GameObject inGameUI;   // Kéo object chứa ScoreText in-game vào đây để ẩn/hiện cho đẹp

    public PipeSpawner pipeSpawner;
    public BirdController bird;

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        EnterWaiting();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandleEscape();
    }

    void HandleEscape() => Application.Quit();

    void EnterWaiting()
    {
        state = GameState.Waiting;

        mainMenuUI.SetActive(true);
        gameOverUI.SetActive(false);
        if (inGameUI != null) inGameUI.SetActive(false); // Ẩn điểm in-game khi ở menu

        pipeSpawner.enabled = false;
        CleanupPipes();
        bird.ResetBird();

        // Cập nhật điểm cao ở MainMenu
        if (ScoreManager.instance != null) ScoreManager.instance.UpdateMenuHighScore();
        //Bật nhạc nền khi ở màn hình chờ(Main menu)
        if (AudioManager.instance != null) AudioManager.instance.PlayMusic();
    }

    public void StartGame()
    {
        if (state != GameState.Waiting) return;

        state = GameState.Playing;

        mainMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        if (inGameUI != null) inGameUI.SetActive(true); // Bật điểm in-game

        pipeSpawner.enabled = true;
        bird.EnableControl();

        //Tắt nhạc nền khi bắt đầu vào game(Playing)
        if (AudioManager.instance != null) AudioManager.instance.StopMusic();
    }

    public void GameOver()
    {
        if (state != GameState.Playing) return;

        state = GameState.GameOver;

        gameOverUI.SetActive(true);
        if (inGameUI != null) inGameUI.SetActive(false); // Ẩn điểm in-game

        pipeSpawner.enabled = false;
        bird.DisableControl();

        CleanupPipes();

        // --- LƯU VÀ HIỂN THỊ ĐIỂM (Module 3) ---
        ScoreManager.instance.SaveAndShowScores();
    }

    void CleanupPipes()
    {
        // Dùng FindObjectsByType và chọn None để bỏ qua bước sort tốn tài nguyên
        Pipe[] pipes = FindObjectsByType<Pipe>(FindObjectsSortMode.None);
        foreach (Pipe pipe in pipes)
        {
            Destroy(pipe.gameObject);
        }
    }

    public void RestartGame()
    {
        //Thay vì load lại scene, ta reset trực tiếp và vào Playing luôn
        state = GameState.Playing;
        //1.Xử lý UI: Ẩn menu & game over,bật bảng tính điểm in-game
        mainMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        if (inGameUI != null) inGameUI.SetActive(true);
        //2.Reset điểm số về 0
        if (ScoreManager.instance != null) ScoreManager.instance.ResetScore();
        //3.Dọn sạch ống cũ và bật lại Spawner (Khi bật lại,OnEnable của Spawner sẽ tự reset tốc độ)
        CleanupPipes();
        pipeSpawner.enabled = true;
        //4.Đưa chim về vị trí gốc và bật điều khiển bay ngay lập tức
        bird.ResetBird();
        bird.EnableControl();
        //5.Đảm bảo tắt nhạc nền khi đang bay
        if (AudioManager.instance != null) AudioManager.instance.StopMusic();
    }

    public void GoToMainMenu()
    {
        //Quay lại trạng thái chờ, hiện menu, ẩn hết ống và reset chim
        EnterWaiting();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}