using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cài đặt màu sắc")]
    public Color dayColor = Color.white;  //Màu trắng cho ngày
    public Color nightColor = Color.gray; //Màu tối mở cho đêm

    [Header("Tốc độ trôi thời gian")]
    //Số càng nhỏ thì chuyển màu càng chậm(0.05 nghĩa là mất khoảng 20 giây để tối hẳn)
    public float cycleSpeed = 0.05f;

    private SpriteRenderer skyRenderer;
    private float timer = 0f;
    void Start()
    {
        //Tự động túm lấy cái component chứa hình ảnh bầu trời
        skyRenderer = GetComponent<SpriteRenderer>();
        //Đặt màu mặc định lúc mới vào game là ban ngày
        if (skyRenderer != null) skyRenderer.color = dayColor;
    }

    // Update is called once per frame
    void Update()
    {
        //Nếu game chưa bắt đầu hoặc khủng long đã tạch thì thời gian ngưng đọng
        if (GameManager.Instance != null && (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)) return;
        //Tăng bộ đếm thời gian
        timer += Time.deltaTime * cycleSpeed;
        //Bí quyết là đây: Mathf.PingPong sẽ làm cho tỉ lệ t chạy từ 0 lên 1 , rồi lại từ 1 lùi về 0(Sáng -> tối -> sáng)
        float t = Mathf.PingPong(timer, 1f);
        //Color.Lerp sẽ pha trộn 2 màu dựa theo tỉ lệ t
        if (skyRenderer != null)
            skyRenderer.color = Color.Lerp(dayColor, nightColor, t);
    }
}
