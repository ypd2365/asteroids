using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class asteroidData : ScriptableObject
{
    public float minSize = 0.35f;
    public float maxSize = 1.65f;
    [Range(100f, 245f)]
    public float movementSpeed = 100f;
    public enum powerupAppereance { frequent, rare }

    public powerupAppereance powerupappereance;
}
