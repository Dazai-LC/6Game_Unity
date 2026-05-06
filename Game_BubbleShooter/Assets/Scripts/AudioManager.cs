using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }

    [Header("Loa Phát(kéo AudioSource vào đây)")]
    public AudioSource bgmSource; // loa phát nhạc nền
    public AudioSource sfxSource; // loa phát hiệu ứng

    [Header("Nhạc nền(BGM)")]
    public AudioClip mainMenuBGM;
    public AudioClip menuLevelBGM;

    [Header("Hiệu ứng(SFX)")]
    public AudioClip shootSFX;
    public AudioClip fallSFX;
    public AudioClip pop3SFX;
    public AudioClip comboPopSFX;
    public AudioClip winSFX;
    public AudioClip loseFSX;

    private void Awake()
    {
        // phép thuật Singleton + Bất tử qua các scene
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ cục này sống mãi
        }
        else
        {
            Destroy(gameObject); // Đã có một bản thể rồi thì tự xóa để không bị vang tiếng
                                 
        }
    }

    //Các hàm phát nhạc nền
    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmClip == null) return;
        if(bgmSource.clip == bgmClip && bgmSource.isPlaying)
        {
            return; // Tránh phát lại từ đầu nếu đang chạy bài này rồi
        }
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
    
    //Các hàm phát hiệu ứng(FSX)
    public void PlaySFX(AudioClip clip)
    {
        if(clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    //Menu gọi nhanh cho code khác
    public void PlayShoot() => PlaySFX(shootSFX);
    public void PlayFall() => PlaySFX(fallSFX);
    public void PlayPop3() => PlaySFX(pop3SFX);
    public void PlayComboPop() => PlaySFX(comboPopSFX);
    public void PlayWin() => PlaySFX(winSFX);
    public void PlayLose() => PlaySFX(loseFSX);

}
