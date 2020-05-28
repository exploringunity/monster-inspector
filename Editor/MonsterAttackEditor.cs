using System;
using UnityEditor;
using UnityEngine.UIElements;

public class MonsterAttackEditor : VisualElement
{
    TextField attackTxt;
    Monster monster;
    int attackIdx;
    Action<int> deleteCallback;
    Action<int> duplicateCallback;

    public MonsterAttackEditor(Monster monster_,
                              int attackIdx_,
                              Action<int> deleteCallback_,
                              Action<int> duplicateCallback_)
    {
        monster = monster_;
        attackIdx = attackIdx_;
        deleteCallback = deleteCallback_;
        duplicateCallback = duplicateCallback_;
        var uxmlTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MonsterAttackEditor.uxml");
        var ui = uxmlTemplate.CloneTree();
        attackTxt = ui.Q<TextField>("attackTxt");
        attackTxt.RegisterCallback<BlurEvent>(HandleAttackChanged);
        attackTxt.label = $"#{attackIdx}";
        attackTxt.value = monster.attacks[attackIdx];
        var deleteBtn = ui.Q<Button>("deleteBtn");
        deleteBtn.clicked += DeleteAttack;
        var duplicateBtn = ui.Q<Button>("duplicateBtn");
        duplicateBtn.clicked += DuplicateAttack;
        Add(ui);
    }

    void HandleAttackChanged(BlurEvent evt)
    {
        Undo.RecordObject(monster, "Rename Monster Attack");
        monster.attacks[attackIdx] = attackTxt.value;
    }

    void DeleteAttack()
    {
        deleteCallback(attackIdx);
    }

    void DuplicateAttack()
    {
        duplicateCallback(attackIdx);
    }
}
