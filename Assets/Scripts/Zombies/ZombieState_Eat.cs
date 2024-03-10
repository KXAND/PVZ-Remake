using Plant;
using System.Collections;
using UnityEngine;
namespace Zombie
{
    public class ZombieState_Eat : IState
    {
        private ZombieBase zombie;
        AudioClip[] chomps;
        Coroutine coroutine;

        PlantBase beEatingPlant;

        public void SetPlant(PlantBase plant)
        {
            beEatingPlant = plant;
        }

        public ZombieState_Eat(ZombieBase zombie, AudioClip[] chomps)
        {
            this.zombie = zombie;
            this.chomps = chomps;
        }
        public void OnEnter()
        {
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
                zombie.SoundPlay(clip);
                beEatingPlant?.TakeDamage(zombie.attack);
                yield return new WaitForSeconds(clip.length);
            }
        }
    }
}