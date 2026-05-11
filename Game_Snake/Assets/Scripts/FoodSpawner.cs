using UnityEngine;

public class FoodSpawner : MonoBehaviour
{

    [Header("Khu vực sinh mồi")]
    public BoxCollider2D gridArea;

    [Header("Food Prefabs")]
    public GameObject foodNormalPrefab; //Mồi thường(+điểm)
    public GameObject foodSpeedPrefab; // Mồi tăng tốc
    public GameObject foodSlowPrefab; // Mồi giảm tốc

    private GameObject currentFood;

    private void Start()
    {
        //Sinh cục mồi đầu tiên khi vừa vào game
        SpawnFood();
    }

    public void SpawnFood()
    {
        //Xóa mồi cũ nếu đang có trên map (dùng cho lúc reset game)
        if(currentFood != null)
        {
            Destroy(currentFood);
        }

        Bounds bounds = gridArea.bounds;
        Vector2 spawnPosition = Vector2.zero;
        bool validPosition = false;
        int attempts = 0;

        //Tìm vị trí trống (tối đa 100 lần thử để tránh vòng lặp vô hạn gây treo game)
        while(!validPosition && attempts < 100)
        {
            //Random toạn độ trong vùng gridArea và làm tròn để luôn khớp với lưới 1x1
            float x = Mathf.Round(Random.Range(bounds.min.x, bounds.max.x));
            float y =Mathf.Round(Random.Range(bounds.min.y, bounds.max.y));
            spawnPosition = new Vector2(x, y);

            //Bắn một vòng tròn nhỏ(bán kính 0.2f) check xem tọa độ này có đang bị Rắn đè lên không
            Collider2D hit = Physics2D.OverlapCircle(spawnPosition, 0.2f);

            if(hit == null)
            {
                validPosition = true; //Chỗ trống , hợp lệ
            }
            attempts++;
        }

        //Tỷ lệ rớt đồ (random.value trả về từ 0.0 đến 1.0);
        //70% thường , 15% tốc độ , 15% chậm
        float randomType = Random.value;
        GameObject prefabToSpawn = foodNormalPrefab;

        if(randomType > 0.85f)
        {
            prefabToSpawn = foodSpeedPrefab;
        } else if(randomType > 0.70f)
        {
            prefabToSpawn = foodSlowPrefab;
        }
        //Sinh mồi mới ra bản đồ
        currentFood = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
