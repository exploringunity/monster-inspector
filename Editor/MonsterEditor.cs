using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Monster))]
public class MonsterEditor : Editor
{
    bool attacksExpanded = true;
    Monster targetMonster;
    Label attacksLbl;
    VisualElement attacksContainer;

    public override VisualElement CreateInspectorGUI()
    {
        targetMonster = (Monster)target;
        var uxmlTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MonsterEditor.uxml");
        var ui = uxmlTemplate.CloneTree();
        attacksContainer = ui.Q<VisualElement>("attacksContainer");
        attacksLbl = ui.Q<Label>("attacksLbl");
        attacksLbl.RegisterCallback<MouseDownEvent>(ToggleAttacksListDisplay);
        var addAttackBtn = ui.Q<Button>("addAttackBtn");
        addAttackBtn.clicked += AddAttack;
        SyncAttacksUI();
        Undo.undoRedoPerformed += SyncAttacksUI;
        return ui;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= SyncAttacksUI;
    }

    void SyncAttacksUI()
    {
        UpdateAttackListLabel();
        attacksContainer.Clear();
        for (var i = 0; i < targetMonster.attacks.Count; i++)
        {
            var newAttack = new MonsterAttackEditor(targetMonster, i, DeleteAttack, DuplicateAttack);
            attacksContainer.Add(newAttack);
        }
    }

    void AddAttack()
    {
        Undo.RecordObject(targetMonster, "Add Monster Attack");
        targetMonster.attacks.Add(string.Empty);
        SyncAttacksUI();
    }

    void DeleteAttack(int attackIdx)
    {
        Undo.RecordObject(targetMonster, $"Delete Monster Attack #{attackIdx}");
        targetMonster.attacks.RemoveAt(attackIdx);
        SyncAttacksUI();
    }

    void DuplicateAttack(int attackIdx)
    {
        Undo.RecordObject(targetMonster, $"Duplicating Monster Attack #{attackIdx}");
        targetMonster.attacks.Insert(attackIdx, targetMonster.attacks[attackIdx]);
        SyncAttacksUI();
    }

    void ToggleAttacksListDisplay(MouseDownEvent evt)
    {
        var newStyle = attacksExpanded ? DisplayStyle.None : DisplayStyle.Flex;
        attacksContainer.style.display = newStyle;
        attacksExpanded = !attacksExpanded;
        UpdateAttackListLabel();
    }

    void UpdateAttackListLabel()
    {
        var icon = attacksExpanded ? "▼" : "►";
        attacksLbl.text = $"{icon} Attacks ({targetMonster.attacks.Count})";
    }
}
