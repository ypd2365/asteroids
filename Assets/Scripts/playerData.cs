using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class playerData : ScriptableObject
{
    public float thrustSpeed;
    public float rotationSpeed;
    public int numberOfBullets;
    public float spreadAngle;
    public int playerHealth;
    public int blasterBoosterDuration;
    public int scoreForIncreasingDifficulty;
}
