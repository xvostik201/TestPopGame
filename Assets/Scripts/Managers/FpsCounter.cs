using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text _fpsText;
    [Header("Settings")]
    [SerializeField] private float _updateInterval = 0.5f;

    private float _accumulatedTime;
    private int _frames;
    private float _intervalTimer;

    private void Start()
    {
        _intervalTimer = _updateInterval;
    }

    private void Update()
    {
        _accumulatedTime += Time.unscaledDeltaTime;
        _frames++;
        _intervalTimer -= Time.unscaledDeltaTime;

        if (_intervalTimer <= 0f)
        {
            float fps = _frames / _accumulatedTime;
            _fpsText.text = Mathf.RoundToInt(fps).ToString() + " FPS";

            _intervalTimer = _updateInterval;
            _accumulatedTime = 0f;
            _frames = 0;
        }
    }
}
