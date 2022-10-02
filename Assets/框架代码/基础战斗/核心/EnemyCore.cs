using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCore : BattleCore
{
    [Header("敌人数据")]
    public enemyInfo ei_;
    
    [Header("敌人的行动轨迹")] 
    public int wave;
    public float appearTime;        //该敌人出现的时间
    public List<Vector3> pointList = new List<Vector3>();   //该敌人的路径锚点(x,y,time,sta)

    private Animator anim;
    public SpineAnimController ac_;
    public EnemyPathController epc_;
    private int fightingContinue = 0;       // fight激活后延续几帧

    private int cannotMove = 0;     // 锁定移动，只有该变量=0时才可以移动
    private int blocked = 0;        // 阻挡该敌人的干员数量
    public ValueBuffer speedDeta = new ValueBuffer(1);    // 移动速度和动画播放速度的改变量

    public PushAndPullController ppc_;
    
    protected override void Awake_Core()
    {
        base.Awake_Core();
        InitManager.Register(this);
        ppc_ = new PushAndPullController(this);
        transform.position = BaseFunc.x0z(pointList[0]);
    }

    protected override void Start_Core()
    {
        base.Start_Core();
        
        anim = transform.Find("anim").GetComponent<Animator>();
        animTransform = anim.transform;
        ac_ = new SpineAnimController(anim, this, 0.3f);
        epc_ = new EnemyPathController(this, pointList);
        
        // 初始化battleCalculation
        InitCalculation();
        
        gameObject.SetActive(false);        // 敌人开始是处于关闭状态
    }
    
    
    protected override void Update_Core()
    {
        base.Update_Core();
        
        ac_.Update();
        epc_.Update();
        ppc_.Update();
        
        Dizzy();
        Move();
        Fight();
        GetPriority();
    }

    private void InitCalculation()
    {
        atk_.ChangeBaseValue(ei_.atk);
        def_.ChangeBaseValue(ei_.def);
        magicDef_.ChangeBaseValue(ei_.magicDef);
        life_.InitBaseLife(ei_.life);
        maxBlock.ChangeBaseValue(ei_.consumeBlock);
        atkSpeedController = new AtkSpeedController(this, ac_, 0, ei_.minAtkInterval);
    }

    public override void GetDizzy()
    {
        base.GetDizzy();
        cannotMove++;
    }

    public override void RevokeDizzy()
    {
        base.RevokeDizzy();
        cannotMove--;
    }
    
    private void Dizzy()
    {
        if (dizziness > 0)
        {
            anim.SetBool("down", true);
        }
        else
        {
            anim.SetBool("down", false);
        }
    }

    private void Move()
    {
        if (cannotMove > 0)
        {   // 如果当前无法move
            anim.SetBool("move", false);
            return;
        }
        
        Vector3 nxtPoint = epc_.GetTarPoint();
        if (nxtPoint == Vector3.zero)
        {   // 如果已经到达终点
            ReachEnd();
            return;
        }

        if (BaseFunc.preEqual(transform.position, nxtPoint))
        {   // 如果当前应该去的点位就是脚下
            anim.SetBool("move", false);
            ac_.slowSpeed = 1;
            ac_.ChangeAnimSpeed();
            return;
        }
        
        anim.SetBool("move", true);
        ac_.slowSpeed = speedDeta.val;
        ac_.ChangeAnimSpeed();
        
        Vector2 tmp = nxtPoint - transform.position;
        if (tmp.x < 0)
            ac_.TurnLeft();
        else if (tmp.x > 0)
            ac_.TurnRight();
        
        transform.position = Vector3.MoveTowards(transform.position, nxtPoint,
            ei_.speed * speedDeta.val * Time.deltaTime);
    }

    private void Fight()
    {
        if (dizziness > 0) return;
        var staInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (staInfo.IsName("Fight"))
        {
            fighting = true;
        }
        else fighting = false;
        
        if (!tarIsNull && CanAtk())
        {
            anim.SetBool("fight", true);
            NorAtkStartCool();
            fightingContinue = (int) (3 / Time.timeScale);;
            
            // 根据目标位置转变敌人朝向
            Vector2 detaPos = BaseFunc.xz(transform.position) - BaseFunc.xz(target.transform.position);
            if (detaPos.x < 0) ac_.TurnRight();
            else ac_.TurnLeft();
        }
        else if (fightingContinue > 0)
        {
            fightingContinue--;
        }
        else
        {
            anim.SetBool("fight", false);
        }
    }

    private Vector2 lastPoint;
    private Vector2 enterPos;
    protected virtual void GetPriority()
    {
        var position = transform.position;

        if (ei_.isDrone)
        {
            tarPriority = Vector2.Distance(BaseFunc.xz(position), epc_.finalPoint);
        }
        else
        {
            Vector2 nowPoint = BaseFunc.FixCoordinate(position);
            
            if (nowPoint != lastPoint)
            {
                lastPoint = nowPoint;
                enterPos = BaseFunc.xz(transform.position);
            }
            
            if (InitManager.blueDoorPathList[epc_.blueDoorNum].dis.ContainsKey(nowPoint))
            {
                tarPriority = InitManager.blueDoorPathList[epc_.blueDoorNum].dis[nowPoint];
                tarPriority -= Vector2.Distance(BaseFunc.xz(transform.position), enterPos);
            }
            else tarPriority = 0;
        }
    }
    
    protected virtual void ReachEnd()
    {
        // 敌人到达终点后
        if (dying) return;
        dying = true;
        if (DieAction != null) DieAction(this);
        InitManager.resourceController.HPIncrease(-ei_.consumeHP);
        anim.transform.parent = null;
        transform.position = new Vector3(999, 999, 999);
        ac_.ChangeColor(Color.black);
        Invoke(nameof(DestroySelf), 0.8f);
    }
    
    /// <summary>
    /// 该敌人被某干员阻挡
    /// </summary>
    public void BeBlocked()
    {
        blocked++;
        cannotMove++;
    }

    /// <summary>
    /// 该敌人脱离某干员的阻挡
    /// </summary>
    public void UnBlocked()
    {
        blocked--;
        cannotMove--;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("operator") && !other.CompareTag("box")) return;
        BattleCore battleCore = other.GetComponent<BattleCore>();
        battleCore.DieAction += DelBattleCore_EnemyBlock;
        blockList.Add(battleCore);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("operator") && !other.CompareTag("box")) return;
        BattleCore battleCore = other.GetComponent<BattleCore>();
        battleCore.DieAction -= DelBattleCore_EnemyBlock;
        DelBattleCore_EnemyBlock(battleCore);
    }
    
    private void DelBattleCore_EnemyBlock(BattleCore bc_)
    {
        blockList.Remove(bc_);
    }


    protected override void DieBegin()
    {// 死亡撤退函数
        anim.transform.parent = null;
        transform.position = new Vector3(999, 999, 999);

        anim.SetBool("die", true);
        ac_.ChangeColor(Color.black);
    }

    public virtual void OnAttack()
    {
        Battle(target, atk_.val, DamageMode.Physical);
    }

    public void OnDie()
    {
        Destroy(anim.gameObject);
        Destroy(gameObject);
    }

    private void DestroySelf()
    {
        Destroy(anim.gameObject);
        Destroy(gameObject);
    }

    public override void FrozenBegin()
    {
        cannotMove++;
    }

    public override void FrozenEnd()
    {
        cannotMove--;
    }
}

public class Spfa
{
    private const int INF = (int)1e9;
    private float[] fx = {1, -1, 0, 0};
    private float[] fy = {0, 0, 1, -1};

    public Vector2 startPos { get; private set; }
    public Vector2 tarPos { get; private set; }
    public Dictionary<Vector2, int> dis { get; private set; } = new Dictionary<Vector2, int>();
    public Dictionary<Vector2, Vector2> prt { get; private set; } = new Dictionary<Vector2, Vector2>();
    private Dictionary<Vector2, bool> visit = new Dictionary<Vector2, bool>();
    private Queue<Vector2> q = new Queue<Vector2>();

    
    private void GetStart(Vector2 S, Vector2 T)
    {
        Vector2 preS = new Vector2();
        Vector2 slashS = new Vector2();
        Vector2 ds = new Vector2();

        preS = BaseFunc.FixCoordinate(S);
        if (InitManager.GetMap(preS).type == TileType.box)
        {
            q.Enqueue(preS);
            dis.Add(preS, 0);
            visit.Add(preS, true);
            return;
        }

        S.x = Mathf.Round(S.x);
        S.y = Mathf.Round(S.y);

        slashS.x = S.x * 2 - preS.x;
        slashS.y = S.y * 2 - preS.y;
        
        for (int i = 0; i < 4; i++)
        {
            if (i == 0 || i == 1) ds.x = S.x + 0.5f;
            else ds.x = S.x - 0.5f;
            if (i == 0 || i == 2) ds.y = S.y + 0.5f;
            else ds.y = S.y - 0.5f;

            if (InitManager.GetMap(ds) == null) continue;
            if (!Interpreter.canPass(InitManager.GetMap(ds).type)) continue;
            if (ds == slashS) continue;
            
            q.Enqueue(ds);
            dis.Add(ds, 0);
            visit.Add(ds, true);
        }
    }

    /// <summary>
    /// 执行一次spfa，会生成S到T的路径锚点队列，用于一般坐标的敌人寻址
    /// </summary>
    public bool RunSpfa(Queue<Vector2> realPointQueue, Vector2 S, Vector2 T)
    {
        startPos = S;
        tarPos = T;
        dis.Clear();
        prt.Clear();
        visit.Clear();
        q.Clear();

        GetStart(S, T);

        while (q.Count > 0)
        {
            Vector2 u = q.Dequeue();
            visit[u] = false;

            for (int i = 0; i < 4; i++)
            {
                Vector2 v = new Vector2(u.x + fx[i], u.y + fy[i]);
                TileSlot vSlot = InitManager.GetMap(v);
                if (vSlot == null) continue;
                if (!Interpreter.canPass(vSlot.type) && v != T) continue;

                if (!dis.ContainsKey(v)) dis[v] = INF;
                if (dis[v] > dis[u] + vSlot.len)
                {
                    dis[v] = dis[u] + vSlot.len;
                    
                    if (!visit.ContainsKey(v)) visit.Add(v, false);
                    if (!visit[v])
                    {
                        visit[v] = true;
                        q.Enqueue(v);
                    }
                    if (!prt.ContainsKey(v)) prt.Add(v, u); 
                    prt[v] = u;
                }
            }
        }

        GetPath(realPointQueue, T);
        return visit.ContainsKey(T);
    }
    
    private void GetPath(Queue<Vector2> realPointQueue, Vector2 T)
    {
        if (prt.ContainsKey(T)) 
            GetPath(realPointQueue,prt[T]);

        realPointQueue.Enqueue(T);
    }
    
    /// <summary>
    /// 执行一次spfa，S为标准坐标，用于固定位置的全图最短路，返回是否可到达全部出兵点
    /// </summary>
    public bool RunSpfa(Vector2 S)
    {
        S = BaseFunc.FixCoordinate(S);
        startPos = S;
        dis.Clear();
        prt.Clear();
        visit.Clear();
        q.Clear();
        
        q.Enqueue(S);
        dis.Add(S, 0);
        visit.Add(S, true);

        while (q.Count > 0)
        {
            Vector2 u = q.Dequeue();
            visit[u] = false;
            for (int i = 0; i < 4; i++)
            {
                Vector2 v = new Vector2(u.x + fx[i], u.y + fy[i]);
                TileSlot vSlot = InitManager.GetMap(v);
                if (vSlot == null) continue;
                if (!Interpreter.canPass(vSlot.type)) continue;

                if (!dis.ContainsKey(v)) dis[v] = INF;
                if (dis[v] > dis[u] + vSlot.len)
                {
                    dis[v] = dis[u] + vSlot.len;
                    
                    if (!visit.ContainsKey(v)) visit.Add(v, false);
                    if (!visit[v])
                    {
                        visit[v] = true;
                        q.Enqueue(v);
                    }
                    if (!prt.ContainsKey(v)) prt.Add(v, u); 
                    prt[v] = u;
                }
            }
        }
        
        foreach (var i in InitManager.startPointList)
        {
            if (!dis.ContainsKey(i)) return false;
        }

        return true;
    }
}

public class EnemyPathController
{
    private Queue<Vector3> pointQueue = new Queue<Vector3>();   // 该敌人的路径锚点(x,y,time)
    private Spfa spfa_ = new Spfa();
    private EnemyCore ec_;
    private bool isDrone;
    
    private Vector3 endPoint = new Vector3();           // (x, y, time) 二维坐标
    public Vector2 finalPoint { get; private set; } = new Vector3();         // (x, y) 二维坐标
    private Vector3 nxtPoint = new Vector3();           // (x, y, z) 真实坐标
    private Vector3 lastPoint = new Vector3();

    public int blueDoorNum{ get; private set; }
    private Queue<Vector2> realPointQueue = new Queue<Vector2>();

    public EnemyPathController(EnemyCore enemyCore, List<Vector3> pointList)
    {
        // 注意，构造本结构体必须在Start中构造，不能提前

        ec_ = enemyCore;
        isDrone = ec_.ei_.isDrone;
        foreach (var point in pointList)
        {
            pointQueue.Enqueue(point);
        }
        
        finalPoint = pointList[pointList.Count - 1];
        endPoint = pointQueue.Dequeue();
        nxtPoint = BaseFunc.x0z(endPoint);
        lastPoint = nxtPoint;
        
        for (int i = 0; i < InitManager.blueDoorPathList.Count; i++)
        {
            if (finalPoint != InitManager.blueDoorPathList[i].startPos) continue;
            blueDoorNum = i;
            break;
        }
    }

    public void Update()
    {
        // 该函数必须在外界Update中被调用才会生效

        if (BaseFunc.Equal(BaseFunc.xz(ec_.transform.position), endPoint))
        {
            if (endPoint.z > 0)
            {
                endPoint.z -= Time.deltaTime;
            }
            else if (pointQueue.Count == 0)
            {
                nxtPoint = Vector3.zero;        // 以目标点为(0, 0, 0)作为到达终点的判断条件
            }
            else
            {
                GetNextPointQueue();
            }
        }
        
    }

    
    /// <summary>
    /// 返回当前敌人需要到达的下一个目标点
    /// </summary>
    public Vector3 GetTarPoint()
    {
        if (BaseFunc.preEqual(ec_.transform.position,nxtPoint))
        {   // 如果已经到达指定的目标点
            
            if (realPointQueue.Count != 0)
            {   // 如果真实路径队列中还有点，从队列中取出下一个点
                nxtPoint = BaseFunc.x0z(realPointQueue.Dequeue());
            }
            else
            {   // 真实路径队列中没有点了
                
            }
            
        }
        return nxtPoint;
    }

    private void GetNextPointQueue()
    {
        if (pointQueue.Count == 0)
        {
            return;
        }
        endPoint = pointQueue.Dequeue();
        ChangeRoute();
    }

    
    /// <summary>
    /// 以当前的endPoint为目标点，刷新其路径
    /// </summary>
    public bool ChangeRoute()
    {
        if (isDrone)
        {
            nxtPoint = BaseFunc.x0z(endPoint);;
            return true;
        }

        realPointQueue.Clear();
        bool found = spfa_.RunSpfa(realPointQueue, BaseFunc.xz(ec_.transform.position), endPoint);

        if (!found) return false;
        nxtPoint = BaseFunc.x0z(realPointQueue.Dequeue());;
        return true;
    }
}

public class PushAndPullController
{
    public static float littleForce = 20f;
    public static float mediumForce = 40f;
    public static float greatForce = 60f;
    public static float superGreatForce = 80f;
    
    public EnemyCore ec_;
    public Rigidbody rb_;
    private float py;

    public PushAndPullController(EnemyCore enemyCore)
    {
        ec_ = enemyCore;
        rb_ = ec_.GetComponent<Rigidbody>();
        py = ec_.transform.position.y;
    }

    public void Update()
    {

    }


    public void Push(Vector3 forcePoint, float forceStrength)
    {
        // 从力的发出点往外推
        Vector3 pos = ec_.transform.position;
        Vector3 direction = new Vector3(pos.x - forcePoint.x, 0, pos.z - forcePoint.z);
        Displacement(direction,forceStrength);
    }
    
    public void Pull(Vector3 forcePoint, float forceStrength)
    {
        // 从力的发出点往里拉
        Vector3 pos = ec_.transform.position;
        Vector3 direction = new Vector3(forcePoint.x - pos.x, 0, forcePoint.z - pos.z);
        Displacement(direction,forceStrength);
    }

    public void Displacement(Vector3 direction, float forceStrength)
    {
        // 方向向量归一化
        float k = Mathf.Sqrt(1.0f / (direction.x * direction.x + direction.z * direction.z));
        direction.x *= k;
        direction.z *= k;
        direction *= forceStrength;

        // 给敌人一个瞬间的力
        rb_.AddForce(direction, ForceMode.Impulse);
        
        // 让敌人在位移途中眩晕，如果位置距离太短则不用眩晕
        bool willDizzy = forceStrength / rb_.mass >= 10;
        PushDizzyBuff buff = new PushDizzyBuff(this, willDizzy);
        BuffManager.AddBuff(buff);
    }
    
}

public class PushDizzyBuff : BuffSlot
{
    private PushAndPullController ppc_;
    private Rigidbody rb_;
    private EnemyCore ec_;
    private bool willDizzy;
    
    
    private bool isDie;
    private int checkDelay;

    public PushDizzyBuff(PushAndPullController pushAndPullController, bool dizzy)
    {
        ppc_ = pushAndPullController;
        rb_ = ppc_.rb_;
        ec_ = ppc_.ec_;
        willDizzy = dizzy;
    }

    public override void BuffStart()
    {
        if (willDizzy) ec_.GetDizzy();
        ec_.DieAction += Die;
        checkDelay = (int) (4 / Time.timeScale);
    }

    public override void BuffUpdate() { }

    public override bool BuffEndCondition()
    {
        if (isDie) return true;
        if (checkDelay > 0)
        {
            checkDelay--;
            return false;
        }
        return almostZero(rb_.velocity);
    }

    public override void BuffEnd()
    {
        if (willDizzy) ec_.RevokeDizzy();
        if (isDie) return;
        ec_.DieAction -= Die;
        ec_.epc_.ChangeRoute();
    }

    private void Die(BattleCore bc_)
    {
        isDie = true;
    }

    private bool almostZero(Vector3 a)
    {
        return Mathf.Abs(a.x) < 1e-2 && Mathf.Abs(a.y) < 1e-2 && Mathf.Abs(a.z) < 1e-2;
    }
    
}

public class EnemySlowDurationBuff : BattleCoreDurationBuff
{
    private EnemyCore ec_;
    private float slowRate;
    private ValueBuffInner buffInner; 

    public EnemySlowDurationBuff(EnemyCore enemyCore, float slowRate_, float during) : base(enemyCore, during)
    {
        ec_ = enemyCore;
        slowRate = slowRate_;
    }

    public override void BuffStart()
    {
        base.BuffStart();
        buffInner = new ValueBuffInner(ValueBuffMode.Multi, slowRate);
        ec_.speedDeta.AddValueBuff(buffInner);
    }

    public override void BuffEnd()
    {
        base.BuffEnd();
        ec_.speedDeta.DelValueBuff(buffInner);
    }
}