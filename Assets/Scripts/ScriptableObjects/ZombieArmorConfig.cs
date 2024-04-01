using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New ArmorConfig", menuName = "Scriptable Objects/New ArmorConfig")]
public class ZombieArmorConfig : ScriptableObject
{
    public float armorHealth;
    public AudioClip[] hitSound;
    public Sprite armorFine;
    public Sprite armorSlightBroken;
    public Sprite armorAlmostBroken;
}
