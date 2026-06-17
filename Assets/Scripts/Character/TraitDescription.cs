using System;
using UnityEngine;

/// <summary>
/// A list of flavor text descriptions for each of the levels of a particular stat. Just a data container for strings, effectively.
/// </summary>
[Serializable]
public class TraitDescription
{
    [SerializeField] private string[] traitDescriptions = new string[5];
}