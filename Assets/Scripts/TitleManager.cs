using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private TMP_Text touchText;
    [SerializeField] private float blinkSpeed = 1.5f;
    [SerializeField] private string nextSceneName = "MenuScene";
    [SerializeField] private AudioClip gameStartSfx;

    private void Update()
    {
        BlinkTouchText();

        if (Pointer.current != null && 
            Pointer.current.press.wasReleasedThisFrame)
        {
            SoundManager.Instance.PlaySFX(gameStartSfx);
            SceneManager.LoadScene(nextSceneName);
        }

    }

    private void BlinkTouchText()
    {
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        touchText.alpha = alpha;
    }
}