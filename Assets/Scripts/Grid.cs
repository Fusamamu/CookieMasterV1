using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid sharedInstance { get; set; }

    public GameObject[]     Cookie_PREFAB_TYPES = new GameObject[4];

    public GameObject[,]    GridBlocks;
    public Vector3[,]       GridPositions;

    [Range(4,8)] public int ROW      = 8;
    [Range(4,8)] public int COLUMN   = 8;

    public float Block_WIDTH    = 1.0f;
    public float Block_HEIGHT   = 1.0f;

    public float Padding = 0.25f;

    [Range(5.0f, 20.0f)] public float moveSpeed = 5;

    private void Awake()
    {
        sharedInstance = this;

        GridBlocks = new GameObject[COLUMN, ROW];
        GridPositions = new Vector3[COLUMN, ROW];
        InitilizeGrid();
    }

    private void InitilizeGrid()
    {
        for(int i = 0; i < COLUMN; i++)
        {
            for(int j = 0; j < ROW; j++)
            {
                float x = i * (Block_WIDTH  + Padding);
                float z = j * (Block_HEIGHT + Padding);
                Vector3 placement = new Vector3(x, 0, z);

                GridPositions[i, j] = placement;

                if (i == 0 || i == COLUMN - 1 || j == 0 || j == ROW - 1)
                {
                    GridBlocks[i, j] = null;
                }
                else
                {
                    int randomInt = Random.Range(0, Cookie_PREFAB_TYPES.Length);
                    GameObject randomCookie = Cookie_PREFAB_TYPES[randomInt];
                    
                    GameObject cookie = Instantiate(randomCookie, placement, Quaternion.identity, this.transform);
                    cookie.name = "(" + i + ", " + j + ")";
                    cookie.GetComponent<Cookie>().Row       = j;
                    cookie.GetComponent<Cookie>().Column    = i;
                    cookie.GetComponent<Cookie>().type      = (Cookie.TYPE)randomInt;
                    GridBlocks[i, j] = cookie;
                }
            }
        }
    }

    public List<GameObject> Get_ROWCOLUMN(int j, int i)
    {
        List<GameObject> _rowColumn = GetRow(j);
        _rowColumn.Remove(GridBlocks[i, j]);
        _rowColumn.AddRange(GetColumn(i));
        return _rowColumn;
    }

    public List<GameObject> GetRow(int j)
    {
        List<GameObject> _row = new List<GameObject>();
        for(int i = 1; i < COLUMN - 1; i++)
        {
            _row.Add(GridBlocks[i, j]);
        }
        return _row;
    }

    public List<GameObject> GetColumn(int i)
    {
        List<GameObject> _column = new List<GameObject>();
        for(int j = 1; j < ROW - 1; j++)
        {
            _column.Add(GridBlocks[i, j]);
        }
        return _column;
    }

    public List<GameObject> Create_New_TopRow()
    {
        List<GameObject> _topRow = new List<GameObject>();

        for(int i = 1; i < COLUMN - 1; i++)
        {
            int randomInt = Random.Range(0, Cookie_PREFAB_TYPES.Length);

            Vector3 _placement = GridPositions[i, ROW - 1];
            GameObject _cookie = Instantiate(Cookie_PREFAB_TYPES[randomInt], _placement, Quaternion.identity, this.transform);
            _cookie.GetComponent<Cookie>().name     = "(" + i + ", " + (ROW - 1) + ")";
            _cookie.GetComponent<Cookie>().Row      = ROW - 1;
            _cookie.GetComponent<Cookie>().Column   = i;
            _cookie.GetComponent<Cookie>().type     = (Cookie.TYPE)randomInt;

            _topRow.Add(_cookie);
        }

        return _topRow;
    }

    public List<GameObject> Create_New_RightColumn()
    {
        List<GameObject> _rightColumn = new List<GameObject>();

        for(int i = 1; i < ROW - 1; i++)
        {
            int randomInt = Random.Range(0, Cookie_PREFAB_TYPES.Length);

            Vector3 _placement = GridPositions[COLUMN -1 , i];
            GameObject _cookie = Instantiate(Cookie_PREFAB_TYPES[randomInt], _placement, Quaternion.identity, this.transform);
            _cookie.GetComponent<Cookie>().name         = "(" + (COLUMN - 1) + ", " + i + ")";
            _cookie.GetComponent<Cookie>().Row          = i;
            _cookie.GetComponent<Cookie>().Column       = COLUMN - 1;
            _cookie.GetComponent<Cookie>().type         = (Cookie.TYPE)randomInt;

            _rightColumn.Add(_cookie);
        }
        return _rightColumn;
    }

    public void Move(GameObject cookie, Vector3 target)
    {
        cookie.GetComponent<Cookie>().isMoving = true;

        if (Vector3.Distance(cookie.transform.position, target) > 0.02)
        {
            cookie.transform.position = Vector3.Lerp(cookie.transform.position, target, moveSpeed * Time.deltaTime);
        }
        else
        {
            cookie.transform.position = target;
            cookie.GetComponent<Cookie>().isMoving = false;
        }
    }
}
