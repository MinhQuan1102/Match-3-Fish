using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BottomCells
{
    private int bottomCellSize;
    private int boardSizeX;
    private int boardSizeY;
    public bool IsTimerMode { get; set; } = false;
    public List<Cell> cells;
    private Transform root;


    public BottomCells(Transform transform, GameSettings gameSettings) 
    {
        root = transform;
        this.bottomCellSize = gameSettings.BottomCells;
        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;
        cells = new List<Cell>(bottomCellSize + 1);
        Cell.onCellSelected += OnCellSelected;
        Cell.onCellDeselected += OnCellDeselected;
        CreateBottomCells();
        foreach(Cell cell in cells) {
            cell.Item.ExplodeView();
        }
    }

    private void CreateBottomCells() 
    {
        Vector3 origin = new Vector3(0, 0, 0);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_TILE);
        for (int x = 0; x < bottomCellSize; x++)
        {
            int y = 0;
            GameObject go = GameObject.Instantiate(prefabBG);

            go.transform.position = origin + new Vector3(x - 2, y - 4, 0);
            go.transform.SetParent(root);

            Cell cell = go.GetComponent<Cell>();
            if (IsTimerMode) {
                cell.IsInteractable = true;
            } else {
                cell.IsInteractable = false;
            }
        }
    }

    private void OnCellSelected(Cell cell) {
        cells.Add(cell);
   
        if (cells.Count <= bottomCellSize) {
            cell.Item.MoveView(new Vector3(cells.Count - 3, -4, 0));
        }
        if (IsTimerMode) 
        {
            Fill();
        }
        CheckAndRemoveTriplet();
    }

    private void OnCellDeselected(Cell cell) {
        if (IsTimerMode) {
            cell.Item.MoveView(new Vector3(cell.BoardX - (boardSizeX * 0.5f - 0.5f), cell.BoardY - (boardSizeY * 0.5f - 0.5f), 0));
            cells.RemoveAll((c) => c.BoardX == cell.BoardX && c.BoardY == cell.BoardY);
            Fill();
        }
    }

    private void CheckAndRemoveTriplet() 
    {
        Dictionary<NormalItem.eNormalType, int> counts = new Dictionary<NormalItem.eNormalType, int>();
        foreach(Cell cell in cells) {
            NormalItem.eNormalType cellType = cell.GetType();
            if (counts.ContainsKey(cellType)) 
            {
                counts[cellType] ++;
            } 
            else
            {
                counts[cellType] = 1;
            }
        }

        foreach(KeyValuePair<NormalItem.eNormalType, int> kvp in counts) 
        {
            if (kvp.Value >= 3) 
            {
                foreach(Cell cell in cells) {
                    if (cell.GetType() == kvp.Key) {
                        cell.Item.ExplodeView();
                    }
                }
                cells.RemoveAll(item => item.GetType() == kvp.Key);
                for (int i = 0; i < cells.Count; i++)
                {
                    Cell cell = cells[i];
                    cell.Item.MoveView(new Vector3(i - 2, -4, 0));
                }
            }
        }

    }

    private void Fill() 
    {
        for (int x = 0; x < bottomCellSize; x++)
        {
            Cell cell = root.GetComponentsInChildren<Cell>()[x];
            if (x < cells.Count) {
                cell.IsInteractable = true;
                Cell storedCell = cells[x];
                NormalItem item = new NormalItem();
                item.SetType(storedCell.GetType());
                item.SetViewRoot(root);

                cell.Assign(item);
                cell.Setup(storedCell.BoardX, storedCell.BoardY, true);
            }
            else {
                cell.IsInteractable = false;
            }
        }
    }
}
