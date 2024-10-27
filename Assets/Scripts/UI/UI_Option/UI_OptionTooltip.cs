using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_OptionTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI optionDescriptionText;

    public void ShowStatTooltip(string _text)
    {
        optionDescriptionText.text = _text;

        gameObject.SetActive(true);
    }

    public void HideStatTooltip() => gameObject.SetActive(false);
}
