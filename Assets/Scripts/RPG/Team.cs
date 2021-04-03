using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
    public Hero[] heroes;
    public MaskSet[] maskSets;

    public int size => heroes.Length;
}

[System.Serializable]
public class MaskSet
{
    public Mask mask;
    public Troop troop;
}

[System.Serializable]
public class Mask
{
    public Kanohi kanohi;
    public int elementId;
}