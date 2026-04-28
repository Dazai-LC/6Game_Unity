using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Board board;
    public TetrominoData[] tetrominoes;
    public Vector3 spawnPosition = new Vector3(4f, 18f, 0f);

    public Piece activePiece;
    public Ghost ghost;

    // --- BIẾN MỚI THÊM: Kích thước của gạch trưng bày ---
    [Header("UI Preview Settings")]
    public float previewScale = 0.5f; // Mặc định thu nhỏ 50% (0.5)

    [Header("Next & Hold Settings")]
    public Transform[] nextPositions;
    private TetrominoData[] nextQueue = new TetrominoData[3];
    private GameObject[] nextPreviews = new GameObject[3];

    public Transform holdPosition;
    private TetrominoData holdData;
    private GameObject holdPreview;
    private bool canHold = true;
    private bool hasHeldPiece = false;

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            nextQueue[i] = tetrominoes[Random.Range(0, tetrominoes.Length)];
        }
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        TetrominoData data = nextQueue[0];

        nextQueue[0] = nextQueue[1];
        nextQueue[1] = nextQueue[2];
        nextQueue[2] = tetrominoes[Random.Range(0, tetrominoes.Length)];

        UpdateNextQueueUI();

        InstantiateAndSetup(data);
        canHold = true;
    }

    private void InstantiateAndSetup(TetrominoData data)
    {
        GameObject pieceObj = Instantiate(data.prefab, spawnPosition, Quaternion.identity);
        Piece piece = pieceObj.GetComponent<Piece>();
        piece.board = this.board;
        piece.stepDelay = this.board.currentFallSpeed; // ---Bơm tốc độ rơi theo level
        piece.tetrominoData = data;
        activePiece = piece;

        if (ghost != null) ghost.TrackPiece(piece);

        if (!this.board.IsValidPosition(pieceObj.transform))
        {
            Debug.Log("GAME OVER!");
            Destroy(pieceObj);
            //Gọi hàm GameOver bên Board
            this.board.GameOver();
        }
    }

    public void Hold()
    {
        if (Time.timeScale == 0f || !canHold) return;

        Destroy(activePiece.gameObject);

        if (!hasHeldPiece)
        {
            holdData = activePiece.tetrominoData;
            hasHeldPiece = true;
            SpawnPiece();
        }
        else
        {
            TetrominoData temp = holdData;
            holdData = activePiece.tetrominoData;
            InstantiateAndSetup(temp);
        }

        UpdateHoldUI();
        canHold = false;
    }

    private void UpdateNextQueueUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (nextPreviews[i] != null) Destroy(nextPreviews[i]);

            nextPreviews[i] = Instantiate(nextQueue[i].prefab, nextPositions[i].position, Quaternion.identity);
            nextPreviews[i].GetComponent<Piece>().enabled = false;

            // --- ÉP KÍCH THƯỚC NHỎ LẠI TẠI ĐÂY ---
            nextPreviews[i].transform.localScale = new Vector3(previewScale, previewScale, 1f);
        }
    }

    private void UpdateHoldUI()
    {
        if (holdPreview != null) Destroy(holdPreview);

        holdPreview = Instantiate(holdData.prefab, holdPosition.position, Quaternion.identity);
        holdPreview.GetComponent<Piece>().enabled = false;

        // --- ÉP KÍCH THƯỚC NHỎ LẠI TẠI ĐÂY ---
        holdPreview.transform.localScale = new Vector3(previewScale, previewScale, 1f);
    }

    public void OnRotateButtonClicked()
    {
        if (Time.timeScale == 0f || activePiece == null || !activePiece.enabled) return;
        activePiece.Rotate();
    }
}