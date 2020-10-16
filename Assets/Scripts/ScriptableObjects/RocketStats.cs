using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName ="PlayerStats")]
public class RocketStats : ScriptableObject
{
    public float BurnForce, MaxFuel, FuelBurnRate;
    public float RotationSpeed, FasterRotationMod;
    public float Torque = 15f, Mass = 1f, AngularDrag = 2f;
    public float Gravity;

    public int MaxMinerals = 50;
    public float MineralWeight = 0.02f;
    public float MiningLaserRange = 12f;

    private float BoostForce, BoostFuelUse;
    [Tooltip("If checked, boost is a straight velocity change \nIf unchecked, it's a force affected by mass")]
    private bool CrutchBoost;
}
