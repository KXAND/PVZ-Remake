using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zombie;

// 1. ��Ϸ���ϵ����ɽ�ʬ����
// 2. ����һ���εĽ�ʬ��в�ȵ���һ��ֵʱ�����߳���ʱ�����һ��ֵʱ����Ϊ��������ɣ���Ϸ�������µĽ�ʬ����
// 3. ����һ���εĽ�ʬ��в�ȵ���һ��ֵʱ�����߳���ʱ�����һ��ֵʱ����Ϊ��������ɣ����Ӷ�Ӧ�Ľ���
// 3.1 ��������ɵľ���ʵ������в��Ϊ�ա������ǵ���Ӧ����
// 4. ����в�ȡ����š����ȡ����Ӷ�������Ҳ����˵��в�����Ų��ε����Ӷ�����
// 5. ���ȴﵽһ���̶ȣ��ͻ���֡�һ�󲨡���ʬ�������ж���Ľ�ʬ����
// 6. ���ȴﵽĿ��ֵʱ����Ϸ����
public class LevelManager : MonoBehaviour
{
    float _progress;
    float _threat;
    float _threatLowHold = 2;
    float _threatHighHold = 6;

    float progressDelta = 1;
    float progressTimes = 0;
    float targetProgress;
    public bool isGameEnding = false;

    UnityEvent ProgressChanged = new();
    UnityEvent ProgressDone = new();
    float TargetThreat => Progress * 1 + 4;
    public float Progress
    {
        get { return _progress; }
        protected set
        {
            _progress = value;
            ProgressChanged?.Invoke();
            if (_progress > targetProgress) ProgressDone?.Invoke();
        }
    }
    public float Threat
    {
        get { return _threat; }
        set
        {
            _threat = value;
            if (_threat < _threatLowHold && isGameEnding == false)
            {
                _threatLowHold = TargetThreat * 0.75f;// ��һ����в�Ȳ��㣬�����²��Ρ�ͬʱ��ֻ���������²���ʱ���½�ᱻ����
                _threatHighHold = Mathf.Clamp(TargetThreat + 1, TargetThreat * 1.25f, 30);
                SpawnZombies();
                Progress += progressDelta * progressTimes;
                progressTimes = 4;
            }
        }
    }

    public static LevelManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    public void Init(LevelConfig config)
    {
        ZombiesPools.Instance.Init(config);
        _progress = 0.0f;
        targetProgress = 10.0f;
        ProgressDone.AddListener(OnProgressDone);
        StartCoroutine(GrowProgress());
        Threat = 0;


        // Spawn Idle Zombie
        int cnt = 0;
        foreach (var zombie in config.zombieList)
        {
            cnt += zombie.quantity;
        }

        int IdleNum = Random.Range(10 - 3, 10 + 3);
        foreach (var zombie in config.zombieList)
        {
            for (int i = 0; i < zombie.quantity * IdleNum / cnt + 1; i++)
            {
                var instance = ZombiesPools.Instance.GetFromPool(zombie.zombieType, transform, Zombie.ZombieState.Idle);
                instance.transform.SetParent(transform);
                float xDis = 1f;
                float yDis = 2.5f;
                instance.transform.position = new(Random.Range(transform.position.x - xDis, transform.position.x + xDis), Random.Range(transform.position.y - yDis, transform.position.y + yDis), 0f);
            }
        }
    }
    void OnProgressDone()
    {
        isGameEnding = true;
    }
    void SpawnZombies()
    {
        while (_threat < TargetThreat)
        {
            var type = (ZombieType)Random.Range(0, 3);
            if (GlobalSettings.Threatness[type] + _threat > _threatHighHold)
                continue;
            var zombie = ZombiesPools.Instance.GetFromPool(type, transform);
            zombie.transform.SetParent(transform);
            zombie.transform.position = PlantingManager.Instance.GetRandomRowRightGridPosition() + new Vector3(2f, 0f, 0f);

            _threat += GlobalSettings.Threatness[type];
        }
    }
    IEnumerator GrowProgress()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("progress" + Progress);
            if (progressTimes > 0)
            {
                Progress += progressDelta;
                progressTimes--;
                if (progressTimes == 0)
                {
                    yield return new WaitForSeconds(5);
                    Threat = 0;
                }
            }
        }
    }

}