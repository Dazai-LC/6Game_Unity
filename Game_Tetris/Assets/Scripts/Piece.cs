using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board;
    public TetrominoData tetrominoData;
    public float stepDelay = 1f;
    private float moveTimer;

    public int rotationIndex = 0;

    // --- 1. THÊM BIẾN CỜ GHI NHỚ THAO TÁC XOAY CUỐI CÙNG ---
    private bool lastMoveWasRotate = false;

    void Update()
    {
        if (Time.time >= moveTimer)
        {
            Move(Vector2Int.down);
            moveTimer = Time.time + stepDelay;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) { MoveLeft(); }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { MoveRight(); }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) { Move(Vector2Int.down); }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) { Rotate(); }
        else if (Input.GetKeyDown(KeyCode.Space)) { HardDrop(); }
        else if (Input.GetKeyDown(KeyCode.C)) { FindFirstObjectByType<Spawner>().Hold(); }
    }

    public void MoveLeft() { Move(Vector2Int.left); }
    public void MoveRight() { Move(Vector2Int.right); }

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

        // --- 2. NẾU DI CHUYỂN HOẶC RƠI THÀNH CÔNG -> HỦY CỜ XOAY ---
        lastMoveWasRotate = false;
        return true;
    }

    public void Rotate()
    {
        if (!this.enabled) return;

        int originalRotation = rotationIndex;
        Vector3 originalPos = transform.position;

        rotationIndex = (rotationIndex + 1) % 4;
        transform.eulerAngles -= new Vector3(0, 0, 90);

        bool wallKickSuccess = TestWallKicks(rotationIndex);

        // --- 3. NẾU XOAY (HOẶC LÁCH TƯỜNG) THÀNH CÔNG -> BẬT CỜ XOAY ---
        if (wallKickSuccess)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.moveSound);
            lastMoveWasRotate = true;
        }
        else
        {
            rotationIndex = originalRotation;
            transform.position = originalPos;
            transform.eulerAngles += new Vector3(0, 0, 90);
        }
    }

    private bool TestWallKicks(int targetRotationIndex)
    {
        Vector2Int[,] wallKicksData;
        if (tetrominoData.type == TetrominoType.O) return true;
        else if (tetrominoData.type == TetrominoType.I) wallKicksData = Data.WallKicksI;
        else wallKicksData = Data.WallKicksJLOSTZ;

        for (int i = 0; i < 5; i++)
        {
            Vector2Int translation = wallKicksData[targetRotationIndex, i];
            transform.position += new Vector3(translation.x, translation.y, 0);

            if (board.IsValidPosition(this.transform))
            {
                return true;
            }
            transform.position -= new Vector3(translation.x, translation.y, 0);
        }
        return false;
    }

    public void HardDrop()
    {
        if (!this.enabled) return;
        while (board.IsValidPosition(this.transform)) transform.position += Vector3.down;
        transform.position += Vector3.up;
        AudioManager.instance.PlaySFX(AudioManager.instance.hardDropSound);
        Lock();
    }

    // --- 4. HÀM KIỂM TRA LUẬT 3 GÓC CỦA T-SPIN ---
    private bool CheckTSpin()
    {
        // Phải là khối T và hành động cuối cùng phải là Xoay
        if (tetrominoData.type != TetrominoType.T || !lastMoveWasRotate) return false;

        int cornersOccupied = 0;
        Vector2Int pos = Vector2Int.RoundToInt(transform.position);

        // Tọa độ 4 góc xung quanh tâm khối T
        Vector2Int[] corners = {
            new Vector2Int(pos.x - 1, pos.y + 1),
            new Vector2Int(pos.x + 1, pos.y + 1),
            new Vector2Int(pos.x - 1, pos.y - 1),
            new Vector2Int(pos.x + 1, pos.y - 1)
        };

        foreach (Vector2Int corner in corners)
        {
            // Nếu góc đó nằm ngoài bàn cờ (chạm tường/đáy) hoặc có gạch đè lên -> Tính là 1 góc bị kẹt
            if (corner.x < 0 || corner.x >= board.width || corner.y < 0) cornersOccupied++;
            else if (corner.y < board.height && board.grid[corner.x, corner.y] != null) cornersOccupied++;
        }

        // Kẹt từ 3 góc trở lên -> Đạt chuẩn T-Spin!
        return cornersOccupied >= 3;
    }

    void Lock()
    {
        foreach (Transform child in transform)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            board.grid[pos.x, pos.y] = child;
        }
        this.enabled = false;

        // --- 5. BÓP CÒ KIỂM TRA VÀ TRUYỀN DỮ LIỆU T-SPIN CHO BOARD ---
        bool isTSpin = CheckTSpin();
        if (isTSpin)
        {
            Debug.Log("🔥 T-SPIN THÀNH CÔNG! Đã ăn điểm nhân phẩm!");
        }

        board.CheckForLines(isTSpin);
        FindFirstObjectByType<Spawner>().SpawnPiece();
    }
}