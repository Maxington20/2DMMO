using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private PlayerCurrency playerCurrency;
    [SerializeField] private TextMeshProUGUI copperText;

    private void Start()
    {
        if (playerCurrency != null)
        {
            playerCurrency.OnCurrencyChanged += Refresh;
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (playerCurrency != null)
        {
            playerCurrency.OnCurrencyChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (playerCurrency == null || copperText == null)
        {
            return;
        }

        copperText.text = $"{playerCurrency.Copper} Copper";
    }
}