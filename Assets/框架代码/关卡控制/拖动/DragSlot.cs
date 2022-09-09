using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [HideInInspector] public OperatorCore operatorCore;       // 干员预制体，关卡内不释放
    [HideInInspector] public bool operIsNull = true;
    private Transform operatorAnim;         // 干员预制体下的动画子物体
    private dirDrag dirDrag_;               


    public RectTransform rectTransform;
        
    public Image operImage;
    public Image elementImage;
    public Text costText;
    public GameObject numPanel;
    public Text numText;
    public GameObject recoverPanel;
    public Text recoverText;
    [HideInInspector] public Animator anim;     // 干员头像动画控制器，控制拖动时的上浮与下沉


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();
        InitManager.dragSlotController.Register(this);
        
    }

    void Start()
    {
        dirDrag_ = OperUIElements.dragPanel.transform.Find("MovingArrow").GetComponent<dirDrag>();
    }

    private void Update()
    {
        if (operIsNull) return;
        float reTime = InitManager.operReTime[operatorCore.operID];
        if (reTime - Time.deltaTime > 0)
        {
            recoverText.text = reTime.ToString("f1");
            InitManager.operReTime[operatorCore.operID] -= Time.deltaTime;
        }
        else if (reTime > 0)
        {
            recoverPanel.SetActive(false);
        }
        
    }

    public void Refresh(OperatorCore oc_)
    {
        if (oc_ == null)
        {
            operatorCore = null;
            operIsNull = true;
            return;
        }
        operatorCore = oc_;
        operIsNull = false;
        int id = operatorCore.operID;
        operData od_ = InitManager.allOperDataList[id];

        operImage.sprite = od_.imageInQueue;
        elementImage.sprite = SpriteElement.GetElementSprite(od_.elementType);
        costText.text = operatorCore.costNeed.ToString();

        int num = InitManager.offOperList[id].Count;
        switch (num)
        {
            case 0:
                gameObject.SetActive(false);
                break;
            case 1:
                numPanel.SetActive(false);
                break;
            default:
                numPanel.SetActive(true);
                numText.text = "X" + num;
                break;
        }

        if (InitManager.operReTime[id] > 0) recoverPanel.SetActive(true);
        else recoverPanel.SetActive(false);
    }


    bool CanPut()
    {
        
        if (operIsNull) return false;
        if (recoverPanel.activeSelf) return false;
        if (InitManager.resourceController.remainPlace < operatorCore.od_.consumPlace) return false;
        return true;
    }

    private Vector3 Get_Pos(Vector3 pos)
    {
        // 根据当前的xz坐标，以及地图方块类别，获得应有的坐标
        pos.y = 0;
        TileSlot tileSlot = InitManager.GetMap(pos);
        if (tileSlot == null) return pos;
        
        if (Interpreter.canPut(tileSlot.type))
            pos = BaseFunc.x0z(BaseFunc.FixCoordinate(pos));
        
        if (Interpreter.isHigh(InitManager.GetMap(pos).type))
                    pos.y = BaseFunc.highOper_y;

        return pos;
    }
    
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanPut()) return;

        OperUIManager.OpenOperUI(UIstate.Dragging, operatorCore);

        anim.SetBool("up", true);  // 底部头像上浮
        operatorAnim = operatorCore.animObject.transform;
        
        // 将anim单独拿出，作为拖动对象
        operatorAnim.parent = null;
        
        // 像dirDrag注册，自身为当前的workingDragSlot
        dirDrag_.workingDragSlot = this;

        InitManager.TimeSlowDrag();     // 执行拖动时的偏移与时停
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanPut()) return;

        // 利用鼠标射线得到世界坐标，并在偏移后赋给给动画坐标
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;  // 得到碰撞点的坐标
            point = Get_Pos(point);
            point.z += BaseFunc.operAnimFix_z;
            operatorAnim.position = point;
        }
    }
    

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanPut()) return;

        if (InitManager.GetMap(operatorAnim.position) != null &&
            Interpreter.canPut(InitManager.GetMap(operatorAnim.position).type))
        {
            OperUIManager.rightUIController.DragPanelPos(operatorAnim.position);
            dirDrag_.BackGroundButton.onClick.AddListener(PutFailed);
        }
        else
        {
            DragRecover();
        }
    }

    public void AnimTurnRight()
    {
        operatorAnim.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    public void AnimTurnLeft()
    {
        operatorAnim.localScale = new Vector3(-0.3f, 0.3f, 0.3f);
    }

    /// <summary>
    /// 从Drag状态还原到正常状态，仅还原DragSlot相关
    /// </summary>
    public void DragRecover()
    {
        anim.SetBool("up", false);  // 底部头像下沉
        InitManager.TimeRecover();
        OperUIManager.CloseOperUI();
            
        // 将anim还给其父亲
        operatorAnim.parent = operatorCore.transform;
        operatorAnim.localPosition = new Vector3(0, 0, BaseFunc.operAnimFix_z);
    }

    
    /// <summary>
    /// 本次放置未能成功，还原一切到放之前状态
    /// </summary>
    public void PutFailed()
    {
        DragRecover();
    }

    /// <summary>
    /// 本次放置成功，部署干员预制体
    /// </summary>
    public void PutSuccessfully()
    {
        OperUIManager.CloseOperUI();
        
        Vector3 pos = BaseFunc.x0z(BaseFunc.FixCoordinate(operatorAnim.position));
        if (Interpreter.isHigh(InitManager.GetMap(operatorAnim.position).type))
            pos.y = BaseFunc.highOper_y;

        operatorCore.transform.position = pos;
        DragRecover();
        
        operatorCore.gameObject.SetActive(true);
        operatorCore.OperInit();
        
        dirDrag_.BackGroundButton.onClick.RemoveListener(PutFailed);

        InitManager.operReTime[operatorCore.operID] = operatorCore.recoverTime.val;

        InitManager.resourceController.CostIncrease(-operatorCore.costNeed);
        InitManager.resourceController.RemainPlaceIncrease(-operatorCore.od_.consumPlace);
        
        InitManager.operList.Add(operatorCore);
        InitManager.offOperList[operatorCore.operID].RemoveAt(0);
        InitManager.dragSlotController.RefreshDragSlot();
        
    }

    public void OnClick()
    {// 点击下方的干员按钮，以场下方式打开左侧UI
        if (operIsNull) return;
        OperUIManager.OpenOperUI(UIstate.Down, operatorCore);
        InitManager.TimeSlow();
        anim.SetBool("up", true);
    }

}
