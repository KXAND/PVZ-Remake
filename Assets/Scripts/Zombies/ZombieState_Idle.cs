using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
namespace Zombie
{
    public class ZombieState_Idle : IState
    {
        ZombieBase zombie;
        //float speed = 0;
        public ZombieState_Idle(ZombieBase zombie)
        {
            this.zombie = zombie;
        }

        public void OnEnter()
        {
            zombie.animator.Play("Zombie_Idle");
            //if (Random.Range(0, 2) == 0)
                //zombie.animator.SetBool("isIdle", true);
            //else
                //zombie.animator.SetBool("isSwag", true);
                //zombie.animator.SetBool("isIdle", true);

        }

        public void OnLeave()
        {
            return;
        }

        public void OnState()
        {
            return;
        }


    }
}