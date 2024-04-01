using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
namespace Zombie
{
    public class ZombieState_Die : IState
    {
        ZombieBase zombie;
        AudioClip limbPopSound;
        float t;
        bool isRealeased = false;


        GameObject head;
        public ZombieState_Die(ZombieBase zombie, GameObject head, AudioClip limbPopSound)
        {
            this.zombie = zombie;
            this.head = head;
            this.limbPopSound = limbPopSound;
        }

        public void OnEnter()
        {
            t = 0;
            isRealeased = false;
            zombie.animator.SetTrigger("isDead");
            //zombie.transform.DetachChildren();

            head.GetComponent<AudioSource>().clip = limbPopSound;
            head.SetActive(true);
        }

        public void OnLeave()
        {
            head.SetActive(false);
            return;
        }

        public void OnState()
        {
            t += Time.deltaTime;
            if (t > 3f)
            {
                if (!isRealeased)
                {
                    ZombiesPools.Instance.Release(zombie.gameObject, zombie.type);
                    isRealeased = true;
                }
            }
        }
    }
}