using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Board board;
    public Piece trackingPiece; // Bám đuôi viên gạch nào?

    private Transform[] cells; // 4 ô gạch con của Ghost

    void Awake()
    {
        // Vừa vào game, tự động nặn ra 4 cục gạch tàng hình làm "đệ tử"
        cells = new Transform[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject cellObj = new GameObject("GhostCell");
            cellObj.transform.SetParent(this.transform); // Nhận Ghost làm cha

            SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Bricks"; // Chui vào đúng layer để không bị đen thui

            cells[i] = cellObj.transform;
        }
    }

    // Hàm này Spawner sẽ gọi để giao nhiệm vụ theo dõi
    public void TrackPiece(Piece piece)
    {
        this.trackingPiece = piece;

        for (int i = 0; i < 4; i++)
        {
            // Lấy hình ảnh từ viên gạch thật ốp sang Ghost
            Transform realChild = piece.transform.GetChild(i);
            SpriteRenderer ghostSr = cells[i].GetComponent<SpriteRenderer>();

            ghostSr.sprite = realChild.GetComponent<SpriteRenderer>().sprite;

            // Chỉnh màu cho mờ đi (Tạo hiệu ứng bóng ma - Alpha 30%)
            ghostSr.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    // Dùng LateUpdate để chạy SAU khi viên gạch thật đã di chuyển xong
    void LateUpdate()
    {
        // Nếu không có gạch thật (đang Pause hoặc Game Over) -> Tàng hình
        if (trackingPiece == null || !trackingPiece.enabled)
        {
            for (int i = 0; i < 4; i++) cells[i].gameObject.SetActive(false);
            return;
        }

        // Hiện thân
        for (int i = 0; i < 4; i++) cells[i].gameObject.SetActive(true);

        // 1. Copy y xì đúc vị trí, góc xoay và cấu hình của gạch thật
        transform.position = trackingPiece.transform.position;
        transform.rotation = trackingPiece.transform.rotation;
        for (int i = 0; i < 4; i++)
        {
            cells[i].localPosition = trackingPiece.transform.GetChild(i).localPosition;
        }

        // 2. Thuật toán "Phi thân dò đường"
        while (board.IsValidPosition(this.transform))
        {
            transform.position += Vector3.down; // Rớt 1 ô
        }

        // 3. Vòng lặp dừng khi đã lẹm vào tường/gạch -> Lùi lại 1 bước cho chuẩn!
        transform.position += Vector3.up;
    }
}