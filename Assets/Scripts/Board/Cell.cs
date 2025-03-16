using System;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int BoardX { get; private set; }

    public int BoardY { get; private set; }

    public NormalItem Item { get; private set; }

    public bool IsEmpty => Item == null;
    public bool IsInteractable = true;
    public bool IsBottomCell;
    public static Action<Cell> onCellSelected;
    public static Action<Cell> onCellDeselected;

    public void Setup(int cellX, int cellY, bool IsBottomCell = false)
    {
        this.BoardX = cellX;
        this.BoardY = cellY;
        this.IsBottomCell = IsBottomCell;
    }

    public void Selected()
    {
        if (!IsInteractable) return;
        onCellSelected?.Invoke(this);
        IsInteractable = false;
        IsBottomCell = true;
        // Clear();
    }

    public void Deselected()
    {
        onCellDeselected?.Invoke(this);
        IsInteractable = true;
        IsBottomCell = false;
        // Clear();
    }

    public void Assign(NormalItem item)
    {
        Item = item;
        Item.SetCell(this);
    }

    public void ApplyItemPosition(bool withAppearAnimation)
    {
        Item.SetViewPosition(this.transform.position);

        if (withAppearAnimation)
        {
            Item.ShowAppearAnimation();
        }
    }

    internal void Clear()
    {
        if (Item != null)
        {
            Item.Clear();
            Item = null;
        }
    }

    internal NormalItem.eNormalType GetType() {
        return Item.ItemType;
    }

    internal bool IsSameType(Cell other)
    {
        return Item != null && other.Item != null && Item.IsSameType(other.Item);
    }

    internal void ExplodeItem()
    {
        if (Item == null) return;

        Item.ExplodeView();
        Item = null;
    }

    internal void ApplyItemMoveToPosition()
    {
        Item.AnimationMoveToPosition();
    }
}
