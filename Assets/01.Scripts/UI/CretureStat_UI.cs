using TMPro;
using UnityEngine;

public class CretureStat_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueText;

    public void UpdateValue(int value)
    {
        _valueText.text = value.ToString();
    }
}
