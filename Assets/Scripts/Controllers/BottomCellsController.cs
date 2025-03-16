using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCellsController : MonoBehaviour
{
    public BottomCells m_bottomCells { get; private set; }
    private GameManager m_gameManager;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;
        m_bottomCells = new BottomCells(this.transform, gameSettings);
    }
}
