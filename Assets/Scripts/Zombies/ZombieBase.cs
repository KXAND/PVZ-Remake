using Plant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zombie
{
    public enum ZombieState
    {
        Idle,
        Eat,
        Walk,
        Die,
        Empty
    }

    public class ZombieBase : MonoBehaviour, IProduct
    {
        public virtual float ThreatWeight => 1;// 威胁度 

        //[HideInInspector]
        public float speed = 1f;
        [HideInInspector] public Rigidbody2D rb;
        [HideInInspector] public AudioSource audioSource;
        [HideInInspector] public Animator animator;
        public float attack = 10f;
        public bool isDead = false;

        Dictionary<ZombieState, IState> states = new();
        [SerializeField] ZombieState currentState;
        [SerializeField] protected float health;
        [SerializeField] protected SoundPack soundPack;
        public ZombieType type = ZombieType.Zombie;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();

            states.Add(ZombieState.Idle, new ZombieState_Idle(this));
            states.Add(ZombieState.Walk, new ZombieState_Walk(this, audioSource, soundPack.groanSounds));
            states.Add(ZombieState.Eat, new ZombieState_Eat(this, soundPack.chompSounds));
            states.Add(ZombieState.Die, new ZombieState_Die(this, transform.GetChild(0).gameObject, soundPack.limbsPopSound));
        }
        public virtual void Init(ZombieState defaultState)
        {
            TransitionState(defaultState);
            health = 100;
        }

        private void Update()
        {
            states[currentState]?.OnState();
        }

        protected void TransitionState(ZombieState newState = ZombieState.Empty)
        {
            states[currentState].OnLeave();
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
        public virtual void TakeDamage(float damage, AudioClip hitSound = null)
        {
            //Boomdamage可以考虑另写一个函数，这是因为Boom伤害有如下特点
            // 1. 没有击中音效
            // 2. 在失去头颅后可以变黑，倒地后会直接消失
            // 3. 对除伽刚特尔以外的大部分僵尸都是秒杀的
            SoundPlay(hitSound ?
                hitSound :
                soundPack.hitSounds[Random.Range(0, soundPack.hitSounds.Length)]);
            if (health <= 0) return;// 已经stateDie了，但是死去的僵尸可能还在承伤
            health -= damage;

            if (health <= 0)
            {
                TransitionState(ZombieState.Die);
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

        public virtual void SoundPlay(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public IProduct Produce()
        {
            return Instantiate(this);
        }
    }
}