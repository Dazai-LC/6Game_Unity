using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
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