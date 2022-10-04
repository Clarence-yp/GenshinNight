using UnityEditor;

// 确定我们需要自定义编辑器的组件
[CustomEditor(typeof(enemyInfo))]
public class enemyInfoEnum : Editor
{
    // 序列化对象
    private SerializedObject ei_;

    // 序列化属性
    private SerializedProperty type;
    
    //"基本资料"0中变量
    private SerializedProperty  Name;
    private SerializedProperty  enemyPrefab;
    private SerializedProperty  elementType;
    private SerializedProperty  aimingMode;
    private SerializedProperty  dropCost;
    private SerializedProperty  dropExp;
    private SerializedProperty consumeHP;
    private SerializedProperty speed;
    private SerializedProperty mass;
    private SerializedProperty isDrone;
    private SerializedProperty headImage;
    
    //"基础属性"1中变量
    private SerializedProperty atk;
    private SerializedProperty def;
    private SerializedProperty magicDef;
    private SerializedProperty life;
    private SerializedProperty consumeBlock;
    private SerializedProperty minAtkInterval;
    private SerializedProperty  elementalMastery;
    private SerializedProperty  elementalDamage;
    private SerializedProperty  elementalResistance;
    private SerializedProperty  spRecharge;
    private SerializedProperty  shieldStrength;
    private SerializedProperty  talentDescription;
    
    //"技能数据"2中变量
    private SerializedProperty skillName;
    private SerializedProperty skillDescription;
    private SerializedProperty initSP;
    private SerializedProperty maxSP;
    private SerializedProperty duration;
    private SerializedProperty skill_recoverType;
    private SerializedProperty skill_releaseType;
    



    void OnEnable()
    {
        // 获取当前的序列化对象（target：当前检视面板中显示的对象）
        ei_ = new SerializedObject(target);
        
        // 抓取对应的序列化属性
        type = ei_.FindProperty("type");

        //"基本资料"0中变量
        Name = ei_.FindProperty("Name");
        enemyPrefab = ei_.FindProperty("enemyPrefab");
        elementType = ei_.FindProperty("elementType");
        aimingMode = ei_.FindProperty("aimingMode");
        dropCost = ei_.FindProperty("dropCost");
        dropExp = ei_.FindProperty("dropExp");
        consumeHP = ei_.FindProperty("consumeHP");
        speed = ei_.FindProperty("speed");
        mass = ei_.FindProperty("mass");
        isDrone = ei_.FindProperty("isDrone");
        headImage = ei_.FindProperty("headImage");

        //"基础属性"1中变量
        atk = ei_.FindProperty("atk");
        def = ei_.FindProperty("def");
        magicDef = ei_.FindProperty("magicDef");
        life = ei_.FindProperty("life");
        consumeBlock = ei_.FindProperty("consumeBlock");
        minAtkInterval = ei_.FindProperty("minAtkInterval");
        elementalMastery = ei_.FindProperty("elementalMastery");
        elementalDamage = ei_.FindProperty("elementalDamage");
        elementalResistance = ei_.FindProperty("elementalResistance");
        spRecharge = ei_.FindProperty("spRecharge");
        shieldStrength = ei_.FindProperty("shieldStrength");
        talentDescription = ei_.FindProperty("talentDescription");
    
        //"技能数据"2中变量
        skillName = ei_.FindProperty("skillName");
        skillDescription = ei_.FindProperty("skillDescription");
        initSP = ei_.FindProperty("initSP");
        maxSP = ei_.FindProperty("maxSP");
        duration = ei_.FindProperty("duration");
        skill_recoverType = ei_.FindProperty("skill_recoverType");
        skill_releaseType = ei_.FindProperty("skill_releaseType");
    }
    
    
    public override void OnInspectorGUI()
    {
        // 从物体上抓取最新的信息
        ei_.Update();

        EditorGUILayout.PropertyField(type);

        if (type.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(Name);
            EditorGUILayout.PropertyField(enemyPrefab);
            EditorGUILayout.PropertyField(elementType);
            EditorGUILayout.PropertyField(aimingMode);
            EditorGUILayout.PropertyField(dropCost);
            EditorGUILayout.PropertyField(dropExp);
            EditorGUILayout.PropertyField(consumeHP);
            EditorGUILayout.PropertyField(speed);
            EditorGUILayout.PropertyField(mass);
            EditorGUILayout.PropertyField(isDrone);
            EditorGUILayout.PropertyField(headImage);
        }
        else if (type.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(atk);
            EditorGUILayout.PropertyField(def);
            EditorGUILayout.PropertyField(magicDef);
            EditorGUILayout.PropertyField(life);
            EditorGUILayout.PropertyField(consumeBlock);
            EditorGUILayout.PropertyField(minAtkInterval);
            EditorGUILayout.PropertyField(elementalMastery);
            EditorGUILayout.PropertyField(elementalDamage);
            EditorGUILayout.PropertyField(elementalResistance);
            EditorGUILayout.PropertyField(spRecharge);
            EditorGUILayout.PropertyField(shieldStrength);
            EditorGUILayout.PropertyField(talentDescription);
        }
        else
        {
            EditorGUILayout.PropertyField(skillName);
            EditorGUILayout.PropertyField(initSP);
            EditorGUILayout.PropertyField(maxSP);
            EditorGUILayout.PropertyField(duration);
            EditorGUILayout.PropertyField(skill_recoverType);
            EditorGUILayout.PropertyField(skill_releaseType);
            EditorGUILayout.PropertyField(skillDescription);
        }



        ei_.ApplyModifiedProperties();
    }
    
    
    
}

