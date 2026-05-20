using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public float speed = 1.5f;
    private float repeatWidth;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        // TỰ ĐỘNG tính độ rộng dựa trên SpriteRenderer của ảnh
        if (GetComponent<SpriteRenderer>() != null)
        {
            repeatWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        else
        {
            repeatWidth = 10f; // Giá trị dự phòng
        }
    }

    void Update()
    {
        // Kiểm tra đúng trạng thái Playing mới trôi
        if (GameManager.instance != null && GameManager.instance.state == GameState.Playing)
        {
            // Di chuyển sang trái
            transform.position += Vector3.left * speed * Time.deltaTime;

            // Nếu trôi qua hết độ rộng ảnh thì reset về vị trí cũ
            if (transform.position.x < startPosition.x - repeatWidth)
            {
                transform.position = startPosition;
            }
        }
    }
}