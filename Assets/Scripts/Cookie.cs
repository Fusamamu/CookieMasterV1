using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Cookie : MonoBehaviour
{
    private Grid _grid;

    public int Row;
    public int Column;

    public enum TYPE
    {
        CHERRYTOP   = 0,
        HEART       = 1,
        SQUARE      = 2,
        CREAM       = 3
    }

    public TYPE type;

    public Vector3 rightPos;
    public Vector3 upPos;

    public bool isMoving = false;

    private void Start()
    {
        _grid       = Grid.sharedInstance;

        rightPos    = transform.position + Vector3.right    + new Vector3(_grid.Padding, 0, 0);
        upPos       = transform.position + Vector3.forward  + new Vector3(_grid.Padding, 0, 0);
    }

    
}
