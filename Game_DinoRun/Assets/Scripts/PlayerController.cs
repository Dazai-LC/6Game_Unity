using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Máy trạng thái (State Machine)
    public enum PlayerState { Running, Jumping, Ducking, Dead }
    [Header("Trạng thái hiện tại")]
    public PlayerState currentState = PlayerState.Running;

    [Header("Cài đặt nhảy(jump)")]
    public float jumpForce = 15f;
    public float normalGravity = 3f; // trọng lực bình thường
    public float fallGravity = 6f;   // trọng lực khi thả tay sớm(rơi nhanh hơn)

    [Header("Kiểm tra chạm đất")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Cài đặt cúi(Ducking)")]
    public float duckDuration = 1f;  //Thời gian khủng long giữ tư thế cúi
    public float minSwipeDistance = 50f; // Khoảng cách vuốt tối thiểu để nhận diện là swipe

    private float duckTimer;
    private Vector2 touchStartPos; // lưu vị trí bắt đầu chạm tay

    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    private bool isGrounded;

    // Biến cho Animation
    private Animator anim;

    //Lưu kích thước collider ban đầu để phục hồi sau khi cúi
    private Vector2 normalColliderSize;
    private Vector2 normalColliderOffset;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();

        //Lấy animator
        anim = GetComponent<Animator>();

        rb.gravityScale = normalGravity;

        normalColliderSize = boxCol.size;
        normalColliderOffset = boxCol.offset;
    }

    void Update()
    {
        if (currentState == PlayerState.Dead) return; // Nếu chết thì dừng mọi logic
        //if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver) return; (tùy chọn cho màn dạo đầu)
        CheckGrounded();
        HandleInput();
        //Đếm ngược thời gian cúi
        if(currentState == PlayerState.Ducking)
        {
            duckTimer -= Time.deltaTime;
            if(duckTimer <= 0)
            {
                StandUp();
            }
        }
        // Gọi hàm cập nhật hình ảnh mỗi khung hình
        UpdateAnimationState();
    }

    // Hàm đồng bộ giữa ENUM trong code và INT trong Animator
    private void UpdateAnimationState()
    {
        //Nhắc lại quy ước : 0 =Run, 1 = Jump , 2 = Duck , 3 = Dead
        switch (currentState)
        {
            case PlayerState.Running:
                anim.SetInteger("stateInteger", 0);
                break;
            case PlayerState.Jumping:
                anim.SetInteger("stateInteger", 1);
                break;
            case PlayerState.Ducking:
                anim.SetInteger("stateInteger", 2);
                break;
            case PlayerState.Dead:
                anim.SetInteger("stateInteger", 3);
                break;
        }
    }

    private void CheckGrounded()
    {
        // Vẽ một vòng tròn nhỏ ở gót chân để quét xem có chạm đất hay không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //Nếu chạm đất và đang không cúi, chuyển về trạng thái chạy
        if(isGrounded && rb.linearVelocity.y <= 0.1f && currentState != PlayerState.Ducking)
        {
            currentState = PlayerState.Running;
        }
    }
    private void HandleInput()
    {
        //Nhận diện từ bàn phím(PC)
        //Bấm phím space hoặc mũi tên lên để nhảy
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded && currentState != PlayerState.Ducking) Jump();
        }
        //Thả phím nhảy ra sớm thì rơi nhanh hơn
        if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (rb.linearVelocity.y > 0) rb.gravityScale = fallGravity;
        }
        //Bấm phím V hoặc mũi tên xuống để cúi
        if(Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isGrounded && currentState != PlayerState.Ducking) Duck();
        }
        //Nhận diện Input từ chuột(test trên PC)
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            //Nếu click nhanh (không vuốt) -> nhảy
            if(isGrounded && currentState != PlayerState.Ducking)
            {
                Jump();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipeDelta = (Vector2)Input.mousePosition - touchStartPos;
            //Nếu vuốt xuống đủ dài ->Cúi
            if(swipeDelta.y < -minSwipeDistance && isGrounded && currentState != PlayerState.Ducking)
            {
                Duck();
            } else if(rb.linearVelocity.y > 0)
            {
                rb.gravityScale = fallGravity;
            }
        }
        //Nhận diện Input từ màn hình cảm ứng(mobile)
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                if(isGrounded && currentState != PlayerState.Ducking)
                {
                    Jump();
                }
            } else if(touch.phase == TouchPhase.Ended)
            {
                Vector2 swipeDelta = touch.position - touchStartPos;
                if(swipeDelta.y < -minSwipeDistance && isGrounded && currentState != PlayerState.Ducking)
                {
                    Duck();
                } else if(rb.linearVelocity.y > 0)
                {
                    rb.gravityScale = fallGravity;
                }
            }
        }
    }
    
    private void Jump()
    {
        currentState = PlayerState.Jumping;
        rb.gravityScale = normalGravity; // Reset trọng lực

        //Đưa vận tốc trục y về 0 trước khi nhảy để lực nảy luôn nhất quán
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        //Thêm lực đẩy lên
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Duck()
    {
        currentState = PlayerState.Ducking;
        duckTimer = duckDuration;
        //Thu nhỏ chiều cao của BoxCollider đi một nửa
        boxCol.size = new Vector2(normalColliderSize.x, normalColliderSize.y / 2);
        //Hạ offset y xuống để đáy của collider vẫn chạm đất
        boxCol.offset = new Vector2(normalColliderOffset.x, normalColliderOffset.y - (normalColliderSize.y / 4));
    }

    private void StandUp()
    {
        //Trả lại trạng thái chạy bình thường và kích thước gốc của Collider
        currentState = PlayerState.Running;
        boxCol.size = normalColliderSize;
        boxCol.offset = normalColliderOffset;
    }

    private void OnDrawGizmosSelected()
    {
        if(groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Kiểm tra xem thứ chạm vào có phải mang mác "Obstacle" không
        if (collision.CompareTag("Obstacle"))
        {
            //Nếu đúng, và khủng long chưa chết, thì cho nó oẳng
            if(currentState != PlayerState.Dead)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        currentState = PlayerState.Dead;
        //Khóa cứng trọng lực và vận tốc để khủng long rơi oạch xuống đất hoặc đứng yên
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = normalGravity;
        //Kích hoạt ngay lập tức animation chết(state = 3)
        UpdateAnimationState();
        GameManager.Instance.GameOver();
        Debug.Log("Game OVer!!! Khủng long hóa thạch.");
    }

}
