using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board;
    public TetrominoData tetrominoData; // --- THÊM DÒNG NÀY ĐỂ NHỚ MẶT GẠCH ---
    public float stepDelay = 1f;
    private float moveTimer;

    public int rotationIndex = 0;

    void Update()
    {
        // 1. Logic rơi tự động theo thời gian
        if (Time.time >= moveTimer)
        {
            Move(Vector2Int.down);
            moveTimer = Time.time + stepDelay;
        }

        // 2. Code điều khiển (ĐÃ XÓA ĐOẠN BỊ NHÂN ĐÔI)
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { Move(Vector2Int.left); }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { Move(Vector2Int.right); }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) { Move(Vector2Int.down); }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) { Rotate(); }
        else if (Input.GetKeyDown(KeyCode.Space)) { HardDrop(); }
        else if (Input.GetKeyDown(KeyCode.C)) { FindFirstObjectByType<Spawner>().Hold(); }
    }

    // --- CÁC HÀM PUBLIC ĐỂ NGÓN TAY ĐIỀU KHIỂN ---
    public void MoveLeft() { Move(Vector2Int.left); }
    public void MoveRight() { Move(Vector2Int.right); }

    // Rơi chậm (Soft Drop): Ép nó nhích xuống 1 ô ngay lập tức và reset lại thời gian rơi
    public void SoftDrop()
    {
        if (Move(Vector2Int.down))
        {
            moveTimer = Time.time + stepDelay;
        }
    }

    bool Move(Vector2Int translation)
    {
        if (!this.enabled) return false;
        Vector3 newPos = transform.position;
        newPos.x += translation.x;
        newPos.y += translation.y;
        transform.position = newPos;

        if (!board.IsValidPosition(this.transform))
        {
            transform.position -= new Vector3(translation.x, translation.y, 0);
            if (translation == Vector2Int.down) Lock();
            return false;
        }
        return true;
    }

    // CHỈ CẦN THÊM CHỮ "public" VÀO ĐÂY LÀ HẾT ĐỎ!
    public void Rotate()
    {
        if (!this.enabled) return;

        // 1. Lưu lại trạng thái cũ đề phòng thất bại toàn tập
        int originalRotation = rotationIndex;
        Vector3 originalPos = transform.position;

        // 2. Xoay thử 90 độ và tăng index (0->1->2->3->0)
        rotationIndex = (rotationIndex + 1) % 4;
        transform.eulerAngles -= new Vector3(0, 0, 90);

        // 3. Mở sổ tay Wall Kick ra tra cứu
        bool wallKickSuccess = TestWallKicks(rotationIndex);
        //xoay thành công, thưởng nhạc:
        AudioManager.instance.PlaySFX(AudioManager.instance.moveSound);

        // 4. Nếu tra cả 5 phương án đều thất bại -> Bó tay, quay về như cũ!
        if (!wallKickSuccess)
        {
            rotationIndex = originalRotation;
            transform.position = originalPos;
            transform.eulerAngles += new Vector3(0, 0, 90);
        }
    }

    private bool TestWallKicks(int targetRotationIndex)
    {
        // Lấy đúng bảng Wall Kick tùy theo hình dáng gạch (O không cần xoay, I lấy bảng I, còn lại lấy chung)
        Vector2Int[,] wallKicksData;
        if (tetrominoData.type == TetrominoType.O) return true; // Cục vuông khỏi cần test
        else if (tetrominoData.type == TetrominoType.I) wallKicksData = Data.WallKicksI;
        else wallKicksData = Data.WallKicksJLOSTZ;

        // Thử lần lượt 5 phương án trong bảng
        for (int i = 0; i < 5; i++)
        {
            Vector2Int translation = wallKicksData[targetRotationIndex, i];

            // Dịch chuyển thử
            transform.position += new Vector3(translation.x, translation.y, 0);

            // Hỏi sếp Board xem chỗ này ngon chưa?
            if (board.IsValidPosition(this.transform))
            {
                return true; // Ngon! Chốt phương án này!
            }

            // Nếu không ngon, lùi lại vị trí cũ để test phương án tiếp theo
            transform.position -= new Vector3(translation.x, translation.y, 0);
        }

        return false; // Thất bại toàn tập
    }

    public void HardDrop()
    {
        if (!this.enabled) return;
        while (board.IsValidPosition(this.transform)) transform.position += Vector3.down;
        transform.position += Vector3.up;
        // khi chạm đất, thưởng nhạc:
        AudioManager.instance.PlaySFX(AudioManager.instance.hardDropSound);
        Lock();
    }
    void Lock()
    {
        foreach (Transform child in transform)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            board.grid[pos.x, pos.y] = child;
        }
        this.enabled = false;
        board.CheckForLines();
        FindFirstObjectByType<Spawner>().SpawnPiece();
    }
}