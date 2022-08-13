using UnityEngine;
using UnityEditor;

// 确定我们需要自定义编辑器的组件
[CustomEditor(typeof(operData))]
public class operDataEnum : Editor
{
    // 序列化对象
    private SerializedObject od_;
    
    // 序列化属性
    private SerializedProperty type;
    private SerializedProperty skillType;
    //"基本资料"0中变量
    private SerializedProperty  Name;
    private SerializedProperty  EnName;
    private SerializedProperty  operPrefab;
    private SerializedProperty  atkRange;
    private SerializedProperty  elementType;
    private SerializedProperty  is3D;
    private SerializedProperty  isMedical;
    private SerializedProperty  canAtkDrone;
    private SerializedProperty  banLowGround;
    private SerializedProperty  banHighGround;
    
    //"立绘图片"1中变量
    private SerializedProperty imageInQueue;
    private SerializedProperty spineDefault;
    private SerializedProperty operUIImage1;
    private SerializedProperty operUIImage2;
    private SerializedProperty illustratedBookImage;
    private SerializedProperty profession;
    private SerializedProperty star;
    private SerializedProperty skillImage;
    private SerializedProperty  atkRangeImage;
    //“语音集合”2中变量
    private SerializedProperty Report;
    private SerializedProperty LevelUP;
    private SerializedProperty ElitismUP;
    private SerializedProperty Join;
    private SerializedProperty Leave;
    private SerializedProperty Begin;
    private SerializedProperty Selected;
    private SerializedProperty Deploy;
    private SerializedProperty Fighting;
    private SerializedProperty  End_4Star;
    private SerializedProperty End_3Star;
    private SerializedProperty End_Not3Star;
    private SerializedProperty End_Fail;
    private SerializedProperty Touch;
    //"基础属性"3中变量
    private SerializedProperty atk;
    private SerializedProperty def;
    private SerializedProperty magicDef;
    private SerializedProperty life;
    private SerializedProperty maxBlock;
    private SerializedProperty cost;
    private SerializedProperty consumPlace;
    private SerializedProperty reTime;
    private SerializedProperty maxLevel;
    private SerializedProperty  Description;
    //"成长属性"4中变量
    private SerializedProperty growingAtk;
    private SerializedProperty growingDef;
    private SerializedProperty growingLife;
    private SerializedProperty elitismAtk;
    private SerializedProperty elitismDef;
    private SerializedProperty elitismLife;
    private SerializedProperty elitismMagicDef;
    private SerializedProperty elitismBlock;
    private SerializedProperty elitismExp;
    private SerializedProperty elitismCost;
    //"技能1数据"中变量
    private SerializedProperty skillName0;
    private SerializedProperty description0;
    private SerializedProperty initSP0;
    private SerializedProperty maxSP0;
    private SerializedProperty duration0;
    private SerializedProperty expNeed0;
    private SerializedProperty costNeed0;
    private SerializedProperty skill0_recoverType;
    private SerializedProperty skill0_releaseType;
    //"技能2数据"中变量
    private SerializedProperty skillName1;
    private SerializedProperty description1;
    private SerializedProperty initSP1;
    private SerializedProperty maxSP1;
    private SerializedProperty duration1;
    private SerializedProperty expNeed1;
    private SerializedProperty costNeed1;
    private SerializedProperty skill1_recoverType;
    private SerializedProperty skill1_releaseType;
    //"技能3数据"中变量
    private SerializedProperty skillName2;
    private SerializedProperty description2;
    private SerializedProperty initSP2;
    private SerializedProperty maxSP2;
    private SerializedProperty duration2;
    private SerializedProperty expNeed2;
    private SerializedProperty costNeed2;
    private SerializedProperty skill2_recoverType;
    private SerializedProperty skill2_releaseType;
    

    void OnEnable()
    {
        // 获取当前的序列化对象（target：当前检视面板中显示的对象）
        od_ = new SerializedObject(target);

        // 抓取对应的序列化属性
        type = od_.FindProperty("type");
        skillType = od_.FindProperty("skillType");
        
        //"基本资料"0中变量
        Name = od_.FindProperty("Name");
        EnName = od_.FindProperty("EnName");
        operPrefab = od_.FindProperty("operPrefab");
        atkRange = od_.FindProperty("atkRange");
        elementType = od_.FindProperty("elementType");
        is3D = od_.FindProperty("is3D");
        isMedical = od_.FindProperty("isMedical");
        canAtkDrone = od_.FindProperty("canAtkDrone");
        banLowGround = od_.FindProperty("banLowGround");
        banHighGround = od_.FindProperty("banHighGround");
        
        //"立绘图片"1中变量
        imageInQueue = od_.FindProperty("imageInQueue");
        spineDefault = od_.FindProperty("spineDefault");
        operUIImage1 = od_.FindProperty("operUIImage1");
        operUIImage2 = od_.FindProperty("operUIImage2");
        illustratedBookImage = od_.FindProperty("illustratedBookImage");
        profession = od_.FindProperty("profession");
        star = od_.FindProperty("star");
        skillImage = od_.FindProperty("skillImage");
        atkRangeImage = od_.FindProperty("atkRangeImage");
        //"语音集合"2中变量
        Report = od_.FindProperty("Report");
        LevelUP = od_.FindProperty("LevelUP");
        ElitismUP = od_.FindProperty("ElitismUP");
        Join = od_.FindProperty("Join");
        Leave = od_.FindProperty("Leave");
        Begin = od_.FindProperty("Begin");
        Selected = od_.FindProperty("Selected");
        Deploy = od_.FindProperty("Deploy");
        Fighting = od_.FindProperty("Fighting");
        End_4Star = od_.FindProperty("End_4Star");
        End_3Star = od_.FindProperty("End_3Star");
        End_Not3Star = od_.FindProperty("End_Not3Star");
        End_Fail = od_.FindProperty("End_Fail");
        Touch = od_.FindProperty("Touch");
        //"基础属性"3中变量
        atk = od_.FindProperty("atk");
        def = od_.FindProperty("def");
        magicDef = od_.FindProperty("magicDef");
        life = od_.FindProperty("life");
        maxBlock = od_.FindProperty("maxBlock");
        cost = od_.FindProperty("cost");
        consumPlace = od_.FindProperty("consumPlace");
        reTime = od_.FindProperty("reTime");
        maxLevel = od_.FindProperty("maxLevel");
        Description = od_.FindProperty("Description");
        //"成长属性"4中变量
        growingAtk = od_.FindProperty("growingAtk");
        growingDef = od_.FindProperty("growingDef");
        growingLife = od_.FindProperty("growingLife");
        elitismAtk = od_.FindProperty("elitismAtk");
        elitismDef = od_.FindProperty("elitismDef");
        elitismLife = od_.FindProperty("elitismLife");
        elitismMagicDef = od_.FindProperty("elitismMagicDef");
        elitismBlock = od_.FindProperty("elitismBlock");
        elitismExp = od_.FindProperty("elitismExp");
        elitismCost = od_.FindProperty("elitismCost");
        //"技能1数据"5中变量
        skillName0 = od_.FindProperty("skillName0");
        description0 = od_.FindProperty("description0");
        initSP0 = od_.FindProperty("initSP0");
        maxSP0 = od_.FindProperty("maxSP0");
        duration0 = od_.FindProperty("duration0");
        expNeed0 = od_.FindProperty("expNeed0");
        costNeed0 = od_.FindProperty("costNeed0");
        skill0_recoverType = od_.FindProperty("skill0_recoverType");
        skill0_releaseType = od_.FindProperty("skill0_releaseType");
        //"技能2数据"6中变量
        skillName1 = od_.FindProperty("skillName1");
        description1 = od_.FindProperty("description1");
        initSP1 = od_.FindProperty("initSP1");
        maxSP1 = od_.FindProperty("maxSP1");
        duration1 = od_.FindProperty("duration1");
        expNeed1 = od_.FindProperty("expNeed1");
        costNeed1 = od_.FindProperty("costNeed1");
        skill1_recoverType = od_.FindProperty("skill1_recoverType");
        skill1_releaseType = od_.FindProperty("skill1_releaseType");
        //"技能3数据"7中变量
        skillName2 = od_.FindProperty("skillName2");
        description2 = od_.FindProperty("description2");
        initSP2 = od_.FindProperty("initSP2");
        maxSP2 = od_.FindProperty("maxSP2");
        duration2 = od_.FindProperty("duration2");
        expNeed2 = od_.FindProperty("expNeed2");
        costNeed2 = od_.FindProperty("costNeed2");
        skill2_recoverType = od_.FindProperty("skill2_recoverType");
        skill2_releaseType = od_.FindProperty("skill2_releaseType");
    }
    
    public override void OnInspectorGUI()
    {
        // 从物体上抓取最新的信息
        od_.Update();

        EditorGUILayout.PropertyField(type);

        if (type.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(Name);
            EditorGUILayout.PropertyField(EnName);
            EditorGUILayout.PropertyField(operPrefab);
            EditorGUILayout.PropertyField(atkRange);
            EditorGUILayout.PropertyField(elementType);
            EditorGUILayout.PropertyField(is3D);
            EditorGUILayout.PropertyField(isMedical);
            EditorGUILayout.PropertyField(canAtkDrone);
            EditorGUILayout.PropertyField(banLowGround);
            EditorGUILayout.PropertyField(banHighGround);
            
        }
        else if(type.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(imageInQueue);
            EditorGUILayout.PropertyField(spineDefault);
            EditorGUILayout.PropertyField(operUIImage1);
            EditorGUILayout.PropertyField(operUIImage2);
            EditorGUILayout.PropertyField(illustratedBookImage);
            EditorGUILayout.PropertyField(profession);
            EditorGUILayout.PropertyField(star);
            EditorGUILayout.PropertyField(skillImage);
            EditorGUILayout.PropertyField(atkRangeImage);
        }
        else if(type.enumValueIndex == 2)
        {
            EditorGUILayout.PropertyField(Report);
            EditorGUILayout.PropertyField(LevelUP);
            EditorGUILayout.PropertyField(ElitismUP);
            EditorGUILayout.PropertyField(Join);
            EditorGUILayout.PropertyField(Leave);
            EditorGUILayout.PropertyField(Begin);
            EditorGUILayout.PropertyField(Selected);
            EditorGUILayout.PropertyField(Deploy);
            EditorGUILayout.PropertyField(Fighting);
            EditorGUILayout.PropertyField(End_4Star);
            EditorGUILayout.PropertyField(End_3Star);
            EditorGUILayout.PropertyField(End_Not3Star);
            EditorGUILayout.PropertyField(End_Fail);
            EditorGUILayout.PropertyField(Touch);
        }
        else if (type.enumValueIndex == 3)
        {
            EditorGUILayout.PropertyField(atk);
            EditorGUILayout.PropertyField(def);
            EditorGUILayout.PropertyField(magicDef);
            EditorGUILayout.PropertyField(life);
            EditorGUILayout.PropertyField(maxBlock);
            EditorGUILayout.PropertyField(cost);
            EditorGUILayout.PropertyField(consumPlace);
            EditorGUILayout.PropertyField(reTime);
            EditorGUILayout.PropertyField(maxLevel);
            EditorGUILayout.PropertyField(Description);
        }
        else if (type.enumValueIndex == 4)
        {
            EditorGUILayout.PropertyField(growingAtk);
            EditorGUILayout.PropertyField(growingDef);
            EditorGUILayout.PropertyField(growingLife);
            EditorGUILayout.PropertyField(elitismAtk);
            EditorGUILayout.PropertyField(elitismDef);
            EditorGUILayout.PropertyField(elitismLife);
            EditorGUILayout.PropertyField(elitismMagicDef);
            EditorGUILayout.PropertyField(elitismBlock);
            EditorGUILayout.PropertyField(elitismExp);
            EditorGUILayout.PropertyField(elitismCost);
        }
        else
        {
            EditorGUILayout.PropertyField(skillType);
            if (skillType.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(skillName0);
                EditorGUILayout.PropertyField(description0);
                EditorGUILayout.PropertyField(initSP0);
                EditorGUILayout.PropertyField(maxSP0);
                EditorGUILayout.PropertyField(duration0);
                EditorGUILayout.PropertyField(expNeed0);
                EditorGUILayout.PropertyField(costNeed0);
                EditorGUILayout.PropertyField(skill0_recoverType);
                EditorGUILayout.PropertyField(skill0_releaseType);
            }
            else if (skillType.enumValueIndex == 1)
            {
                EditorGUILayout.PropertyField(skillName1);
                EditorGUILayout.PropertyField(description1);
                EditorGUILayout.PropertyField(initSP1);
                EditorGUILayout.PropertyField(maxSP1);
                EditorGUILayout.PropertyField(duration1);
                EditorGUILayout.PropertyField(expNeed1);
                EditorGUILayout.PropertyField(costNeed1);
                EditorGUILayout.PropertyField(skill1_recoverType);
                EditorGUILayout.PropertyField(skill1_releaseType);
            }
            else
            {
                EditorGUILayout.PropertyField(skillName2);
                EditorGUILayout.PropertyField(description2);
                EditorGUILayout.PropertyField(initSP2);
                EditorGUILayout.PropertyField(maxSP2);
                EditorGUILayout.PropertyField(duration2);
                EditorGUILayout.PropertyField(expNeed2);
                EditorGUILayout.PropertyField(costNeed2);
                EditorGUILayout.PropertyField(skill2_recoverType);
                EditorGUILayout.PropertyField(skill2_releaseType);
            }
        }

        od_.ApplyModifiedProperties();
    }



}
