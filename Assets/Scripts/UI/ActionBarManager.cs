using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActionBarManager : MonoBehaviour
{
    [Header("UI Slots")]
    [SerializeField] private List<RectTransform> _slots;
    [Header("Field Root")]
    [SerializeField] private Transform _spawnRoot;
    [Header("Animation Settings")]
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private Ease _moveEase = Ease.OutBack;
    [SerializeField] private float _rotateDuration = 0.3f;
    [SerializeField] private Ease _rotateEase = Ease.OutQuad;
    [SerializeField] private float _removeAnimTime = 0.25f;
    [SerializeField] private float _rearrangeDelay = 0.1f;
    [Header("VFX")]
    [SerializeField] private GameObject _matchVfxPrefab;
    [SerializeField] private GameObject _bombVfxPrefab;

    private readonly List<ShapeController> _inBar = new();

    public bool IsFull => _inBar.Count >= _slots.Count;

    public event Action OnWin;
    public event Action OnLose;

    public void AddToBar(ShapeController shape, Action onRejected = null)
    {
        if (_inBar.Contains(shape) || IsFull)
        {
            onRejected?.Invoke();
            return;
        }

        var rb = shape.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        var col = shape.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        shape.transform.DOKill(true);
        int idx = _inBar.Count;
        _inBar.Add(shape);

        shape.transform
             .DOLocalRotate(Vector3.zero, _rotateDuration)
             .SetEase(_rotateEase);

        shape.transform
             .DOMove(_slots[idx].position, _moveDuration)
             .SetEase(_moveEase)
             .OnComplete(() => OnShapeLanded(shape, idx));
    }

    private void OnShapeLanded(ShapeController shape, int slotIndex)
    {
        shape.transform.DOKill(true);
        shape.transform.SetParent(_slots[slotIndex], worldPositionStays: true);
        shape.OnLanded();

        CheckAllTriplets();

        if (IsFull)
        {
            int n = _inBar.Count;
            if (n >= 3)
            {
                var a = _inBar[n - 3];
                var b = _inBar[n - 2];
                var c = _inBar[n - 1];
                bool endMatch = a.Shape == b.Shape && b.Shape == c.Shape
                             && a.Animal == b.Animal && b.Animal == c.Animal
                             && a.BodyColor == b.BodyColor && b.BodyColor == c.BodyColor;
                if (endMatch)
                {
                    RemoveTripleWithAnimation(n - 3);
                    return;
                }
            }
            OnLose?.Invoke();
            return;
        }

        if (_spawnRoot.childCount == 0)
            OnWin?.Invoke();
    }

    private void CheckAllTriplets()
    {
        for (int i = 0; i <= _inBar.Count - 3; i++)
        {
            var a = _inBar[i];
            var b = _inBar[i + 1];
            var c = _inBar[i + 2];

            if (a.IsFlying || b.IsFlying || c.IsFlying) continue;

            bool match = a.Shape == b.Shape && b.Shape == c.Shape
                      && a.Animal == b.Animal && b.Animal == c.Animal
                      && a.BodyColor == b.BodyColor && b.BodyColor == c.BodyColor;

            if (!match) continue;

            var center = (_slots[i].position + _slots[i + 1].position + _slots[i + 2].position) / 3f;
            if (a.IsExplosive || b.IsExplosive || c.IsExplosive)
            {
                Instantiate(_bombVfxPrefab, center, Quaternion.identity);
                AudioManager.I.PlaySFX(AudioManager.Sfx.Explosion);
                DOVirtual.DelayedCall(0f, ClearBar);
            }
            else
            {
                Instantiate(_matchVfxPrefab, center, Quaternion.identity);
                AudioManager.I.PlaySFX(AudioManager.Sfx.Pop);
                DOVirtual.DelayedCall(0f, () => RemoveTripleWithAnimation(i));
            }
            return;
        }
    }

    private void RemoveTripleWithAnimation(int startIndex)
    {
        if (startIndex < 0 || startIndex + 2 >= _inBar.Count) return;

        var first = _inBar[startIndex];
        var mid = _inBar[startIndex + 1];
        var last = _inBar[startIndex + 2];
        if (first.IsFlying || mid.IsFlying || last.IsFlying) return;

        _inBar.RemoveRange(startIndex, 3);

        first.transform.DOKill(true);
        mid.transform.DOKill(true);
        last.transform.DOKill(true);

        var seq = DOTween.Sequence();
        seq.Append(first.transform.DOScale(0f, _removeAnimTime))
           .Join(mid.transform.DOScale(0f, _removeAnimTime))
           .Join(last.transform.DOScale(0f, _removeAnimTime))
           .OnComplete(() =>
           {
               Destroy(first.gameObject);
               Destroy(mid.gameObject);
               Destroy(last.gameObject);
               AudioManager.I.PlaySFX(AudioManager.Sfx.Pop);
               RearrangeBar();
               DOVirtual.DelayedCall(_rearrangeDelay, CheckAllTriplets);
           });
    }

    private void RearrangeBar()
    {
        for (int i = 0; i < _inBar.Count; i++)
        {
            var shape = _inBar[i];
            shape.transform.DOKill(true);
            var rb = shape.GetComponent<Rigidbody2D>();
            if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            shape.transform
                .DOLocalRotate(Vector3.zero, _rotateDuration)
                .SetEase(_rotateEase)
                .SetDelay(0.1f);

            shape.transform
                .DOMove(_slots[i].position, _moveDuration * 0.5f)
                .SetEase(_moveEase)
                .OnComplete(() => shape.OnLanded());

            shape.transform.SetParent(_slots[i], worldPositionStays: true);
        }
    }

    public void ClearBar()
    {
        foreach (var shape in _inBar)
        {
            shape.transform.DOKill(true);
            var rb = shape.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = true;
                rb.constraints = RigidbodyConstraints2D.None;
            }
            var col = shape.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
            Destroy(shape.gameObject);
        }
        _inBar.Clear();
    }
}
