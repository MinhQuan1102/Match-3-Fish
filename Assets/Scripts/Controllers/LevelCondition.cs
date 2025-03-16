using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    public event Action ConditionCompleteEvent = delegate { };
    public event Action ConditionFailEvent = delegate { };

    protected Text m_txt;

    protected bool m_conditionCompleted = false;
    protected bool m_conditionFailed = false;

    public virtual void Setup(float value, Text txt)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mngr)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mngr, BoardController board)
    {
        m_txt = txt;
    }
    public virtual void Setup(float value, Text txt, BoardController board, GameSettings gameSettings, BottomCellsController bottomCellsController)
    {
        m_txt = txt;
    }

    protected virtual void UpdateText() { }

    protected void OnConditionComplete()
    {
        m_conditionCompleted = true;

        ConditionCompleteEvent();
    }

    protected void OnConditionFail()
    {
        m_conditionFailed = true;

        ConditionFailEvent();
    }

    protected virtual void OnDestroy()
    {

    }
}
