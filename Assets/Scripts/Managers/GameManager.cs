using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Win/Lose Events")]
    public UnityEvent OnGameWin = new UnityEvent();
    public UnityEvent OnGameLose = new UnityEvent();

    public static GameManager I { get; private set; }

    private bool _isGameOver;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    private void Start()
    {
        var bar = FindObjectOfType<ActionBarManager>();
        bar.OnWin += HandleWin;
        bar.OnLose += HandleLose;
    }

    private void HandleWin()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        OnGameWin.Invoke();
    }

    private void HandleLose()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        OnGameLose.Invoke();
    }

    public void ResetGame()
    {
        _isGameOver = false;
    }
}
