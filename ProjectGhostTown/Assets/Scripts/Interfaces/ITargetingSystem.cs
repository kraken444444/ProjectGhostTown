using System;
using UnityEngine;
public interface ITargetingSystem
{
    bool IsTargeting { get; }
    Vector2 CurrentTargetPosition { get; }
    void StartTargeting(Spell spell);
    void CancelTargeting();
    void UpdateTargetingVisual(Spell spell);
    event Action<Vector2> OnTargetSelected;
    event Action OnTargetingCancelled;
}