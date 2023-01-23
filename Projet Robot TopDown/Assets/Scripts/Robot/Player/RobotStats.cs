using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RobotStats", menuName = "ScriptableObjects/RobotStats", order = 1)]
public class RobotStats : ScriptableObject
{
    public string _name;
    public int _actionPointsPerTurn;
    public int _viewDistance;
    public List<WeaponStats> _weapons;
}
