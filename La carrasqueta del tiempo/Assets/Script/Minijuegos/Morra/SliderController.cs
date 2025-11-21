using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SliderController : MonoBehaviour
{
    public TMP_Text valueText;

    public void OnSliderChanged(float value)
    {
        valueText.SetText(value.ToString());
    }
}
