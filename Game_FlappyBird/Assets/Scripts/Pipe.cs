using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float moveSpeed = 2f; // Sẽ được PipeSpawner ghi đè
    float destroyX;

    void Awake()
    {
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        destroyX = -camHalfWidth - 2f;
    }

    void Update()
    {
        if (GameManager.instance.state == GameState.Playing)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}