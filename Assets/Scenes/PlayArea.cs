using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    [SerializeField] BoardProperties boardProperties;

    void Awake()
    {
        transform.localScale = new Vector3(
            boardProperties.HalfExtent.x * 2 + 1,
            boardProperties.HalfExtent.y * 2 + 1,
            1
        );
    }
}
