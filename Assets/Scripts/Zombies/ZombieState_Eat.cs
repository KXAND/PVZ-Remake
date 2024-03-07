using Plant;
using System.Collections;
using UnityEngine;
namespace Zombie
{
    public class ZombieState_Eat : IState
    {
        private ZombieBase zombie;
        AudioSource audioSource;
        AudioClip[] chomps;
        Coroutine coroutine;

        PlantBase beEatingPlant;

        public void SetPlant (PlantBase plant)
        {
            beEatingPlant = plant;
        }

        public void GivePlantsInfo() { }
        public ZombieState_Eat(ZombieBase zombie, AudioSource audioSource, AudioClip[] chomps)
        {
            this.zombie = zombie;
            this.audioSource = audioSource;
            this.chomps = chomps;
        }
        public void OnEnter()
        {
            zombie.rb.velocity = Vector2.zero;
            zombie.animator.SetBool("isAttacking", true);
            coroutine = zombie.StartCoroutine(PlayAudioClips());
            return;
        }

        public void OnLeave()
        {
            zombie.animator.SetBool("isAttacking", false);
            zombie.StopCoroutine(coroutine);
            return;
        }

        public void OnState()
        {

        }
        IEnumerator PlayAudioClips()
        {
            while (true)
            {
                // 播放第一个 AudioClip
                AudioClip clip = chomps[Random.Range(0, chomps.Length - 1)];
                zombie.audioSource.clip = clip;
                audioSource.Play();
                beEatingPlant?.TakeDamage(zombie.attack);
                yield return new WaitForSeconds(clip.length);
            }
        }
    }
}