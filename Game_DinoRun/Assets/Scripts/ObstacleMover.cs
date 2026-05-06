using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 5f; // tốc độ trôi, nên bằng với BG_Near
    public float leftBound = -15f; //Tọa độ x ngoài cùng bên trái để biến mất

    void Update()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver) return;
        //Di chuyển sang trái
        transform.Translate(Vector3.left * speed * GameManager.Instance.gameSpeedMultiplier * Time.deltaTime);
        // Nếu đi quá giới hạn màn hình -> tự động chết lâm sàng(trả về Pool)
        if(transform.position.x <= leftBound)
        {
            gameObject.SetActive(false);
        }
    }
}
