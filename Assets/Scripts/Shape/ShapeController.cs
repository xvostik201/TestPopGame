using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ShapeController : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private SpriteRenderer _bodyRenderer;
    [SerializeField] private SpriteRenderer _animalRenderer;

    private SpriteRenderer _outlineRenderer;
    private ShapeData _data;
    private Rigidbody2D _rb;
    private Collider2D _col;
    private ActionBarManager _bar;
    private bool _isFlying;
    private bool _isFrozen;
    private int _frozenCounter;

    public UnityEvent<ShapeController> OnClicked = new UnityEvent<ShapeController>();

    public ShapeType Shape => _data.ShapeType;
    public AnimalType Animal => _data.AnimalType;
    public Color BodyColor => _data.BodyColor;
    public bool IsFlying => _isFlying;
    public bool IsExplosive => _data.Special == SpecialType.Explosive;

    private void Awake()
    {
        _outlineRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _bar = FindObjectOfType<ActionBarManager>();
    }

    public void Initialize(ShapeData data)
    {
        _data = data;
        _frozenCounter = data.FrozenThreshold;
        ApplyData();
        ApplySpecialOnSpawn();
    }

    private void ApplyData()
    {
        if (_bodyRenderer && _data.BodySprite != null)
            _bodyRenderer.sprite = _data.BodySprite;
        _bodyRenderer.color = _data.BodyColor;

        if (_animalRenderer && _data.AnimalSprite != null)
            _animalRenderer.sprite = _data.AnimalSprite;

        if (_data.Special == SpecialType.Explosive)
            _outlineRenderer.color = Color.black;
    }

    private void ApplySpecialOnSpawn()
    {
        switch (_data.Special)
        {
            case SpecialType.Heavy:
                _rb.gravityScale *= 2f;
                break;
            case SpecialType.Frozen:
                _isFrozen = true;
                break;
        }
    }

    private void OnMouseDown()
    {
        if (_isFlying || _isFrozen) return;
        _isFlying = true;
        _col.enabled = false;
        _rb.isKinematic = true;

        OnClicked.Invoke(this);
        AudioManager.I.PlaySFX(AudioManager.Sfx.Click);
        _bar.AddToBar(this, ReturnToPhysics);
    }

    private void ReturnToPhysics()
    {
        _isFlying = false;
        _rb.isKinematic = false;
        _col.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_data.Special != SpecialType.Sticky) return;
        if (collision.collider.TryGetComponent<ShapeController>(out var other))
            transform.SetParent(other.transform, worldPositionStays: true);
    }

    public void OnLanded()
    {
        _isFlying = false;
        if (_data.Special == SpecialType.Frozen)
        {
            _frozenCounter--;
            if (_frozenCounter <= 0) _isFrozen = false;
        }
    }
}
