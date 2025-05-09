using UnityEngine;

public class ProjectileLauncher : SpellEffect
{
    public override void Execute(Spell spell, ISpellCaster caster, Vector3 position, Vector3 direction)
    {
        if (spell.projectilePrefab == null) return;
        
        GameObject projectile = Object.Instantiate(
            spell.projectilePrefab,
            caster.transform.position,
            Quaternion.identity
        );
        
        var controller = projectile.GetComponent<ProjectileController>();
        if (controller != null)
        {
            controller.Initialize(spell, caster, direction);
        }
        else
        {
            Debug.LogWarning($"Projectile {spell.projectilePrefab.name} has no ProjectileController component");
            Object.Destroy(projectile);
        }
    }
}
