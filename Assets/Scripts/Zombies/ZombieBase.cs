using Plant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zombie
{
    enum ZombieState
    {
        Idle,
        Eat,
        Walk,
        Die
    }

    public class ZombieBase : MonoBehaviour
    {
        public float speed;
        public Rigidbody2D rb;
        public AudioSource audioSource;
        public Animator animator;
        public float attack = 10f;
        public bool isDead = false;

        Dictionary<ZombieState, IState> states = new();
        [SerializeField] ZombieState currentState;
        [SerializeField] float health;
        [SerializeField] SoundPack soundPack;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();

            states.Add(ZombieState.Idle, new ZombieState_Idle(this));
            states.Add(ZombieState.Walk, new ZombieState_Walk(this, audioSource, soundPack.groanSounds));
            states.Add(ZombieState.Eat, new ZombieState_Eat(this, soundPack.chompSounds));
            states.Add(ZombieState.Die, new ZombieState_Die(this, transform.GetChild(0).gameObject, soundPack.limbsPopSound));

            TransitionState(ZombieState.Walk);

            health = 100;
        }

        private void Update()
        {
            states[currentState]?.OnState();

        }

        void TransitionState(ZombieState newState)
        {
            states[currentState]?.OnLeave();
            currentState = newState;
            states[newState].OnEnter();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Plant"))
            {
                TransitionState(ZombieState.Eat);
                (states[currentState] as ZombieState_Eat).
                    SetPlant(collision.GetComponent<PlantBase>());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Plant"))
            {
                TransitionState(ZombieState.Walk);
            }
        }

        // 如果不是特殊子弹例如玉米粒或火豌豆，则是僵尸已知的splat音效
        public void TakeDamage(float damage, bool isBoom = false, AudioClip hitSound = null)
        {
            SoundPlay(hitSound?
                hitSound :
                soundPack.hitSounds[Random.Range(0, soundPack.hitSounds.Length)]);
            if (health <= 0) return;
            health -= damage;

            if (health <= 0)
            {
                if (!isBoom) TransitionState(ZombieState.Die);
            }
        }

        public void AnimationEventWalkingAfterDead()
        {
            rb.velocity = new Vector2(-1 * speed, 0);
        }

        public void AnimationEventStopWalkingAfterDead()
        {
            rb.velocity = Vector2.zero;
        }

        public void SoundPlay(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}