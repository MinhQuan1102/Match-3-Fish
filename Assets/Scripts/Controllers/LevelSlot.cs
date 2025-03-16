using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : LevelCondition
{
    private BottomCellsController m_bottomCellsController;
    private BoardController m_board;
    private GameSettings m_gameSettings;

    public override void Setup(float value, Text txt, BoardController board, GameSettings gameSettings, BottomCellsController bottomCellsController)
    {
        base.Setup(value, txt);
        m_bottomCellsController = bottomCellsController;
        m_gameSettings = gameSettings;
        m_board = board;
        m_board.OnClickEvent += OnClick;
        UpdateText();
    }

    private void OnClick() {
        if (m_conditionCompleted) return;
        if (m_bottomCellsController.m_bottomCells.cells.Count > m_gameSettings.BottomCells) {
            OnConditionFail();
        }
        if (m_board.IsBoardCleared()) {
            OnConditionComplete();
        }
    }

    protected override void UpdateText()
    {
        m_txt.text = string.Format("MOVES:\n{0}", 12);
    }
}
