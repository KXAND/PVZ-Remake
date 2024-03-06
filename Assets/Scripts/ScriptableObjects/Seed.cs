using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Seed", menuName = "Scriptable Objects/New Seed")]
public class Seed : ScriptableObject
{
    public Sprite seedSprite;
    public int costSun;
    public int CardCDTime;
    public GameObject seedInstance;
    public bool isReadyAtStart;
}
