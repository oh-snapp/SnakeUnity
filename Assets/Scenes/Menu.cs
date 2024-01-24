using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] BoardProperties boardProperties;

    [SerializeField] TMPro.TMP_InputField widthTmp;
    [SerializeField] TMPro.TMP_InputField heightTmp;
    
    [SerializeField] Slider speedSlider;
    [SerializeField] Button startButton;

    [SerializeField] string gameSceneName;

    void Start()
    {
        widthTmp.text = boardProperties.HalfExtent.x.ToString();
        heightTmp.text = boardProperties.HalfExtent.y.ToString();
        speedSlider.value = boardProperties.Speed;

        startButton.onClick.AddListener(OnStart);
    }

    void OnStart()
    {
        if (widthTmp.text.Length == 0 || heightTmp.text.Length == 0) return;

        boardProperties.HalfExtent = new Vector2Int(
            int.Parse(widthTmp.text),
            int.Parse(heightTmp.text)
        );
        boardProperties.Speed = speedSlider.value;

        SceneManager.LoadScene(gameSceneName);
    }
}
