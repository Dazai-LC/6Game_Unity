using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Biến static để các script khác có thể gọi dễ dàng(như gọi tổng đài)
    public static AudioManager instance;

    [Header("Dàn loa")]
    public AudioSource bgmSource;//Loa phát nhạc nền(Hát liên tục)
    public AudioSource sfxSource;//Loa phát tiếng động(kêu 1 lần rồi thôi)

    [Header("Băng đĩa(Audio Clips)")]
    public AudioClip moveSound;// Tiếng xê dịch, xoay
    public AudioClip hardDropSound; //Tiếng gạch rơi xuống đáy
    public AudioClip clearLineSound; // Tiếng nổ ăn hàng
    public AudioClip gameOverSound;// Tiếng tèo

    private void Awake()
    {
        // Setup để gọi từ xa
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Hàm gọi tiếng động
    public void PlaySFX(AudioClip clip)
    {
        if(clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    //Hàm tắt nhạc nên khi chết
    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
