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
            zombie.animator.SetTrigger("isDead");
            zombie.transform.DetachChildren();
            zombie.GetComponent<Collider2D>().enabled = false;
            //zombie.SoundPlay(limbPopSound);
            
            head.GetComponent<AudioSource>().clip = limbPopSound;
            //head.GetComponent<AudioSource>().Play();
            head.SetActive(true);
        }

        public void OnLeave()
        {
            return;
        }

        public void OnState()
        {
            t += Time.deltaTime;
            if (t > 3f)
            {
                GameObject.Destroy(head);
                GameObject.Destroy(zombie.gameObject);
            }
        }
    }
}