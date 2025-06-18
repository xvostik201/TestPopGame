using UnityEngine;

public enum ShapeType { Circle, Triangle, Square, Hexagon }
public enum AnimalType { Chicken, Dog, Elephant, Wolf }
public enum SpecialType { None, Heavy, Sticky, Explosive, Frozen }

[CreateAssetMenu(fileName = "ShapeData", menuName = "Game/ShapeData")]
public class ShapeData : ScriptableObject
{
    [Header("Base")]
    [SerializeField] private ShapeType _shapeType;
    [SerializeField] private AnimalType _animalType;
    [SerializeField] private Sprite _bodySprite;
    [SerializeField] private Sprite _animalSprite;
    [SerializeField] private Color _bodyColor;

    [Header("Special Behavior")]
    [SerializeField] private SpecialType _special = SpecialType.None;
    [SerializeField] private int _frozenThreshold = 3;

    [Header("Prefab")]
    [SerializeField] private GameObject _prefab;

    public ShapeType ShapeType => _shapeType;
    public AnimalType AnimalType => _animalType;
    public Sprite BodySprite => _bodySprite;
    public Sprite AnimalSprite => _animalSprite;
    public Color BodyColor => _bodyColor;
    public SpecialType Special => _special;
    public int FrozenThreshold => _frozenThreshold;
    public GameObject Prefab => _prefab;
}
