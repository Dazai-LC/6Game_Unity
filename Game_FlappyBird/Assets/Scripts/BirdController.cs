using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float jumpForce = 5f;
    Rigidbody2D rb;
    bool canControl = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    void Update()
    {
        if (!canControl) return;

        //Xử lý ngóc đầu / chúi đầu tự nhiên theo Physics
        if (rb.simulated)
        {
            //Lấy vận tốc Y (linearVelocity.y chuẩn Unity 6000) nhân với hệ số xoay
            if (rb.simulated)
            {
                float angle = Mathf.Clamp(rb.linearVelocity.y * 8f, -90f, 30f);
                //Xoay object chim theo góc angle (trục z)
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        // Kiểm tra bấm chuột/Space HOẶC chạm ngón tay vào màn hình (Touch)
        bool isPressedDown = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) ||
                             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        bool isReleased = Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) ||
                          (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

        if (isPressedDown)
        {
            AudioManager.instance.PlayPress();
        }

        if (isReleased)
        {
            Flap();
            AudioManager.instance.PlayRelease();
        }
    }

    void Flap()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void EnableControl()
    {
        canControl = true;
        rb.simulated = true;
    }

    public void DisableControl()
    {
        canControl = false;
        rb.simulated = false;
    }

    public void ResetBird()
    {
        canControl = false;
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.instance.state != GameState.Playing) return;

        if (collision.gameObject.CompareTag("Pipe"))
        {
            AudioManager.instance.PlayDiePipe();
            GameManager.instance.GameOver();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            AudioManager.instance.PlayDieFall();
            GameManager.instance.GameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.instance.state != GameState.Playing) return;

        if (other.CompareTag("ScoreZone"))
        {
            // ScoreManager tự lo AddScore, tiếng kêu và Combo
            ScoreManager.instance.AddScore();
        }
    }
}