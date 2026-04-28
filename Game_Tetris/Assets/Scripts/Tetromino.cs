using UnityEngine;

// Enum này để định danh 7 hình dáng gạch
public enum TetrominoType { I, O, T, S, Z, J, L }

// Struct này để gói dữ liệu (Loại gạch + Hình ảnh Prefab của nó)
[System.Serializable]
public struct TetrominoData
{
    public TetrominoType type;
    public GameObject prefab;
}