using UnityEngine;

public class FrameRateSetup : MonoBehaviour
{
    [SerializeField] private int _targetFps = -1;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;           
        Application.targetFrameRate = _targetFps;  
    }
}
