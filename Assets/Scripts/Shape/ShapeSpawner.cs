using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSpawner : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int _initialSetsCount = 10;
    [SerializeField] private int _columns = 6;
    [SerializeField] private float _cellWidth = 1.1f;
    [SerializeField] private Vector2 _startPos = new Vector2(-3f, 6f);

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnDelay = 0.1f;

    [Header("Prefabs & Data")]
    [SerializeField] private List<ShapeData> _shapeDataList;
    [SerializeField] private Transform _spawnRoot;

    private Coroutine _spawnRoutine;
    private int _lastTotal;

    private void Start()
    {
        _lastTotal = _initialSetsCount * 3;
        _spawnRoutine = StartCoroutine(SpawnRoutine(_lastTotal));
    }

    public void FullRespawn()
    {
        StopAndClear();
        _lastTotal = _initialSetsCount * 3;
        _spawnRoutine = StartCoroutine(SpawnRoutine(_lastTotal));
    }

    public void RefreshField()
    {
        int current = _spawnRoot.childCount;
        StopAndClear();
        _lastTotal = current;
        _spawnRoutine = StartCoroutine(SpawnRoutine(_lastTotal));
    }

    private void StopAndClear()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
        for (int i = _spawnRoot.childCount - 1; i >= 0; i--)
            Destroy(_spawnRoot.GetChild(i).gameObject);
    }

    private IEnumerator SpawnRoutine(int totalShapes)
    {
        var queue = new List<ShapeData>(totalShapes);
        int fullTriples = totalShapes / 3;
        for (int i = 0; i < fullTriples; i++)
        {
            var data = _shapeDataList[Random.Range(0, _shapeDataList.Count)];
            queue.Add(data);
            queue.Add(data);
            queue.Add(data);
        }
        while (queue.Count < totalShapes)
            queue.Add(_shapeDataList[Random.Range(0, _shapeDataList.Count)]);

        for (int i = queue.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (queue[i], queue[j]) = (queue[j], queue[i]);
        }

        for (int i = 0; i < queue.Count; i++)
        {
            int col = i % _columns;
            var pos = new Vector3(_startPos.x + col * _cellWidth, _startPos.y, 0f);
            var go = Instantiate(queue[i].Prefab, pos, Quaternion.identity, _spawnRoot);
            go.GetComponent<ShapeController>().Initialize(queue[i]);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }
}
