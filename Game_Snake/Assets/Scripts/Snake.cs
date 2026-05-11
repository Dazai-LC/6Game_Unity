using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Snake : MonoBehaviour
{
    private Vector2 direction = Vector2.up; // Hướng đi mặc định ban đầu (cho giống ảnh image_0)
    private List<Transform> segments = new List<Transform>();

    [Header("Setup (Drag & Drop Prefab)")]
    public Transform segmentPrefab; // Kéo Prefab SnakeBody (32px PPU=32) vào đây
    public int initialSize = 3; // Rắn bắt đầu với 3 đốt (đầu + 2 thân)

    [Header("Grid Movement (Advanced - cells/second)")]
    // Đề bài ban đầu dùng Interval 0.1s, tương đương moveSpeed = 10 (1s đi được 10 ô)
    // Dùng Speed trong FixedUpdate chính xác hơn cho grid movement.
    public float moveSpeed = 10f;
    private float moveStepTimer = 0f;
    private bool shouldMoveThisFixedFrame = false;

    // Các biến để tính toán Swipe (Di động)
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float minSwipeDistance = 50f;

    private FoodSpawner foodSpawner;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        //Tìm và lưu lại dùng dần
        foodSpawner = Object.FindFirstObjectByType<FoodSpawner>();

        ResetState();
    }

    private void Update()
    {
        HandleSwipeInput(); // Ưu tiên vuốt trên mobile
        HandleKeyboardInput(); // Giữ lại keyboard để test nhanh PC

        // Tính toán thời gian giữa 2 bước di chuyển
        moveStepTimer += Time.deltaTime;
        if (moveStepTimer >= (1f / moveSpeed))
        {
            shouldMoveThisFixedFrame = true;
            moveStepTimer -= (1f / moveSpeed);
        }
    }

    private void FixedUpdate()
    {
        // Chỉ di chuyển đúng nhịp độ lưới (Grid timestep)
        if (shouldMoveThisFixedFrame)
        {
            MoveContiguously();
            shouldMoveThisFixedFrame = false; // Reset cho nhịp sau
        }
    }

    private void MoveContiguously()
    {
        // 1. CHUẨN: Di chuyển thân rắn (Bắt đầu từ đuôi lên đầu)
        // Đốt sau 'copy' vị trí của đốt liền trước nó (trước khi đốt liền trước đó di chuyển)
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
            segments[i].rotation = segments[i - 1].rotation; // dòng này để thân xoay theo
        }

        // 2. Di chuyển đầu rắn theo hướng hiện tại (nhảy theo lưới tròn số)
        float x = Mathf.Round(transform.position.x) + direction.x;
        float y = Mathf.Round(transform.position.y) + direction.y;

        transform.position = new Vector2(x, y);

        //3. Xoay đầu rắn theo hướng đi (Dựa trên sprite gốc đang hướng xuống)
        if(direction == Vector2.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); //Giữ nguyên
        } else if(direction == Vector2.up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180); //Xoay ngược lại khi đi lên
        } else if(direction == Vector2.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90); //Xoay trái 90 độ
        } else if(direction == Vector2.left)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90); //Xoay phải 90 độ
        }
    }

    private void HandleSwipeInput()
    {
        // (Code vuốt mobile giữ nguyên như code trước fen đã dùng)
        // ... (phần code vuốt fen đã paste vào script trước đó)
        // *Chú ý:* Nhớ comment out code vuốt cũ ở script trước của fen nếu copy-paste script này đè lên.
        if (Input.touchCount > 0) { Touch touch = Input.GetTouch(0); if (touch.phase == TouchPhase.Began) { touchStartPos = touch.position; touchEndPos = touch.position; } else if (touch.phase == TouchPhase.Ended) { touchEndPos = touch.position; CheckSwipe(); } }
    }

    private void CheckSwipe() { Vector2 distance = touchEndPos - touchStartPos; if (distance.magnitude > minSwipeDistance) { if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y)) { if (distance.x > 0 && direction != Vector2.left) direction = Vector2.right; else if (distance.x < 0 && direction != Vector2.right) direction = Vector2.left; } else { if (distance.y > 0 && direction != Vector2.down) direction = Vector2.up; else if (distance.y < 0 && direction != Vector2.up) direction = Vector2.down; } } }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down) direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up) direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right) direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left) direction = Vector2.right;
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        // CHUẨN: Đặt đốt mới vào vị trí ĐUÔI đang đứng hiện tại
        segment.position = segments[segments.Count - 1].position;
        // *Sắp tới ta sẽ thêm logic rotate đốt thân theo hướng đuôi tại đây*
        segments.Add(segment);
    }

    // *** FIX LỖI "RẮN TÁCH ĐỐT" TỪ ĐÂY ***
    public void ResetState()
    {
        // 1. Xóa thân rắn cũ (chừa đầu)
        for (int i = 1; i < segments.Count; i++)
        {
            if (segments[i] != null) Destroy(segments[i].gameObject);
        }
        segments.Clear();
        segments.Add(transform); // Index 0 luôn là đầu rắn

        // 2. Reset vị trí và hướng
        transform.position = Vector3.zero;
        direction = Vector2.down ;
        transform.rotation = Quaternion.Euler(0, 0, 0); //Đầu khởi tạo hướng xuống dưới

        // 3. FIX: Sinh ra 2 đốt thân nối tiếp CHUẨN ra sau đầu rắn
        for (int i = 1; i < initialSize; i++)
        {
            Transform segment = Instantiate(segmentPrefab);

            // Logic CHUẨN: Đặt đốt hiện tại (i) ở vị trí của đốt (i-1) lùi lại 1 đơn vị
            // ngược với hướng di chuyển hiện tại. Đảm bảo chúng "chồng khít" vào lưới liền kề.
            Vector2 precedingPosition = segments[i - 1].position;
            Vector2 spreadBackward = precedingPosition - direction;
            segment.position = spreadBackward;

            //Đốt sinh ra ban đầu cũng copy luôn góc xoay của đầu
            segment.rotation = transform.rotation;

            segments.Add(segment);
        }
    }

    // Hàm public này để sau này Game Manager gọi khi ăn phải buff/debuff
    public void ChangeSpeed(float newSpeedCellsPerSecond)
    {
        moveSpeed = newSpeedCellsPerSecond;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //1. Rắn ăn mồi
        if(other.CompareTag("FoodNormal") || other.CompareTag("FoodSpeed") || other.CompareTag("FoodSlow"))
        {
            Grow(); // Rắn dài ra

            //Xử lý hiệu ứng tăng/giảm tốc độ tùy loại mội
            if (other.CompareTag("FoodNormal"))
            {
                ChangeSpeed(10f); //Tốc độ bình thường
                GameManager.Instance.AddScore(10);
            } else if (other.CompareTag("FoodSpeed"))
            {
                ChangeSpeed(15f); //Tăng tốc
                GameManager.Instance.AddScore(25);
            } else if (other.CompareTag("FoodSlow"))
            {
                ChangeSpeed(5f); //Giảm tốc
                GameManager.Instance.AddScore(5);
            }
            //Gọi FoodSpawner đẻ mồi mới(Tìm object có script FoodDpawner và gọi hàm SpawnFood)
            if(foodSpawner != null)
            {
                foodSpawner.SpawnFood();
            }

        // 2. Rắn đâm tường hoặc cắn vào thân
        } else if(other.CompareTag("Wall") || other.CompareTag("Body"))
        {
            GameManager.Instance.GameOver(); //Báo cho GameManager biết rắn đã die
            ResetState();
            ChangeSpeed(10f); //Reset lại tốc độ mặc định

            //Gọi đẻ lại mồi mới để tránh mồi cũ bị kẹp
            if (foodSpawner != null) foodSpawner.SpawnFood();
        }
    }
}