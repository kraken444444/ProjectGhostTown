using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "New Spell", menuName = "Game/Spell")]
public class Spell : SerializedScriptableObject
{
    [BoxGroup("Basic Info"), Title("$GetNameWithLevel")]
    [HorizontalGroup("Basic Info/Row1")]
    [VerticalGroup("Basic Info/Row1/Left"), LabelWidth(80)]
    public string spellName;

    [VerticalGroup("Basic Info/Row1/Right"), LabelWidth(80)]
    [PropertyRange(1, 99)]
    public int levelRequirement = 1;

    [VerticalGroup("Basic Info/Row1/Right")]
    public ClassType classRequirement = ClassType.Radiomancer;

    [BoxGroup("Basic Info")]
    [TextArea(3, 5), HideLabel]
    public string description;

    [FoldoutGroup("Casting Settings"), LabelWidth(120)]
    [PropertyRange(0, 100), SuffixLabel("resource")]
    [PropertyTooltip("How much resource is needed to cast this spell")]
    public int resourceCost;

    [FoldoutGroup("Casting Settings"), LabelWidth(120)]
    [PropertyRange(0, 10), SuffixLabel("seconds"), GUIColor(1f, 0.9f, 0.7f)]
    [InfoBox("Cast time affects the player's vulnerability", InfoMessageType.Info)]
    public float castTime;

    [FoldoutGroup("Casting Settings"), LabelWidth(120)]
    [PropertyRange(0, 30), SuffixLabel("seconds"), GUIColor(0.7f, 0.9f, 1f)]
    public float cooldown;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [EnumPaging]
    public TargetType targetType;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [ToggleLeft]
    public bool isAOE;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [PropertyRange(1, 50), SuffixLabel("units")]
    public float range = 10f;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [PropertyRange(1, 20), SuffixLabel("units")]
    [ShowIf("isAOE")]
    [PropertyTooltip("Area effect radius")]
    public float aoeRadius = 5f;

    [TabGroup("Effects", "Damage")]
    [ProgressBar(0, 100, 1, 0, 0)]
    [PropertyTooltip("Base damage done by this spell")]
    [InfoBox("Scales with Offense attribute", InfoMessageType.None)]
    public int baseDamage;

    [TabGroup("Effects", "Healing")]
    [ProgressBar(0, 100, 0, 1, 0)]
    [PropertyTooltip("Base healing done by this spell")]
    public int baseHealing;

    [TabGroup("Effects", "Visuals")]
    [PreviewField(60, ObjectFieldAlignment.Left)]
    public GameObject spellVisualPrefab;

    [TabGroup("Effects", "Visuals")]
    [PreviewField(60, ObjectFieldAlignment.Left)]
    [ShowIf("@targetType != TargetType.Self")]
    public GameObject projectilePrefab;

    [TabGroup("Effects", "Visuals")]
    [PropertyRange(1, 50), ShowIf("@projectilePrefab != null")]
    [SuffixLabel("units/sec")]
    public float projectileSpeed = 15f;

    [TabGroup("Effects", "Audio")]
    public AudioClip castSound;
    
    [TabGroup("Effects", "Audio")]
    [PropertyRange(0, 1)]
    public float soundVolume = 0.7f;

    [TabGroup("Player")] private GameObject playerObject;

    #region Property Getters
    private string GetNameWithLevel()
    {
        return $"{spellName} (Level {levelRequirement})";
    }
    #endregion

    [Button("Test in Scene")]
    private void TestSpellInScene()
    {
        if (Application.isPlaying)
        {
        Character player = playerObject.GetComponent<Character>();
            if (player != null)
            {
                Debug.Log($"Testing {spellName} with player character");
               // Logic to test the spell
            }
            else
            {
             Debug.LogWarning("No player found in scene to test spell with");
            }
        }
        else
        {
            Debug.LogWarning("Enter play mode to test spell");
        }
    }
}
