using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats", order = 2)]
public class WeaponStats : ScriptableObject
{
    public string _name;
    public int _range;
    public int _coneAngle;
    public int _damage;
    public int _accuracy;
}
