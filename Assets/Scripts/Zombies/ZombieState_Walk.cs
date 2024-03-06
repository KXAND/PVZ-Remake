using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
namespace Zombie
{
    public class ZombieState_Walk : IState
    {
        ZombieBase zombie;
        AudioSource audioSource;
        AudioClip[] groans;
        public ZombieState_Walk(ZombieBase zombie, AudioSource audioSource, AudioClip[] groans)
        {
            this.zombie = zombie;
            this.audioSource = audioSource;
            this.groans = groans;
        }

        public void OnEnter()
        {
            zombie.rb.velocity = new Vector2(-1 * zombie.speed, 0);
            zombie.StartCoroutine(PlayGroans());
            zombie.animator.SetBool("isWalking",true);
        }

        public void OnLeave()
        {
            zombie.animator.SetBool("isWalking", false);
            return;
        }

        public void OnState()
        {
            return;
        }

        public IEnumerator PlayGroans()
        {
            yield return new WaitForSeconds(Random.Range(1f, 10f));
            audioSource.clip = groans[Random.Range(0, groans.Length)];
            audioSource.Play();
        }
    }
}