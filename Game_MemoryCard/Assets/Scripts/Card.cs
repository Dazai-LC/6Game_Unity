using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int id;
    public Sprite frontSprite;
    public Sprite backSprite;

    private Image cardImage;  // Dùng Image thay cho SpriteRenderer
    private Button cardButton; // Dùng Button để nhận touch/click
    private bool isFlipped = false;
    private GameManager gameManager;

    public void Init(GameManager gm, Sprite front, Sprite back, int cardId)
    {
        gameManager = gm;
        frontSprite = front;
        backSprite = back;
        id = cardId;

        cardImage = GetComponent<Image>();
        cardImage.sprite = backSprite;

        // ======= Gán sự kiện OnClick bằng code======
        cardButton = GetComponent<Button>();
        if(cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClick);
        }
    }

    //===== Hàm này thay thế hoàn toàn cho Update() và Raycast cũ
    private void OnCardClick()
    {
        if(!isFlipped && gameManager.CanClick())
        {
            Flip();
            gameManager.OnCardClicked(this);
        }
    }

    public void Flip()
    {
        isFlipped = !isFlipped;
       cardImage.sprite = isFlipped ? frontSprite : backSprite;
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    // 🔥 Ẩn card khi match (có animation)
    public void Hide()
    {
        StartCoroutine(HideAnim());
    }

    IEnumerator HideAnim()
    {
        float t = 0;
        Vector3 start = transform.localScale;

        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            transform.localScale = Vector3.Lerp(start, Vector3.zero, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}