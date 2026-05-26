using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NameplateController playerNameplate;
    [SerializeField] private PlayerFrameUI playerFrameUI;
    [SerializeField] private CombatEntity combatEntity;
    [SerializeField] private PlayerAbilityController abilityController;
    [SerializeField] private ActionBarUI actionBarUI;

    private void Start()
    {
        CharacterData savedCharacter = CharacterSaveManager.LoadCharacter();

        if (savedCharacter == null)
        {
            return;
        }

        ApplyCharacter(savedCharacter);
    }

    private void ApplyCharacter(CharacterData characterData)
    {
        if (playerNameplate != null)
        {
            playerNameplate.SetName(characterData.CharacterName);
        }

        if (playerFrameUI != null)
        {
            playerFrameUI.SetPlayerName(characterData.CharacterName);
        }

        if (combatEntity != null)
        {
            combatEntity.SetDisplayName(characterData.CharacterName);
        }

        if (abilityController != null)
        {
            abilityController.ConfigurePrimaryAbilityForClass(characterData.ClassName);
        }

        if (actionBarUI != null && abilityController != null)
        {
            actionBarUI.SetSlotOneAbilityName(abilityController.PrimaryAbilityName);
        }

        ChatManager.Instance?.AddSystemMessage(
            $"Entered world as {characterData.CharacterName}, Level {characterData.Level} {characterData.Species} {characterData.ClassName}."
        );
    }
}