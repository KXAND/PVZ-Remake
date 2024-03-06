using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SoundPack", menuName = "Scriptable Objects/New SoundPack")]
public class SoundPack : ScriptableObject
{
    public AudioClip[] chompSounds;
    public AudioClip[] groanSounds;// ÔÚidle×´Ì¬ÏÂ´¥·¢

    public AudioClip limbsPopSound;
}
