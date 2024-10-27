using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statDescriptionText;

    public void ShowStatTooltip(string _text)
    {
        statDescriptionText.text = _text;

        gameObject.SetActive(true);
    }

    public void HideStatTooltip() => gameObject.SetActive(false);
}
