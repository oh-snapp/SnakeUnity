using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResizer : MonoBehaviour
{
    [SerializeField] BoardProperties boardProperties;
    [SerializeField] new Camera camera;
    [SerializeField] float marginPercent;

    float boardAspect;

    void Reset()
    {
        camera = GetComponent<Camera>();
    }

    void Start()
    {
        boardAspect = (float)boardProperties.HalfExtent.x / boardProperties.HalfExtent.y;
        UpdateCamera();
    }

    void Update()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        // width is unconstrained
        if(camera.aspect > boardAspect)
        {
            camera.orthographicSize = boardProperties.HalfExtent.y + 0.5f;
        }
        else // height is unconstrained
        {
            camera.orthographicSize = (boardProperties.HalfExtent.x + 0.5f) / camera.aspect;
        }
    }
}
