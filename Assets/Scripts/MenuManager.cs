using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private string gameSceneName = "InGameScene";

    private void Start()
    {
        settingsPopup.SetActive(false);
    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickSettings()
    {
        settingsPopup.SetActive(true);
    }

    public void OnClickCloseSettings()
    {
        settingsPopup.SetActive(false);
    }
}