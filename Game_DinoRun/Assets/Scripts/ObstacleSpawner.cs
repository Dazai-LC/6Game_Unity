using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Cài đặt sinh đẻ")]
    public float baseSpawnRate = 2f; //Tốc độ ban đầu
    public float spawnRate = 2f; // cứ 2 giây sinh 1 vật
    public float minSpawnRate = 0.8f; //Giới hạn đẻ nhanh nhất(ép không được nhanh hơn mức này để tránh Dead-end)

    [Header("Thuật toán thông minh")]
    public int maxObstaclesInRow = 3; // Đẻ tối đa 3 cái liên tục sẽ phải nghỉ 1 chút
    private int currentObstacleCount = 0; //Biến đếm số lượng đã đẻ

    private float timer;

    void Update()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver) return;
        timer += Time.deltaTime;
        if(timer >= spawnRate)
        {
            timer = 0; //Reset đồng hồ
            //Gọi xin 1 chướng ngại vật từ kho
            GameObject obstacle = ObjectPooler.Instance.GetPooledObject();
            if(obstacle != null)
            {
                //Đặt vị trí của vật cản bằng đúng vị trí của Spawner
                obstacle.transform.position = transform.position;
                //Bật nó lên
                obstacle.SetActive(true);

                //========Thuật toán thông minh============
                currentObstacleCount++;
                //1. Xử lý "Quy tắc bù trừ" khi gặp chim
                //Đặt tag cho chim là Bird hoặc tên Prefab là Bird
                if(obstacle.CompareTag("Bird")|| obstacle.name.Contains("Bird"))
                {
                    //Lùi timer về số âm để lần đẻ tiếp theo mất nhiều thời gian hơn bình thường 0.5 giây
                    timer = -0.5f;
                }
                //2. Xử lý "Quy tắc thở" chống Dead - end
                //Nếu đã đẻ đủ 3 vật cản liên tiếp
                if(currentObstacleCount >= maxObstaclesInRow)
                {
                    // thì tạo một khoảng trống lớn(delay 1.5 giây)
                    timer = -1.5f;
                    currentObstacleCount = 0; //Reset bộ đếm cho đợt rải bom tiếp theo
                }
            }
            // đoạn logic tăng độ khó 
            // Lấy tốc độ gốc chia cho độ khó hiện tại của game
            float calculatedRate = baseSpawnRate / GameManager.Instance.gameSpeedMultiplier;
            //Ép spawnRate mới vào, nhưng không bao giờ được nhỏ hơn minSpawnRate
            spawnRate = Mathf.Max(calculatedRate, minSpawnRate);
        }
    }
}
