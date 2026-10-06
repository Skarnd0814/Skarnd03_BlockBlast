using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleManage : MonoBehaviour
{
    [SerializeField] private TMP_Text touchText;
    [SerializeField] private float blinkSpeed = 1.5f;
    [SerializeField] private string nextSceneName = "MenuScene";

    private void Update()
    {
        BlinkTouchText();

        if (Pointer.current != null && 
            Pointer.current.press.wasPressedThisFrame)
        {
            SceneManager.LoadScene(nextSceneName);
        }

    }

    private void BlinkTouchText()
    {
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        touchText.alpha = alpha;
    }
}