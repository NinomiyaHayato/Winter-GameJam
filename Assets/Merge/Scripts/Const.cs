using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const
{
    // Header‚ÌF•Ï‚¦—p
    public const string PreColorTag = "<color=#7CFC00>";
    public const string SufColorTag = "</color>";

    public const string PlayerTag = "Player";
    public const string ItemTag = "Item";
    public const string EnemyTag = "Enemy";
    public const string BedTag = "Bed";
    public static readonly int PlayerLayer = 1 << LayerMask.NameToLayer("Player");
}
