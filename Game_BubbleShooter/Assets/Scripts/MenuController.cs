using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public bool isMainMenu = true;
    public void Start()
    {
        if (isMainMenu)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.mainMenuBGM);
        }
        else
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.menuLevelBGM);
        }
    }

    // Mở màn chơi chính
    public void PlayGame()
    {
        Time.timeScale = 1f; // rã đông trước khi vào game
        SceneManager.LoadScene("MenuLevel");
    }
    // Hàm quay về Main Menu
    public void GoToMainMenu()
    {
        // Nhớ rã đông thời gian đề phòng kẹt (thói quen tốt)
        Time.timeScale = 1f;

        // Sửa lại đúng tên Scene Main Menu của fen vào đây nhé
        SceneManager.LoadScene("MainMenu");
    }

    // Sau này fen có thể thêm các hàm như QuitGame() ở đây
    public void QuitGame()
    {
        Debug.Log("Đã thoát game!");
        Application.Quit();
    }
}