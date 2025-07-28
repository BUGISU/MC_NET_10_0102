using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSettingSliderLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Slider slider;
    [SerializeField] private string valueName = "";

    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(UpdateLabel);
        UpdateLabel(slider.value);
    }

    public void UpdateLabel(float value)
    {
        label.text = string.Format(
            "{0}: {1}",
            valueName,
            slider.value.ToString("F1")
        );
    }
}
