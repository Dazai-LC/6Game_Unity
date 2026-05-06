using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    [Header("Tốc độ cuộn (Sky: 0.5 , Far: 2 , Near: 5")]
    public float scrollSpeed;

    private float spriteWidth;
    private Vector2 startPosition;
    void Start()
    {
        //Lưu lại vị trí xuất phát ban đầu
        startPosition = transform.position;
        //Tự động đo xem tấm ảnh này rộng bao nhiêu
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Nếu tổng tư lệnh(GameManager) bảo game over rồi thì nghỉ cuộn
        if (GameManager.Instance != null && (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)) return;
        //Trôi đều đặn về bên trái
        transform.Translate(Vector3.left * scrollSpeed * GameManager.Instance.gameSpeedMultiplier * Time.deltaTime);
        //Nếu tấm ảnh đã trôi qua trái đúng bằng chiều rộng của nó thì:
        if(transform.position.x <= startPosition.x - spriteWidth)
        {
            // lập tức dịch chuyển nó về lại vị trí xuất phát
            transform.position = startPosition;
        }
    }
}
