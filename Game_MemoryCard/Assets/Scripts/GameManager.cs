using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels Flow")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject gamePanel;
    public GameObject endPanel;
    public GameObject settingsPanel;

    [Header("Level Data System")]
    public LevelConfigSO[] allLevelConfigs;
    public Button[] levelButtons;

    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Sprite[] cardSprites;
    public Sprite backSprite;
    public Transform gridParent;

    [Header("In-Game HUD & Stats")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI finalScoreText;

    [Header("Hint System")]
    public Button hintButton;
    public Image hintCooldownImage;
    public float hintCooldownTime = 10f;

    [Header("Audio System")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip wrongClip;
    public AudioClip winClip;
    public TextMeshProUGUI soundBtnText;

    private LevelConfigSO currentLevel;
    private int matchedPairs = 0;
    private int totalPairsNeeded = 0;
    private List<Card> cards = new List<Card>();
    private Card firstCard;
    private Card secondCard;

    private bool canClick = true;
    private int score = 0;
    private float currentTime;
    private bool isGameActive = false;
    private int totalMoves = 0;
    private int currentCombo = 0;
    private bool isHintCooldown = false;
    private float hintTimer = 0f;
    private bool isSoundOn = true;

    void Start()
    {
        isSoundOn = PlayerPrefs.GetInt("SoundState", 1) == 1;
        UpdateSoundState();
        GoToMainMenu();
    }

    void Update()
    {
        if (!isGameActive) return;

        currentTime -= Time.deltaTime;
        UpdateTimerUI();
        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame(false);
        }

        if (isHintCooldown)
        {
            hintTimer -= Time.deltaTime;
            if (hintCooldownImage != null) hintCooldownImage.fillAmount = hintTimer / hintCooldownTime;

            if (hintTimer <= 0)
            {
                isHintCooldown = false;
                if (hintButton != null) hintButton.interactable = true;
                if (hintCooldownImage != null) hintCooldownImage.fillAmount = 0;
            }
        }
    }

    // ==================== HỆ THỐNG FLOW & CÀI ĐẶT ====================

    public void GoToMainMenu()
    {
        isGameActive = false;
        gamePanel.SetActive(false);
        endPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // 🔥 Bật lại nhạc nền khi ra Menu
        if (isSoundOn && bgmSource != null && !bgmSource.isPlaying) bgmSource.Play();
    }

    public void OpenLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        UpdateLevelLockState();

        // 🔥 Bật lại nhạc nền khi ở màn chọn Level
        if (isSoundOn && bgmSource != null && !bgmSource.isPlaying) bgmSource.Play();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt("SoundState", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateSoundState();
    }

    void UpdateSoundState()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !isSoundOn;
            // Nếu bật tiếng và đang không ở GamePanel thì cho phát BGM
            if (isSoundOn && !gamePanel.activeSelf && !bgmSource.isPlaying) bgmSource.Play();
        }
        if (sfxSource != null) sfxSource.mute = !isSoundOn;
        if (soundBtnText != null) soundBtnText.text = isSoundOn ? "Sound: ON" : "Sound: OFF";
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.Save();
        UpdateLevelLockState();
        Debug.Log("Đã reset tiến trình chơi!");
    }

    void UpdateLevelLockState()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null) levelButtons[i].interactable = (i + 1) <= unlockedLevel;
        }
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= allLevelConfigs.Length) return;

        // 🔥 Tắt nhạc nền khi vào màn chơi game
        if (bgmSource != null) bgmSource.Pause();

        currentLevel = allLevelConfigs[levelIndex];
        levelSelectPanel.SetActive(false);
        endPanel.SetActive(false);
        gamePanel.SetActive(true);

        if (timerText != null) timerText.gameObject.SetActive(true);
        if (movesText != null) movesText.gameObject.SetActive(true);
        if (comboText != null) comboText.gameObject.SetActive(true);
        if (hintButton != null) hintButton.gameObject.SetActive(true);
        if (scoreText != null) scoreText.gameObject.SetActive(true);

        currentTime = currentLevel.timeLimit;
        totalMoves = 0;
        currentCombo = 0;
        score = 0;
        matchedPairs = 0;
        UpdateStatsUI();

        foreach (Transform child in gridParent) Destroy(child.gameObject);
        cards.Clear();

        isHintCooldown = false;
        if (hintButton != null) hintButton.interactable = true;
        if (hintCooldownImage != null) hintCooldownImage.fillAmount = 0;

        SpawnCards();
        isGameActive = true;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (isSoundOn && sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // ==================== CORE GAMEPLAY ====================

    void SpawnCards()
    {
        if (cardPrefab == null || gridParent == null) return;

        GridLayoutGroup grid = gridParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = currentLevel.columns;
        }

        int totalCards = currentLevel.columns * currentLevel.rows;
        totalPairsNeeded = totalCards / 2;
        List<(Sprite sprite, int id)> spawnList = new List<(Sprite, int)>();

        for (int i = 0; i < totalPairsNeeded; i++)
        {
            Sprite s = cardSprites[i % cardSprites.Length];
            spawnList.Add((s, i)); spawnList.Add((s, i));
        }

        for (int i = 0; i < spawnList.Count; i++)
        {
            var temp = spawnList[i]; int rand = Random.Range(i, spawnList.Count);
            spawnList[i] = spawnList[rand]; spawnList[rand] = temp;
        }

        for (int i = 0; i < spawnList.Count; i++)
        {
            GameObject obj = Instantiate(cardPrefab, gridParent);
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.localScale = Vector3.one;
                rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f);
            }
            var data = spawnList[i];
            Card card = obj.GetComponent<Card>();
            card.Init(this, data.sprite, backSprite, data.id);
            cards.Add(card);
        }
    }

    public bool CanClick() => canClick && isGameActive;

    public void OnCardClicked(Card card)
    {
        if (!CanClick()) return;

        PlaySFX(flipClip);

        if (firstCard == null) firstCard = card;
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            totalMoves++;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        canClick = false;
        yield return new WaitForSeconds(0.5f);

        if (firstCard.id == secondCard.id)
        {
            PlaySFX(matchClip);
            currentCombo++;
            score += 10 + ((currentCombo - 1) * 5);
            firstCard.Hide(); secondCard.Hide();
            matchedPairs++;

            if (matchedPairs >= totalPairsNeeded) EndGame(true);
        }
        else
        {
            PlaySFX(wrongClip);
            currentCombo = 0; score -= 2;
            firstCard.Flip(); secondCard.Flip();
        }

        UpdateStatsUI();
        firstCard = null; secondCard = null;
        canClick = true;
    }

    public void UseHint()
    {
        if (!CanClick() || isHintCooldown) return;

        List<Card> unflippedCards = cards.FindAll(c => !c.IsFlipped() && c.gameObject.activeSelf);
        if (unflippedCards.Count >= 2)
        {
            Card target1 = unflippedCards[0];
            Card target2 = unflippedCards.Find(c => c.id == target1.id && c != target1);
            if (target2 != null)
            {
                StartCoroutine(HighlightHintCards(target1, target2));
                isHintCooldown = true; hintTimer = hintCooldownTime;
                if (hintButton != null) hintButton.interactable = false;
            }
        }
    }

    IEnumerator HighlightHintCards(Card c1, Card c2)
    {
        Image img1 = c1.GetComponent<Image>(); Image img2 = c2.GetComponent<Image>();
        if (img1 != null) img1.color = Color.yellow; if (img2 != null) img2.color = Color.yellow;
        yield return new WaitForSeconds(1f);
        if (img1 != null) img1.color = Color.white; if (img2 != null) img2.color = Color.white;
    }

    void UpdateStatsUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (movesText != null) movesText.text = "Moves: " + totalMoves;
        if (comboText != null) comboText.text = currentCombo > 1 ? "Combo x" + currentCombo : "";
    }

    void UpdateTimerUI()
    {
        if (timerText != null) timerText.text = "Time: " + Mathf.CeilToInt(currentTime) + "s";
    }

    void EndGame(bool isWin)
    {
        isGameActive = false;
        if (isWin) PlaySFX(winClip);

        if (timerText != null) timerText.gameObject.SetActive(false);
        if (movesText != null) movesText.gameObject.SetActive(false);
        if (comboText != null) comboText.gameObject.SetActive(false);
        if (hintButton != null) hintButton.gameObject.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);

        if (endPanel != null) endPanel.SetActive(true);

        if (finalScoreText != null)
        {
            string title = isWin ? "<color=#FFFF00>VICTORY!</color>" : "<color=#FF3333>TIME'S UP!</color>";
            finalScoreText.text = $"{title}\n\nScore: {score}\nMoves: {totalMoves}\nTime Left: {Mathf.CeilToInt(currentTime)}s";
        }

        // 🔥 NẾU THẮNG: Lưu mốc Level tự động dựa vào Index của mảng
        if (isWin)
        {
            int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
            // Lấy vị trí của Level hiện tại trong mảng (Level 1 đứng ô 0)
            int currentLevelIndex = System.Array.IndexOf(allLevelConfigs, currentLevel);
            int nextLevelTarget = currentLevelIndex + 2; // Thắng ô 0 -> Cần mở mốc 2

            if (nextLevelTarget > currentUnlocked)
            {
                PlayerPrefs.SetInt("UnlockedLevel", nextLevelTarget);
                PlayerPrefs.Save();
                Debug.Log("🔥 Đã mở khóa thành công Level: " + nextLevelTarget);
            }
        }
    }

    public void RestartGame()
    {
        int currentIndex = System.Array.IndexOf(allLevelConfigs, currentLevel);
        StartLevel(currentIndex >= 0 ? currentIndex : 0);
    }
}