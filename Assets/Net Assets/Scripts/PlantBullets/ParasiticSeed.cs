using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiticSeed : ThrowBullet
{
    protected override void attack(ZombieBase zombie)
    {
        zombie.playAudioOfBeingAttacked();
        zombie.beParasiticed(myPlant);
    }
}
