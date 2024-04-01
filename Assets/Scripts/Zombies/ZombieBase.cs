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
        public virtual float ThreatWeight => 1;// ��в�� 

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

        // ������������ӵ���������������㶹�����ǽ�ʬ��֪��splat��Ч
        public virtual void TakeDamage(float damage, AudioClip hitSound = null)
        {
            //Boomdamage���Կ�����дһ��������������ΪBoom�˺��������ص�
            // 1. û�л�����Ч
            // 2. ��ʧȥͷ­����Ա�ڣ����غ��ֱ����ʧ
            // 3. �Գ�٤���ض�����Ĵ󲿷ֽ�ʬ������ɱ��
            SoundPlay(hitSound ?
                hitSound :
                soundPack.hitSounds[Random.Range(0, soundPack.hitSounds.Length)]);
            if (health <= 0) return;// �Ѿ�stateDie�ˣ�������ȥ�Ľ�ʬ���ܻ��ڳ���
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