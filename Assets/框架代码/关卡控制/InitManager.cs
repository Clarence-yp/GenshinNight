using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InitManager : MonoBehaviour
{
    private static InitManager instance;
    
    // 当前场上所有的干员/敌人
    public static List<OperatorCore> operList = new List<OperatorCore>();
    public static List<EnemyCore> enemyList = new List<EnemyCore>();
    // 本场战斗选择的所有干员/分别的数量
    public static List<operData> allOperDataList = new List<operData>();
    public static List<int> allOperNumList = new List<int>();
    // 本场战斗还未布置的所有干员，按角色划分，二级队列为多数量干员（箱子等）准备
    public static List<List<OperatorCore>> offOperList = new List<List<OperatorCore>>();
    // 本场战斗会出现的所有敌人，按波次切分
    public static List<List<EnemyCore>> allEnemyList = new List<List<EnemyCore>>();
    public static int totEnemyNum;
    // 地图信息
    private static Dictionary<Vector2, TileSlot> baseMp = new Dictionary<Vector2, TileSlot>();
    private static Dictionary<Vector2, TileSlot> mp = new Dictionary<Vector2, TileSlot>();
    // 蓝门路径信息
    public static List<Spfa> blueDoorPathList = new List<Spfa>();
    // 红门UI信息
    public static List<redDoorUI> redDoorUILIst = new List<redDoorUI>();
    // 所有出兵点坐标（不仅是红门）
    public static List<Vector2> startPointList = new List<Vector2>();
    
    // 拖拽单元控制器
    public static DragSlotController dragSlotController = new DragSlotController();

    // 相机控制器
    private static CameraController cameraController_ = null;
    // 敌人波次控制器
    public static EnemyWaveController enemyWaveController;
    
    // 关卡资源控制器
    public static ResourceController resourceController = new ResourceController();


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    public static void Init()
    {
        // 生成本关的所有干员预制体
        allOperDataList.Sort((x, y) => x.cost.CompareTo(y.cost));
        for (int i = 0; i < allOperDataList.Count; i++)
        {
            offOperList.Add(new List<OperatorCore>());
            int id = offOperList.Count - 1;
            for (int j = 0; j < allOperNumList[i]; j++)
            {
                GameObject newOper=Instantiate(allOperDataList[i].operPrefab, 
                    null, true);
                newOper.transform.position = new Vector3(999, 999, 999);
                OperatorCore oc_ = newOper.GetComponent<OperatorCore>();
                oc_.operID = id;
                offOperList[id].Add(oc_);
            }
        }
        
        // 初始化拖拽单元控制器
        dragSlotController.Init();
        
        // 初始化关卡资源控制器
        resourceController.Init(1000, 1000, 1000, 1000);


    }
    
    public static void Clear()
    {
        operList.Clear();
        enemyList.Clear();
        allOperDataList.Clear();
        allOperNumList.Clear();
        offOperList.Clear();
        allEnemyList.Clear();
        baseMp.Clear();
        mp.Clear();
        blueDoorPathList.Clear();
        redDoorUILIst.Clear();
        startPointList.Clear();
        // dragSlotController.dragSlotList.Clear();
        totEnemyNum = 0;
    }
    

    public static void Register(Vector3 mpPos, TileSlot slot)
    {
        Vector2 pos = BaseFunc.FixCoordinate(mpPos);
        mp[pos] = baseMp[pos] = slot;
    }
    
    public static void Register(CameraController cc_)
    {
        cameraController_ = cc_;
    }
    
    public static void Register(Spfa spfa_)
    {
        blueDoorPathList.Add(spfa_);
    }
    
    public static void Register(redDoorUI rdu_)
    {
        redDoorUILIst.Add(rdu_);
    }

    public static void Register(EnemyCore ec_)
    {
        while(allEnemyList.Count<=ec_.wave)
            allEnemyList.Add(new List<EnemyCore>());
        allEnemyList[ec_.wave].Add(ec_);
        totEnemyNum++;
    }

    public static TileSlot GetMap(Vector3 pos)
    {
        Vector2 p = BaseFunc.FixCoordinate(pos);
        if (!mp.ContainsKey(p)) return null;
        return mp[p];
    }
    
    public static TileSlot GetMap(Vector2 pos)
    {
        pos = BaseFunc.FixCoordinate(pos);
        if (!mp.ContainsKey(pos)) return null;
        return mp[pos];
    }
    
    
    
    
    public static void TimeSlowDrag()
    {
        // if(!gameManager.pause)
        //     Time.timeScale = 0.1f;
        cameraController_.ChangeTar(cameraController_.basePos, cameraController_.slowRol);
        // cameraController_.tarPos = cameraController_.rightPos;
        // DisableAllRedDoorUI();
    }
    public static void TimeSlowPick(Transform oper)
    {
        // if(!gameManager.pause)
        //     Time.timeScale = 0.1f;
        
        Vector3 tarPos = cameraController_.basePos;
        tarPos.x = oper.position.x - 1f;
        tarPos.z = oper.position.z - 6f;
        cameraController_.ChangeTar(tarPos, cameraController_.slowRol);

        // DisableAllRedDoorUI();
    }
    public static void TimeRecover()
    {
        // if (gameManager.pause)
        //     Time.timeScale = 0;
        // else if (gameManager.twoFast)
        //     Time.timeScale = 2;
        // else 
        //     Time.timeScale = 1;

        cameraController_.ChangeTar(cameraController_.basePos, cameraController_.baseRol);

        // EnableAllRedDoorUI();
    }
    
}

public class DragSlotController
{
    private Color transparent = new Color(255, 255, 255, 0);
    private Color opaque = new Color(255, 255, 255, 255);
    
    public List<DragSlot> dragSlotList = new List<DragSlot>();

    public void Init()
    {
        // 把DragSlot按x坐标从大到小排序
        dragSlotList.Sort((a, b) => -a.rectTransform.anchoredPosition.x
            .CompareTo(b.rectTransform.anchoredPosition.x));
        
        RefreshDragSlot();
    }

    public void Register(DragSlot ds_)
    {
        dragSlotList.Add(ds_);
    }
    
    /// <summary>
    /// 按offOperList重新排列
    /// </summary>
    public void RefreshDragSlot()
    {
        // i是dragSlot的下标，j是allOperDataList的下标，也是offOperList第一层的下标
        int j = 0;
        for (int i = 0; i < dragSlotList.Count; i++, j++) 
        {
            while (j < InitManager.offOperList.Count && InitManager.offOperList[j].Count == 0) j++;
            if (j >= InitManager.offOperList.Count)
            {
                dragSlotList[i].operImage.color = transparent;
                dragSlotList[i].operatorCore = null;
            }
            else
            {
                dragSlotList[i].operImage.sprite = InitManager.allOperDataList[j].imageInQueue;
                dragSlotList[i].operatorCore = InitManager.offOperList[j][0];
            }
        }
    }
}


public class ResourceController
{
    public float cost { get; private set; }
    public float exp { get; private set; }
    public int life { get; private set; }
    public int remainPlace { get; private set; }


    /// <summary>
    /// 初始化关卡资源
    /// </summary>
    public void Init(float cost_p, float exp_p, int life_p, int remainPlace_p)
    {
        cost = cost_p;
        exp = exp_p;
        life = life_p;
        remainPlace = remainPlace_p;
    }

    /// <summary>
    /// cost增加v
    /// </summary>
    public void CostIncrease(float v)
    {
        cost += v;
    }

    /// <summary>
    /// exp增加v
    /// </summary>
    public void ExpIncrease(float v)
    {
        exp += v;
    }

    /// <summary>
    /// life增加v
    /// </summary>
    public void LifeIncrease(int v)
    {
        life += v;
    }

    /// <summary>
    /// remainPlace增加v
    /// </summary>
    public void RemainPlaceIncrease(int v)
    {
        remainPlace += v;
    }
    
    
    
    
    

}
