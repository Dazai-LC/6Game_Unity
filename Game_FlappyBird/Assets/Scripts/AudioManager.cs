using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;   // Phát tiếng động (vỗ cánh, ăn điểm, chết)
    public AudioSource musicSource; // Phát nhạc nền

    [Header("Audio Clips")]
    public AudioClip pressClip;
    public AudioClip releaseClip;
    public AudioClip scoreClip;
    public AudioClip diePipeClip;
    public AudioClip dieFallClip;

    private bool isMuted = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        // 🔥 TỰ ĐỘNG TÌM nếu fen quên kéo thả (Tránh triệt để lỗi UnassignedReferenceException)
        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }
    }

    // Các hàm phát sfx (Đã thêm kiểm tra an toàn != null để game không bao giờ crash)
    public void PlayPress() { if (sfxSource != null && pressClip != null) sfxSource.PlayOneShot(pressClip); }
    public void PlayRelease() { if (sfxSource != null && releaseClip != null) sfxSource.PlayOneShot(releaseClip); }
    public void PlayScore() { if (sfxSource != null && scoreClip != null) sfxSource.PlayOneShot(scoreClip); }
    public void PlayDiePipe() { if (sfxSource != null && diePipeClip != null) sfxSource.PlayOneShot(diePipeClip); }
    public void PlayDieFall() { if (sfxSource != null && dieFallClip != null) sfxSource.PlayOneShot(dieFallClip); }

    // --- ĐIỀU KHIỂN NHẠC NỀN ---
    public void PlayMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // Bật/Tắt toàn bộ âm thanh
    public void ToggleSound()
    {
        isMuted = !isMuted;
        AudioListener.pause = isMuted;
    }
}