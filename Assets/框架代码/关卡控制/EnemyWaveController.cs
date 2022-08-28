using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveController : MonoBehaviour        // 按波次和时间生成敌人
{
    // 默认波次最大等待时间
    private const int MaxWaveTime = 30;
    
    
    public int wave { get; private set; } = -1;     // 当前波次
    public int maxWave { get; private set; }
    private float timeLine = 0;                     // 当前波次的时间轴
    private float nxtWaveTime;                      // 等待的时间轴
    private bool waving;                            // 当前是否处于下一波次展示时间
    
    public bool disableWaveUI = false;              // 激活时，禁用红门UI
    private Queue<EnemyCore> enemyQueue = new Queue<EnemyCore>();           // 当前波次的敌人队列
    private List<List<string>> redDoorTextList = new List<List<string>>();  // 红门展示的敌人文本，以波次切分

    private void Awake()
    {
        InitManager.enemyWaveController = this;
        
    }

    void Start()
    {
        foreach (var i in InitManager.redDoorUILIst)
        {
            i.startButton.onClick.AddListener(NextWaveStart);
        }
        
        maxWave = InitManager.allEnemyList.Count;
        foreach (var i in InitManager.allEnemyList)
        {
            
            
            // 统计每一波次的敌人名称
            List<Dictionary<string, int>> map = new List<Dictionary<string, int>>();
            redDoorTextList.Add(new List<string>());
            int lastNum = redDoorTextList.Count - 1;
            for (int o = 0; o < InitManager.redDoorUILIst.Count; o++)
            {
                redDoorTextList[lastNum].Add("");
                map.Add(new Dictionary<string, int>());
            }

            foreach (var j in i)
            {
                // 初始化startPointList
                if (!InitManager.startPointList.Contains(j.pointList[0]))
                {
                    InitManager.startPointList.Add(j.pointList[0]);
                }
                
                int id = -1;
                for (int k = 0; k < InitManager.redDoorUILIst.Count; k++)
                {
                    if (BaseFunc.preEqual(InitManager.redDoorUILIst[k].transform.position,
                        j.transform.position))
                    {
                        id = k;
                        break;
                    }
                }
                if (id != -1)
                {
                    if (!map[id].ContainsKey(j.ei_.name)) map[id][j.ei_.name] = 0;
                    map[id][j.ei_.name]++;
                }
            }
            for (int k = 0; k < InitManager.redDoorUILIst.Count; k++)
            {
                foreach (var o in map[k])
                {
                    redDoorTextList[lastNum][k] += o.Key + "x" + o.Value.ToString() + "\n";
                }
            }
            // 将每一波的敌人列表按生成时间排序
            i.Sort((x, y) => x.appearTime.CompareTo(y.appearTime));
        }
        
        NextWaveUI();
        foreach (var i in InitManager.redDoorUILIst)
        {
            i.sliderImage.fillAmount = 1;
        }
    }
    
    void Update()
    {
        if (wave == -1) return;
        timeLine += Time.deltaTime;
        
        // 生成满足条件的敌人
        while (enemyQueue.Count > 0 && timeLine >= enemyQueue.Peek().appearTime)
        {
            EnemyCore ec = enemyQueue.Dequeue();
            ec.gameObject.SetActive(true);
            InitManager.enemyList.Add(ec);
            ec.DieAction += DelFromEnemyList;
        }
        // 如果当前波次的所有敌人均已生成，开始进入下一波的等待
        if (enemyQueue.Count == 0 && !disableWaveUI && !waving)
            NextWaveUI();
        
        //下一波开始倒计时
        if (waving && wave < InitManager.allEnemyList.Count)
        {
            nxtWaveTime += Time.deltaTime;
            foreach (var i in InitManager.redDoorUILIst)
            {
                i.sliderImage.fillAmount = nxtWaveTime / MaxWaveTime;
            }
            if (nxtWaveTime >= MaxWaveTime) NextWaveStart();
        }
    }
    
    
    /// <summary>
    /// 准备进入下一波，展示UI
    /// </summary>
    public void NextWaveUI()
    {
        int nwave = wave + 1;
        if (nwave >= InitManager.allEnemyList.Count) return;

        nxtWaveTime = 0;
        // 更新所有红门的文本
        for (int i = 0; i < redDoorTextList[nwave].Count; i++)
        {
            var s = redDoorTextList[nwave][i];
            InitManager.redDoorUILIst[i].rewardUI.SetActive(false);
            if (s == "") continue;
            InitManager.redDoorUILIst[i].gc_.Show();
            InitManager.redDoorUILIst[i].detailText.text = s;
        }
        
        waving = true;
    }
    
    /// <summary>
    /// 进入下一波
    /// </summary>
    public void NextWaveStart()
    {
        wave++;
        if (wave >= InitManager.allEnemyList.Count) return;
        timeLine = 0;
        enemyQueue.Clear();
        if (!disableWaveUI)
        {
            float leftTime = MaxWaveTime - nxtWaveTime;
            
            int gexp = (int) Math.Ceiling(10f * leftTime / MaxWaveTime);
            int gcost = 10;
            // int gcost = (int) cost_.GetCostDuring(leftTime);
            // cost_.GetCost(gcost);
            // exp_.GetExp(gexp);
            
            
            foreach (var i in InitManager.redDoorUILIst)
                i.ChangeRewardUI(gcost, gexp);
            
            // foreach (var i in operDragList)
            //     i.reTime = i.reTime - leftTime > 0 ? i.reTime - leftTime : 0;
        }
        
        
        foreach (var i in InitManager.redDoorUILIst)
        {
            i.gc_.Hide();
        }
        
        foreach (var i in InitManager.allEnemyList[wave])
        {
            enemyQueue.Enqueue(i);
        }
        
        waving = false;
    }

    private void DelFromEnemyList(BattleCore ec_)
    {
        InitManager.enemyList.Remove((EnemyCore) ec_);
    }
    
}
