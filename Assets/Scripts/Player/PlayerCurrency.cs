using System;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField] private int copper;

    public int Copper => copper;

    public event Action OnCurrencyChanged;

    private void Start()
    {
        LoadCurrency();
    }

    public void AddCopper(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        copper += amount;

        SaveCurrency();
        OnCurrencyChanged?.Invoke();
    }

    public bool RemoveCopper(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (copper < amount)
        {
            return false;
        }

        copper -= amount;

        SaveCurrency();
        OnCurrencyChanged?.Invoke();

        return true;
    }

    private void LoadCurrency()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            return;
        }

        copper = Mathf.Max(0, characterData.Copper);
        OnCurrencyChanged?.Invoke();
    }

    private void SaveCurrency()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            return;
        }

        characterData.Copper = copper;

        CharacterSaveManager.SaveCharacter(characterData);
    }
}