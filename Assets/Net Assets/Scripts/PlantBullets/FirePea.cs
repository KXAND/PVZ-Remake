using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePea : StraightBulletAnimationSwitch
{

    protected override void attack(ZombieBase target)
    {
        //��ʬ�ܵ�����
        target.beAttacked(hurt);
        target.beBurned();
    }
}
