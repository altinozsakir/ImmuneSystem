using UnityEngine;


public sealed class FramerateBootstrap : MonoBehaviour
{
[SerializeField] int targetFps = 60;
void Awake()
    {
        Application.targetFrameRate = targetFps;
        QualitySettings.vSyncCount = 0; // optional: disable VSync to enforce target FPS
    }
}