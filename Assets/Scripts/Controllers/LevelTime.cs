﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTime : LevelCondition
{
    private float m_time;

    private GameManager m_mngr;
    private BoardController m_board;

    public override void Setup(float value, Text txt, GameManager mngr, BoardController board)
    {
        base.Setup(value, txt, mngr);

        m_mngr = mngr;

        m_time = value;
        m_board = board;
        m_board.OnClickEvent += OnClick;

        UpdateText();
    }

    private void Update()
    {
        if (m_conditionCompleted) return;

        if (m_mngr.State != GameManager.eStateGame.GAME_STARTED) return;

        m_time -= Time.deltaTime;

        UpdateText();

        if (m_time <= -1f)
        {
            OnConditionFail();
        }
    }

    private void OnClick() {
        if (m_conditionCompleted) return;
        if (m_board.IsBoardCleared()) {
            OnConditionComplete();
        }
    }

    protected override void UpdateText()
    {
        if (m_time < 0f) return;

        m_txt.text = string.Format("TIME:\n{0:00}", m_time);
    }
}
