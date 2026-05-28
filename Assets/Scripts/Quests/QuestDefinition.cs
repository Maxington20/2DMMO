using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "MMO Simulator/Quest")]
public class QuestDefinition : ScriptableObject
{
    [Header("Save Identity")]
    public string questId;

    [Header("Quest Info")]
    public string questName;
    [TextArea] public string description;

    [Header("Kill Objective")]
    public string targetEnemyName;
    public int requiredKills = 1;

    [Header("Rewards")]
    public int xpReward = 0;
    public int copperReward = 0;
}