using UnityEngine;

// 앱 실행 시 자동으로 한 번 실행되는 전역 설정 (씬에 붙일 필요 없음)
public static class AppSettings
{
    private const int MinFrameRate = 60;
    private const int MaxFrameRate = 120;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // 안드로이드는 기본값이 30fps라 드래그가 뚝뚝 끊겨 보이므로, 화면 주사율에 맞춰 60~120fps로 실행
        int refreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        int targetFrameRate = Mathf.Clamp(refreshRate, MinFrameRate, MaxFrameRate);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}
