using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "New Spell", menuName = "Game/Spell")]
public class Spell : SerializedScriptableObject
{
    #region variables
    
    [BoxGroup("Basic Info"), Title("$GetFormattedTitle")]
    [HorizontalGroup("Basic Info/Row1")]
    [VerticalGroup("Basic Info/Row1/Left"), LabelWidth(80)]
    public string spellName;

    [VerticalGroup("Basic Info/Row1/Right"), LabelWidth(80)]
    [PropertyRange(1, 99)]
    public int levelRequirement = 1;

    [BoxGroup("Basic Info")]
    [PreviewField(60, ObjectFieldAlignment.Left)]
    public Sprite icon;

    [BoxGroup("Basic Info")]
    [TextArea(3, 5), HideLabel]
    public string description;

    [FoldoutGroup("Classification")]
    [EnumToggleButtons]
    [OnValueChanged("OnSpellTypeChanged")]
    public GameEnums.SpellType spellType;

    [FoldoutGroup("Classification")]
    [ShowIf("@spellType == GameEnums.SpellType.Attack || spellType == GameEnums.SpellType.Debuff")]
    public GameEnums.DamageType damageType;

    [FoldoutGroup("Classification")]
    [LabelText("Tags"), Tooltip("Tags for filtering and categorization")]
    public List<string> tags = new List<string>();

    [FoldoutGroup("Requirements"), LabelWidth(120)]
    [EnumToggleButtons]
    public GameEnums.ClassType classRequirement;

    [FoldoutGroup("Requirements"), LabelWidth(120)]
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

    [FoldoutGroup("Casting Settings"), LabelWidth(120)]
    [ToggleLeft]
    public bool isChanneled = false;

    [FoldoutGroup("Casting Settings"), LabelWidth(120)]
    [PropertyRange(0, 30), SuffixLabel("seconds")]
    [ShowIf("isChanneled")]
    public float channelMaxTime = 0f;

    [FoldoutGroup("Casting Settings"), LabelWidth(120)]
    [ToggleLeft, ShowIf("isChanneled")]
    public bool canBeInterrupted = true;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [EnumToggleButtons]
    public GameEnums.TargetType targetType;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [ToggleLeft]
    public bool isAOE;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [PropertyRange(1, 50), SuffixLabel("units")]
    [HideIf("@targetType == GameEnums.TargetType.Self")]
    public float range = 10f;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [PropertyRange(1, 20), SuffixLabel("units")]
    [ShowIf("isAOE")]
    [PropertyTooltip("Area effect radius")]
    public float aoeRadius = 5f;

    [FoldoutGroup("Targeting"), LabelWidth(120)]
    [PropertyRange(0, 10), SuffixLabel("targets")]
    [ShowIf("@isAOE && spellType != GameEnums.SpellType.Utility")]
    [PropertyTooltip("Max targets affected (0 = unlimited)")]
    public int maxTargets = 0;

    [TabGroup("Effects", "Damage")]
    [ProgressBar(0, 100, 1, 0, 0)]
    [PropertyTooltip("Base damage done by this spell")]
    [ShowIf("@spellType == GameEnums.SpellType.Attack || spellType == GameEnums.SpellType.Debuff")]
    public int baseDamage;

    [TabGroup("Effects", "Damage")]
    [ShowIf("@baseDamage > 0")]
    [PropertyRange(0, 5), SuffixLabel("seconds")]
    public float tickRate = 0f;

    [TabGroup("Effects", "Healing")]
    [ProgressBar(0, 100, 0, 1, 0)]
    [PropertyTooltip("Base healing done by this spell")]
    [ShowIf("@spellType == GameEnums.SpellType.Healing || spellType == GameEnums.SpellType.Buff")]
    public int baseHealing;

    [TabGroup("Effects", "Duration")]
    [PropertyRange(0, 30), SuffixLabel("seconds")]
    [PropertyTooltip("How long the effect lasts")]
    [ShowIf("@spellType == GameEnums.SpellType.Buff || spellType == GameEnums.SpellType.Debuff || (spellType == GameEnums.SpellType.Utility && targetType != GameEnums.TargetType.Self)")]
    public float duration;

    [TabGroup("Effects", "Status Effects")]
    [PropertyTooltip("Additional status effects applied by this spell")]
    [ShowIf("@spellType == GameEnums.SpellType.Buff || spellType == GameEnums.SpellType.Debuff")]
    [ListDrawerSettings(ShowFoldout = true)]
    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    [TabGroup("Effects", "Status Effects")]
    [PropertyRange(0, 10), SuffixLabel("force")]
    [ShowIf("@spellType == GameEnums.SpellType.Attack")]
    public float knockbackForce = 0f;

    [TabGroup("Effects", "Status Effects")]
    [PropertyRange(0, 5), SuffixLabel("seconds")]
    [ShowIf("@spellType == GameEnums.SpellType.Attack || spellType == GameEnums.SpellType.Debuff")]
    public float stunDuration = 0f;

    [TabGroup("Scaling", "Primary")]
    [ShowIf("@spellType != GameEnums.SpellType.Utility")]
    public GameEnums.AttributeType primaryScalingStat;

    [TabGroup("Scaling", "Primary")]
    [PropertyRange(0, 1), SuffixLabel("x")]
    [ShowIf("@spellType != GameEnums.SpellType.Utility")]
    public float primaryScalingFactor = 0.1f;

    [TabGroup("Scaling", "Secondary")]
    [ShowIf("@spellType != GameEnums.SpellType.Utility")]
    public GameEnums.AttributeType secondaryScalingStat;

    [TabGroup("Scaling", "Secondary")]
    [PropertyRange(0, 1), SuffixLabel("x")]
    [ShowIf("@spellType != GameEnums.SpellType.Utility")]
    public float secondaryScalingFactor = 0.05f;

    [TabGroup("Critical", "Settings")]
    [ToggleLeft]
    [ShowIf("@spellType == GameEnums.SpellType.Attack")]
    public bool canCrit = true;

    [TabGroup("Critical", "Settings")]
    [PropertyRange(0, 100), SuffixLabel("%")]
    [ShowIf("@canCrit && spellType == GameEnums.SpellType.Attack")]
    public float criticalChanceBonus = 0f;

    [TabGroup("Critical", "Settings")]
    [PropertyRange(1, 3), SuffixLabel("x")]
    [ShowIf("@canCrit && spellType == GameEnums.SpellType.Attack")]
    public float criticalDamageMultiplier = 1.5f;

    [TabGroup("Visuals", "Prefabs")]
    [PreviewField(60, ObjectFieldAlignment.Left)]
    public GameObject spellVisualPrefab;

    [TabGroup("Visuals", "Prefabs")]
    [PreviewField(60, ObjectFieldAlignment.Left)]
    [ShowIf("@targetType != GameEnums.TargetType.Self")]
    public GameObject projectilePrefab;

    [TabGroup("Visuals", "Settings")]
    [PropertyRange(1, 50), ShowIf("@projectilePrefab != null")]
    [SuffixLabel("units/sec")]
    public float projectileSpeed = 15f;

    [TabGroup("Audio", "Sounds")]
    public AudioClip castSound;
    
    [TabGroup("Audio", "Sounds")]
    public AudioClip impactSound;
    
    [TabGroup("Audio", "Settings")]
    [PropertyRange(0, 1)]
    public float soundVolume = 0.7f;

    [TabGroup("Advanced", "Bonuses")]
    [PropertyRange(0, 10), SuffixLabel("seconds")]
    [ShowIf("@spellType == GameEnums.SpellType.Attack")]
    public float cooldownReductionOnKill = 0f;

    [TabGroup("Advanced", "Bonuses")]
    [PropertyRange(0, 1), SuffixLabel("resource %")]
    [ShowIf("@spellType == GameEnums.SpellType.Attack")]
    public float resourceRefundOnKill = 0f;
    #endregion
    
    [System.Serializable]
    public class StatusEffect
    {
        [HorizontalGroup("Effect")]
        [VerticalGroup("Effect/Left")]
        public string effectName;
        
        [VerticalGroup("Effect/Right")]
        public GameEnums.StatusEffectType type;
        
        [HorizontalGroup("Duration")]
        [VerticalGroup("Duration/Left")]
        [PropertyRange(0.1f, 30f), SuffixLabel("seconds")]
        public float duration = 5f;
        
        [VerticalGroup("Duration/Right")]
        [PropertyRange(0.1f, 10f), SuffixLabel("potency")]
        public float potency = 1f;
        
        [PreviewField(60, ObjectFieldAlignment.Left)]
        public GameObject effectVisualPrefab;
    }

    #region Property Getters
    private string GetFormattedTitle()
    {
        return $"{spellName} (Level {levelRequirement})";
    }
    #endregion

    // Add this method to your Spell class
    private void OnSpellTypeChanged()
    {
        // This method will be called when spellType is changed in the inspector
    
        // Example: You might want to reset certain fields based on the new spell type
        // Or update field visibility (though Odin handles most of this with attributes)
    
        switch (spellType)
        {
            case GameEnums.SpellType.Attack:
                // Set default values for attack spells
                if (baseDamage <= 0) baseDamage = 10;
                break;
            
            case GameEnums.SpellType.Healing:
                // Set default values for healing spells
                if (baseHealing <= 0) baseHealing = 10;
                break;
            
            case GameEnums.SpellType.Buff:
                // Set default values for buff spells
                if (duration <= 0) duration = 5f;
                break;
            
            case GameEnums.SpellType.Debuff:
                // Set default values for debuff spells
                if (duration <= 0) duration = 3f;
                break;
            
            case GameEnums.SpellType.Utility:
                // Set default values for utility spells
                break;
        }
    }
    // Calculate damage with stat scaling
    public int CalculateDamage(BaseCharacter caster)
    {
        if (caster == null) return baseDamage;
        float primaryStatValue = caster.GetAttributes()[primaryScalingStat];
        float secondaryStatValue = caster.GetAttributes()[secondaryScalingStat];

        return Mathf.RoundToInt(baseDamage + (primaryStatValue * primaryScalingFactor) + (secondaryStatValue * secondaryScalingFactor));
    }

    // Calculate healing with stat scaling
    public int CalculateHealing(PlayerCharacter caster)
    {
        if (caster == null) return baseHealing;

        float primaryStatValue = caster.GetAttributes()[primaryScalingStat];
        float secondaryStatValue = caster.GetAttributes()[secondaryScalingStat];
        
        return Mathf.RoundToInt(baseHealing + (primaryStatValue * primaryScalingFactor) + (secondaryStatValue * secondaryScalingFactor));
    }

    #if UNITY_EDITOR
    [Button("Test in Scene"), GUIColor(0.3f, 0.8f, 0.3f)]
    private void TestSpellInScene()
    {
        if (Application.isPlaying)
        {
            PlayerCharacter player = FindAnyObjectByType<PlayerCharacter>();
            if (player != null)
            {
                Debug.Log($"Testing {spellName} with player character");
                // Find SpellManager to cast the spell
                SpellManager spellManager = GameObject.FindObjectOfType<SpellManager>();
                if (spellManager != null)
                {
                    Vector3 targetPos = player.transform.position + player.transform.forward * 5f;
                 //   spellManager.CastSpell(player, targetPos, this);
                }
                else
                {
                    Debug.LogWarning("No SpellManager found in scene");
                }
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
    #endif
}