using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardProperties", menuName = "Board Properties")]
public class BoardProperties : ScriptableObject
{
    [SerializeField] Vector2Int defaultHalfExtent = new(8, 8);
    [SerializeField] float defaultSpeed = 1f;

    [System.NonSerialized] public Vector2Int HalfExtent;
    [System.NonSerialized] public float Speed;

    void OnEnable()
    {
        HalfExtent = defaultHalfExtent;
        Speed = defaultSpeed;
    }
}
