using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zombie;

// 1. 游戏不断地生成僵尸波次
// 2. 当上一波次的僵尸威胁度低于一定值时，或者出现时间大于一定值时，视为波次已完成，游戏会生成新的僵尸波次
// 3. 当上一波次的僵尸威胁度低于一定值时，或者出现时间大于一定值时，视为波次已完成，增加对应的进度
// 3.1 波次已完成的具体实现是威胁度为空、进度涨到对应进度
// 4. 【威胁度】随着【进度】增加而增长，也可以说威胁度随着波次的增加而增长
// 5. 进度达到一定程度，就会出现【一大波】僵尸，允许有额外的僵尸容量
// 6. 进度达到目标值时，游戏结束
public class LevelManager : MonoBehaviour
{
    float _progress;
    float _threat;
    float _threatLowHold = 2;
    float _threatHighHold = 6;

    float progressDelta = 1;
    float progressTimes = 0;

    public bool isGameEnding = false;

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
                SpawnZombies();
                Debug.Log("threat" + _threat);
                Progress += progressDelta * progressTimes;
                progressTimes = 4;
                _threatLowHold = TargetThreat * 0.75f;// 上一波威胁度不足，生成新波次。同时，只有在生成新波次时，下界会被更新
                _threatHighHold = Mathf.Clamp(TargetThreat + 1, TargetThreat * 1.25f, 30);
            }
        }
    }

    UnityEvent ProgressChanged;
    UnityEvent ProgressDone;
    ZombiesPools pools;

    float targetProgress;
    public static LevelManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    public void Init(LevelConfig config)
    {
        pools = ZombiesPools.Instance;
        pools.Init(config);
        _progress = 0.0f;
        targetProgress = 10.0f;
        //ProgressDone.AddListener(() => { Instance.isGameEnding = true; });
        StartCoroutine(GrowProgress());
        //Threat = 0;
        //SpawnZombies();



        // Spawn Idle Zombie
        int cnt = 0;
        foreach (var zombie in config.zombieList)
        {
            cnt += zombie.quantity;
        }

        int IdleNum = Random.Range(10 - 3, 10 + 3);
        foreach (var zombie in config.zombieList)
        {
            //if(zombie.zombieType== ZombieType.) 
            var rate = (float)zombie.quantity / (float)cnt;
            for (int i = 0; i < zombie.quantity * IdleNum / cnt + 1; i++)
            {
                var instance = pools.GetFromPool(zombie.zombieType, transform, Zombie.ZombieState.Idle);
                instance.transform.SetParent(transform);
                float xDis = 1f;
                float yDis = 2.5f;
                instance.transform.position = new(Random.Range(transform.position.x - xDis, transform.position.x + xDis), Random.Range(transform.position.y - yDis, transform.position.y + yDis), 0f);
            }
        }
    }
    void SpawnZombies()
    {
        while (_threat < TargetThreat)
        {
            var type = (ZombieType)Random.Range(0, 3);
            if (GlobalSettings.Threatness[type] + _threat > _threatHighHold)
                continue;
            //Debug.Log(Progress + " " + _threat + TargetThreat);
            var zombie = pools.GetFromPool((ZombieType)type, transform);
            zombie.transform.SetParent(transform);
            zombie.transform.position = PlantingManager.Instance.GetRandomRowRightGridPosition() + new Vector3(2f, 0f, 0f);

            _threat += GlobalSettings.Threatness[type];
        }
    }
    IEnumerator GrowProgress()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log(Progress);
            if (progressTimes > 0)
            {
                Progress += progressDelta;
                progressTimes--;
                Debug.Log(Progress + " " + Threat);
                if (progressTimes == 0) Threat = 0;
            }
        }
    }

}