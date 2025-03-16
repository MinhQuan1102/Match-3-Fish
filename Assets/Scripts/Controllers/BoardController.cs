using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnClickEvent = delegate { };

    public bool IsBusy { get; private set; }
    public bool IsAutoplay { get; set; }
    public bool IsAutoLose { get; set; }

    public Board m_board;

    private GameManager m_gameManager;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;
    private BottomCellsController m_bottomCellsController;

    private bool m_gameOver;
    private bool isProcessingClick = false;
    private bool isAutoMode = false; 
    private float autoInterval = 0.5f;

    public void StartGame(GameManager gameManager, GameSettings gameSettings, BottomCellsController bottomCellsController)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);
        m_bottomCellsController = bottomCellsController;

        Fill();
        if (IsAutoplay || IsAutoLose) {
            StartAutomode();
        } 
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }
    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;
        if (isProcessingClick) return;

        if (!IsAutoplay && !IsAutoLose) {
            if (Input.GetMouseButtonDown(0))
            {
                isProcessingClick = true;
                var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    m_hitCollider = hit.collider;
                }
                Cell cell = m_hitCollider.GetComponent<Cell>();
                if (cell.IsBottomCell) {
                    cell.Deselected();
                } else if (cell != null && cell.IsInteractable) {
                    cell.Selected();
                    OnClickEvent();
                    int boardX = cell.BoardX;
                    int boardY = cell.BoardY;
                    m_board.m_cells[boardX, boardY] = null;     
                }
                
                StartCoroutine(ResetClickFlag());
            }
        }

    }

    public bool IsBoardCleared() {
        int rows = m_board.m_cells.GetLength(0); 
        int cols = m_board.m_cells.GetLength(1); 

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (m_board.m_cells[i, j] && m_board.m_cells[i, j].IsInteractable)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void ResetRayCast()
    {
        m_hitCollider = null;
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    private IEnumerator ResetClickFlag()
    {
        yield return null; 
        isProcessingClick = false;
    }

    private void StartAutomode() {
        isAutoMode = true;
        StartCoroutine(AutoSelectCoroutine());
    }

    private void StopAutomode() {
        isAutoMode = false;
        StopCoroutine(AutoSelectCoroutine());
    }

    private IEnumerator AutoSelectCoroutine()
    {
        while (isAutoMode)
        {
            if (!m_gameOver && !IsBusy)
            {
                SelectRandomCell(); 
            }
            yield return new WaitForSeconds(autoInterval); 
        }
    }

    private void SelectRandomCell()
    {
        Cell[,] cells = m_board.m_cells;
        List<Cell> bottomCells = m_bottomCellsController.m_bottomCells.cells;
        List<Cell> availableCells = new List<Cell>();
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j] && cells[i, j].IsInteractable)
                {
                    availableCells.Add(cells[i, j]);
                }
            }
        }

        if (availableCells.Count > 0)
        {
            if (bottomCells.Count == 0) {
                int randomIndex = UnityEngine.Random.Range(0, availableCells.Count);
                Cell selectedCell = availableCells[randomIndex];

                selectedCell.Selected(); 
                OnClickEvent();
                int boardX = selectedCell.BoardX; 
                int boardY = selectedCell.BoardY; 
                m_board.m_cells[boardX, boardY] = null; 
            }
            else if (IsAutoplay)
            {
                Cell firstCell = bottomCells[0];
                NormalItem.eNormalType targetType = firstCell.GetType();

                Cell selectedCell = availableCells.Find(cell => cell.GetType() == targetType);

                if (selectedCell != null)
                {
                    selectedCell.Selected();
                    OnClickEvent();
                    int boardX = selectedCell.BoardX;
                    int boardY = selectedCell.BoardY;
                    m_board.m_cells[boardX, boardY] = null;
                }
            } else if (IsAutoLose) 
            {
                Cell firstCell = bottomCells[0];
                NormalItem.eNormalType targetType = firstCell.GetType();

                Cell selectedCell = availableCells.Find(cell => cell.GetType() != targetType);

                if (selectedCell != null)
                {
                    selectedCell.Selected();
                    OnClickEvent();
                    int boardX = selectedCell.BoardX;
                    int boardY = selectedCell.BoardY;
                    m_board.m_cells[boardX, boardY] = null;
                }
            }
        }
        else
        {
            StopAutomode(); 
        }
    }
}
