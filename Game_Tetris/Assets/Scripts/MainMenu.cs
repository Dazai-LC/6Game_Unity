using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện bắt buộc để chuyển cảnh

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        //Chuyển sang GameScene
        SceneManager.LoadScene("GameScene");
    }
    public void QuitGame()
    {
        Debug.Log("Thoát game!");
        Application.Quit(); // Lệnh này chỉ chạy khi đã build thành game chạy thật
    }
}
