using System.Collections.Generic;
using UnityEngine;

public interface ISpellCaster
{
    string CasterID { get; }
    Attributes GetAttributes();
    void ConsumeResource(int amount);
    Transform transform { get; } // For effect positioning
}