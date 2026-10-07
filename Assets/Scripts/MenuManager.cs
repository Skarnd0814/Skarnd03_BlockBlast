using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private string gameSceneName = "InGameScene";

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private AudioClip clickSfx;

    private void Start()
    {
        settingsPopup.SetActive(false);

        musicSlider.value = SoundManager.Instance.BgmVolume;
        soundSlider.value = SoundManager.Instance.SfxVolume;
    }

    public void OnClickPlay()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickSettings()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        settingsPopup.SetActive(true);
    }

    public void OnClickCloseSettings()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        settingsPopup.SetActive(false);
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance.SetBgmVolume(value);
    }

    public void OnSoundVolumeChanged(float value)
    {
        SoundManager.Instance.SetSfxVolume(value);
    }
}