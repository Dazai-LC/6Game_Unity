using UnityEngine;

public static class Data
{
    // Bảng Wall Kick chuẩn SRS cho các khối J, L, O, S, T, Z
    // Mỗi hàng là 5 phương án test khi chuyển từ góc xoay này sang góc xoay khác
    public static readonly Vector2Int[,] WallKicksJLOSTZ = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,  1), new Vector2Int( 0, -2), new Vector2Int(-1, -2) }, // 0 -> 1 (0 -> 90 độ)
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, -1), new Vector2Int( 0,  2), new Vector2Int( 1,  2) }, // 1 -> 2
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,  1), new Vector2Int( 0, -2), new Vector2Int( 1, -2) }, // 2 -> 3
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int( 0,  2), new Vector2Int(-1,  2) }  // 3 -> 0
    };

    // Khối I (Thanh dài) kiêu kỳ nên có bảng Wall Kick riêng của nó
    public static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2, -1), new Vector2Int( 1,  2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1,  2), new Vector2Int( 2, -1) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2,  1), new Vector2Int(-1, -2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1, -2), new Vector2Int(-2,  1) }
    };
}