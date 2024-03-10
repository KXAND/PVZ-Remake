using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SoundPack", menuName = "Scriptable Objects/New SoundPack")]
public class SoundPack : ScriptableObject
{
    public AudioClip[] chompSounds;
    public AudioClip[] groanSounds;// 在idle状态下触发
    public AudioClip limbsPopSound;

    public AudioClip[] hitSounds;// 不包含击中护盾的声音
}
