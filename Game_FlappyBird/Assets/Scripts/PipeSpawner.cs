using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float baseSpawnInterval = 2f;
    public float minSpawnInterval = 1.2f; // Không sinh nhanh quá mức này
    public float minY = -1.5f;
    public float maxY = 1.5f;

    private float currentSpawnInterval;
    private float currentPipeSpeed = 2f;  // Tốc độ di chuyển gốc của ống
    private float timer;
    private float spawnX;

    void Start()
    {
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        spawnX = camHalfWidth + 2f;
    }

    void OnEnable()
    {
        currentSpawnInterval = baseSpawnInterval;
        currentPipeSpeed = 2f; // Reset tốc độ khi chơi lại
        timer = currentSpawnInterval;
    }

    void Update()
    {
        if (GameManager.instance.state != GameState.Playing) return;

        // --- DYNAMIC DIFFICULTY (Module 2) ---
        // Tăng dần độ khó theo thời gian thực (trôi qua mỗi giây)
        if (currentSpawnInterval > minSpawnInterval)
        {
            currentSpawnInterval -= Time.deltaTime * 0.015f; // Giảm thời gian chờ
        }
        currentPipeSpeed += Time.deltaTime * 0.02f;          // Tăng tốc độ bay của ống

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnPipe();
            timer = currentSpawnInterval;
        }
    }

    void SpawnPipe()
    {
        float y = Random.Range(minY, maxY);
        GameObject newPipe = Instantiate(pipePrefab, new Vector3(spawnX, y, 0f), Quaternion.identity);

        // Truyền tốc độ hiện tại cho ống vừa tạo
        Pipe pipeScript = newPipe.GetComponent<Pipe>();
        if (pipeScript != null)
        {
            pipeScript.moveSpeed = currentPipeSpeed;
        }
    }
}