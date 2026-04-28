using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public Spawner spawner; // Để mượn viên gạch đang rơi

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isSwiping = false;

    // Khoảng cách tối thiểu để hệ thống công nhận đây là một cú "Vuốt" (tránh chạm nhầm)
    private float minSwipeDistance = 50f;

    void Update()
    {
        // Dùng GetMouseButton cho tiện vì nó nhận cả Chuột (trên PC) và Ngón tay (trên Điện thoại)
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            currentTouchPosition = Input.mousePosition;
            AnalyzeTouch();
        }
    }

    void AnalyzeTouch()
    {
        // Chặn luồng nếu không có gạch hoặc đang Game Over
        if (spawner.activePiece == null || !spawner.activePiece.enabled || Time.timeScale == 0f) return;

        Vector2 swipeDelta = currentTouchPosition - startTouchPosition;

        // KIỂM TRA: LÀ VUỐT HAY LÀ CHẠM?
        if (swipeDelta.magnitude > minSwipeDistance)
        {
            // --- XỬ LÝ VUỐT (SWIPE) ---
            // Kiểm tra xem vuốt dọc hay ngang (Ở đây mình chỉ quan tâm vuốt dọc theo đề bài)
            if (Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x))
            {
                if (swipeDelta.y > 0)
                {
                    // Vuốt Lên -> Ném thẳng xuống đáy!
                    spawner.activePiece.HardDrop();
                }
                else
                {
                    // Vuốt Xuống -> Rơi nhanh (Soft Drop)
                    spawner.activePiece.SoftDrop();
                }
            }
        }
        else
        {
            // --- XỬ LÝ CHẠM (TAP) ---
            // Nếu điểm chạm nằm ở nửa trái màn hình -> Sang trái
            if (currentTouchPosition.x < Screen.width / 2)
            {
                spawner.activePiece.MoveLeft();
            }
            // Điểm chạm nửa phải -> Sang phải
            else
            {
                spawner.activePiece.MoveRight();
            }
        }
    }
}