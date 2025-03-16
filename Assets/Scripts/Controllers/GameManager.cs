using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode
    {
        TIMER,
        NORMAL,
        AUTOPLAY, 
        AUTOLOSE
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
        GAME_CLEARED
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }


    private GameSettings m_gameSettings;


    private BoardController m_boardController;
    private BottomCellsController m_bottomCellsController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }


    internal void SetState(eStateGame state)
    {
        State = state;

        if(State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        m_bottomCellsController = new GameObject("BottomCellsController").AddComponent<BottomCellsController>();
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_bottomCellsController.StartGame(this, m_gameSettings);

        if (mode == eLevelMode.NORMAL)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelSlot>();
            m_levelCondition.Setup(m_gameSettings.BottomCells, m_uiMenu.GetLevelConditionView(), m_boardController, m_gameSettings, m_bottomCellsController);
            m_bottomCellsController.m_bottomCells.IsTimerMode = false;
        }
        else if (mode == eLevelMode.TIMER)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(m_gameSettings.LevelTime, m_uiMenu.GetLevelConditionView(), this, m_boardController);
            m_bottomCellsController.m_bottomCells.IsTimerMode = true;
        } else if (mode == eLevelMode.AUTOPLAY) {
            m_levelCondition = this.gameObject.AddComponent<LevelSlot>();
            m_levelCondition.Setup(m_gameSettings.BottomCells, m_uiMenu.GetLevelConditionView(), m_boardController, m_gameSettings, m_bottomCellsController);
            m_boardController.IsAutoplay = true;
            m_bottomCellsController.m_bottomCells.IsTimerMode = false;
        } else if (mode == eLevelMode.AUTOLOSE) {
            m_levelCondition = this.gameObject.AddComponent<LevelSlot>();
            m_levelCondition.Setup(m_gameSettings.BottomCells, m_uiMenu.GetLevelConditionView(), m_boardController, m_gameSettings, m_bottomCellsController);
            m_boardController.IsAutoLose = true;
            m_bottomCellsController.m_bottomCells.IsTimerMode = false;

        }
        m_boardController.StartGame(this, m_gameSettings, m_bottomCellsController);

        m_levelCondition.ConditionFailEvent += GameOver;
        m_levelCondition.ConditionCompleteEvent += GameComplete;

        State = eStateGame.GAME_STARTED;
    }

    public void GameOver()
    {
        StartCoroutine(WaitBoardController());
    }
    public void GameComplete()
    {
        StartCoroutine(WaitWinController());
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
    }

    private IEnumerator WaitBoardController()
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        State = eStateGame.GAME_OVER;

        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameOver;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }

    private IEnumerator WaitWinController()
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        State = eStateGame.GAME_CLEARED;

        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameComplete;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}
