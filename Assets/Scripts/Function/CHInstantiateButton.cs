using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CHInstantiateButton : MonoBehaviour
{
    [SerializeField] public GameObject origin;
    [SerializeField] public float margin = 0f;
    [SerializeField] public int horizontalCount = 1;
    [SerializeField] public int verticalCount = 1;
    [SerializeField] public List<string> buttonValue = new List<string>();

    [SerializeField, ReadOnly] public int index = 0;

    private void Start()
    {
        InstantiateButton(origin, margin, horizontalCount, verticalCount);
    }

    void InstantiateButton(GameObject _origin, float _margin, int _horizontalCount, int _verticalCount)
    {
        if (_origin == null) return;

        RectTransform buttonRectTransform = _origin.GetComponent<RectTransform>();

        float buttonX = buttonRectTransform.anchoredPosition.x;
        float buttonY = buttonRectTransform.anchoredPosition.y;

        float buttonWidth = Mathf.Abs(buttonRectTransform.rect.x * 2);
        float buttonHeight = Mathf.Abs(buttonRectTransform.rect.y * 2);

        List<float> xPos = new List<float>();
        List<float> yPos = new List<float>();

        for (int i = 0; i < _horizontalCount; ++i)
        {
            xPos.Add(buttonX + (_margin + buttonWidth) * i);
        }

        for (int i = 0; i < _verticalCount; ++i)
        {
            yPos.Add(buttonY + -(_margin + buttonHeight) * i);
        }

        foreach (var y in yPos)
        {
            foreach (var x in xPos)
            {
                GameObject addObject = Instantiate(_origin, transform);
                addObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

                if (index < buttonValue.Count)
                {
                    addObject.GetComponentInChildren<TMP_Text>().text = buttonValue[index++];
                }
                else
                {
                    addObject.GetComponentInChildren<TMP_Text>().text = "";
                }

                addObject.GetComponent<Button>().OnClickAsObservable().Subscribe(_ =>
                {
                    string value = addObject.GetComponentInChildren<TMP_Text>().text;

                    // 특수 기능을 하는 버튼은 별도 처리
                    switch (value)
                    {
                        default:
                            {

                            }
                            break;
                    }
                });
            }
        }

        _origin.SetActive(false);
    }
}
