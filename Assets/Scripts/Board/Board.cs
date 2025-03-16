using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Board
{
    private int boardSizeX;

    private int boardSizeY;

    public Cell[,] m_cells;

    private Transform m_root;

    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[boardSizeX, boardSizeY];

        CreateBoard();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_TILE);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }
    }

    internal void Fill()
    {
        var enumValues = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().ToList();
        int n = enumValues.Count;
        Queue<NormalItem.eNormalType> result = GenerateList(enumValues, n);

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                NormalItem item = new NormalItem();

                item.SetType(result.Dequeue());
                item.SetView();
                item.SetViewRoot(m_root);

                cell.Assign(item);
                cell.ApplyItemPosition(false);
            }
        }
    }

    public void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                cell?.Clear();

                GameObject.Destroy(cell?.gameObject);
                m_cells[x, y] = null;
            }
        }
    }

    private Queue<NormalItem.eNormalType> GenerateList(List<NormalItem.eNormalType> enumValues, int n)
    {
        Queue<NormalItem.eNormalType> result = new Queue<NormalItem.eNormalType>();
        int totalElements = boardSizeX * boardSizeY;
        int baseCountPerEnum = totalElements / n; 
        int remainder = totalElements % n; 

        int adjustedBaseCount = baseCountPerEnum - (baseCountPerEnum % 3);
        if (adjustedBaseCount < 3) adjustedBaseCount = 3; 

        Dictionary<NormalItem.eNormalType, int> counts = new Dictionary<NormalItem.eNormalType, int>();
        int currentTotal = 0;

        foreach (var value in enumValues)
        {
            counts[value] = adjustedBaseCount;
            currentTotal += adjustedBaseCount;
        }

        int remaining = totalElements - currentTotal;
        int i = 0;
        while (remaining > 0 && i < enumValues.Count)
        {
            counts[enumValues[i]] += 3; 
            remaining -= 3;
            i++;
        }

        List<NormalItem.eNormalType> tempList = new List<NormalItem.eNormalType>();
        foreach (var value in enumValues)
        {
            for (int j = 0; j < counts[value]; j++)
            {
                tempList.Add(value);
            }
        }

        Random rng = new Random();
        tempList = tempList.OrderBy(x => rng.Next()).ToList();

        foreach (var item in tempList)
        {
            result.Enqueue(item);
        }

        return result;
    }
}
