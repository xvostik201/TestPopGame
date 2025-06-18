using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameCanvas : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform _winPanel;
    [SerializeField] private RectTransform _losePanel;

    [Header("Buttons")]
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _refreshFieldButton;

    [Header("Animation Settings")]
    [SerializeField] private float _panelAnimDuration = 0.5f;
    [SerializeField] private Ease _panelEase = Ease.OutBack;

    private void Awake()
    {
        _winPanel.localScale = Vector3.zero;
        _losePanel.localScale = Vector3.zero;
        _retryButton.onClick.AddListener(OnRetryButton);
        _refreshFieldButton.onClick.AddListener(OnRefreshButton);
    }

    private void OnEnable()
    {
        GameManager.I.OnGameWin.AddListener(ShowWin);
        GameManager.I.OnGameLose.AddListener(ShowLose);
    }

    private void OnDisable()
    {
        if (GameManager.I == null) return;
        GameManager.I.OnGameWin.RemoveListener(ShowWin);
        GameManager.I.OnGameLose.RemoveListener(ShowLose);
    }

    private void ShowWin()
    {
        AudioManager.I.PlaySFX(AudioManager.Sfx.Win);
        _winPanel.gameObject.SetActive(true);
        _winPanel.DOScale(Vector3.one, _panelAnimDuration).SetEase(_panelEase);
        ActivateRetry();
    }

    private void ShowLose()
    {
        AudioManager.I.PlaySFX(AudioManager.Sfx.Lose);
        _losePanel.gameObject.SetActive(true);
        _losePanel.DOScale(Vector3.one, _panelAnimDuration).SetEase(_panelEase);
        ActivateRetry();
    }

    private void ActivateRetry()
    {
        _retryButton.gameObject.SetActive(true);
        _retryButton.transform.DOScale(Vector3.one, _panelAnimDuration).SetEase(_panelEase);
        _refreshFieldButton.gameObject.SetActive(false);
    }

    private void OnRetryButton()
    {
        _winPanel.gameObject.SetActive(false);
        _losePanel.gameObject.SetActive(false);
        _retryButton.gameObject.SetActive(false);
        _refreshFieldButton.gameObject.SetActive(true);
        _winPanel.localScale = Vector3.zero;
        _losePanel.localScale = Vector3.zero;

        FindObjectOfType<ActionBarManager>().ClearBar();
        FindObjectOfType<ShapeSpawner>().FullRespawn();
        GameManager.I.ResetGame();
    }

    private void OnRefreshButton()
    {
        FindObjectOfType<ShapeSpawner>().RefreshField();
    }
}
