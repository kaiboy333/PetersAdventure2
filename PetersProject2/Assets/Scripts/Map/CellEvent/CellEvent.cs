using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class CellEvent : MonoBehaviour
{
    private Tilemap tilemap = null;
    protected EventTaskManager eventTaskManager = null;

    public enum CellType
    {
        ON,
        Check,
    }
    public CellType cellType = CellType.ON;

    protected virtual void Start()
    {
        eventTaskManager = FindObjectOfType<EventTaskManager>();

        //タイルマップの位置調整
        tilemap = FindObjectOfType<Tilemap>();

        if (tilemap)
        {
            var cellPos = tilemap.WorldToCell(transform.position);

            var complementPos = new Vector3(tilemap.cellSize.x / 2.0f, tilemap.cellSize.y / 2.0f, 0);

            transform.position = tilemap.CellToWorld(cellPos) + complementPos;
        }
    }

    public abstract void CallEvent();
}
