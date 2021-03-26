using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Matching : MonoBehaviour
{

    public static Matching sharedInstance { get; set; }

    private Grid            _grid;
    private Controller      _controller;

    public  bool isUpdateMatching = false;

    private List<GameObject> _temp_Matched_ROW      = new List<GameObject>();
    private List<GameObject> _temp_Matched_COLUMN   = new List<GameObject>();

    private List<GameObject> _temp_Top_ROW          = new List<GameObject>();
    private List<GameObject> _temp_Right_Column     = new List<GameObject>();

    public int currentMatched_ROW;
    public int currentMatched_COLUMN;

    public UnityEvent ROW_CLEAR;
    public UnityEvent COLUMN_CLEAR;
    public UnityEvent FinishUpdate;

    public struct Match_INFO
    {
        public List<GameObject> matched_row;
        public List<GameObject> matched_column;

        public bool isMatched_ROW;
        public bool isMatched_COLUMN;

    }

    private Match_INFO _match_INFO;

    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        _grid       = Grid.sharedInstance;
        _controller = Controller.sharedInstane;
    }

    private void Update()
    {
        if(_controller.currentAction == Controller.ACTION.IDLE)
        {
            if (!isUpdateMatching)
            {
                FindMatching();
            }
            else
            {
                if (_match_INFO.isMatched_ROW)
                {
                    Remove_COOKIES(_temp_Matched_ROW);
                    ROW_CLEAR.Invoke();
                }

                if (_match_INFO.isMatched_COLUMN)
                {
                    Remove_COOKIES(_temp_Matched_COLUMN);
                    COLUMN_CLEAR.Invoke();
                }

                Move_COOKIES_POSITION();

                if (AllStopMoving())
                {
                    Update_GRIDBLOCKS();

                    _temp_Matched_COLUMN.Clear();
                    _temp_Matched_ROW.Clear();
                    isUpdateMatching = false;
                    FinishUpdate.Invoke();
                }
            }
        }
    }

    private void FindMatching()
    {
        bool _matchFound = false;

        _match_INFO = GetMatching_COOKIES();

        if (_match_INFO.matched_row.Count != 0)
        {
            _temp_Matched_ROW   = _match_INFO.matched_row;
            currentMatched_ROW  = _temp_Matched_ROW[0].GetComponent<Cookie>().Row;

            _matchFound         = true;
            isUpdateMatching    = true;
        }

        if (!_match_INFO.isMatched_ROW)
        {
            if (_match_INFO.matched_column.Count != 0)
            {
                _temp_Matched_COLUMN = _match_INFO.matched_column;
                currentMatched_COLUMN = _temp_Matched_COLUMN[0].GetComponent<Cookie>().Column;
                _matchFound = true;
                isUpdateMatching = true;
            }
        }

        foreach (GameObject cookie in _temp_Matched_ROW)
        {
            AnimateCookie(cookie);
        }

        foreach (GameObject cookie in _temp_Matched_COLUMN)
        {
            AnimateCookie(cookie);
        }

        if (_matchFound)
        {
            if(_match_INFO.isMatched_ROW)
                _temp_Top_ROW = _grid.Create_New_TopRow();

            if (_match_INFO.isMatched_COLUMN)
                _temp_Right_Column = _grid.Create_New_RightColumn();
        }

        _controller.currentAction = Controller.ACTION.IDLE;
    }

    private void AnimateCookie(GameObject _cookie)
    {
        _cookie.GetComponent<Rigidbody>().useGravity = true;

        float random_x = Random.Range(-2, 2);
        float random_y = Random.Range(8, 10);
        float random_z = Random.Range(-2, 2);

        _cookie.GetComponent<Rigidbody>().AddForce(new Vector3(0, random_y, 0), ForceMode.Impulse);
        _cookie.GetComponent<Rigidbody>().AddTorque(new Vector3(random_x, random_y, random_z), ForceMode.Impulse);
    }

    private Match_INFO GetMatching_COOKIES()
    {
        Match_INFO _match_INFO  = new Match_INFO();

        _match_INFO.matched_row     = new List<GameObject>();
        _match_INFO.matched_column  = new List<GameObject>();

        foreach (Cookie.TYPE cookieTYPE in System.Enum.GetValues(typeof(Cookie.TYPE)))
        {
            for (int j = 1; j < _grid.ROW - 1; j++)
            {
                if (ROW_isMatching(j, cookieTYPE))
                {
                    _match_INFO.matched_row         = _grid.GetRow(j);
                    _match_INFO.isMatched_ROW       = true;
                    _match_INFO.isMatched_COLUMN    = false;
                }
            }

            for (int i = 1; i < _grid.COLUMN - 1; i++)
            {
                if(COLUMN_isMatching(i, cookieTYPE))
                {
                    _match_INFO.matched_column      = _grid.GetColumn(i);
                    _match_INFO.isMatched_ROW       = false;
                    _match_INFO.isMatched_COLUMN    = true;
                }
            }
        }

        return _match_INFO;
    }

    private bool ROW_isMatching(int _row, Cookie.TYPE checkedType)
    {
        bool _isMatched = false;

        List<GameObject> checked_ROW = _grid.GetRow(_row);

        foreach(GameObject cookie in checked_ROW)
        {
            if (cookie.GetComponent<Cookie>().type == checkedType)
            {
                _isMatched = true;
            }
            else
            {
                _isMatched = false;
                break;
            }
        }
        return _isMatched;
    }

    private bool COLUMN_isMatching(int _column, Cookie.TYPE checkedType)
    {
        bool _isMatched = false;

        List<GameObject> checkedColumn = _grid.GetColumn(_column);

        foreach (GameObject cookie in checkedColumn)
        {
            if (cookie.GetComponent<Cookie>().type == checkedType)
            {
                _isMatched = true;
            }
            else
            {
                _isMatched = false;
                break;
            }
        }
        return _isMatched;
    }

    private void Remove_COOKIES(List<GameObject> _cookies)
    {
        foreach (GameObject cookie in _cookies)
        {
            if (cookie != null)
            {
                if (cookie.transform.position.y > 1.5f)
                {
                    int _column = cookie.GetComponent<Cookie>().Column;
                    int _row = cookie.GetComponent<Cookie>().Row;
                    _grid.GridBlocks[_column, _row] = null;

                    Destroy(cookie);
                }
            }
        }
    }

    private void Move_COOKIES_POSITION()
    {
        if (_match_INFO.isMatched_ROW)
        {
            for (int i = 1; i < _grid.COLUMN - 1; i++)
            {
                _grid.Move(_temp_Top_ROW[i - 1], _grid.GridPositions[i, _grid.ROW - 2]);

                for (int j = currentMatched_ROW + 1; j < _grid.ROW - 1; j++)
                {
                    _grid.Move(_grid.GridBlocks[i, j], _grid.GridPositions[i, j - 1]);
                }
            }
        }

        if (_match_INFO.isMatched_COLUMN)
        {
            for (int j = 1; j < _grid.ROW - 1; j++)
            {
                _grid.Move(_temp_Right_Column[j - 1], _grid.GridPositions[_grid.COLUMN - 2, j]);

                for (int i = currentMatched_COLUMN + 1; i < _grid.COLUMN - 1; i++)
                {
                    _grid.Move(_grid.GridBlocks[i, j], _grid.GridPositions[i - 1, j]);
                }
            }
        }
    }

    private bool AllStopMoving()
    {
        bool _all_StopMoving = false;

        if (_match_INFO.isMatched_ROW)
        {
            for (int j = currentMatched_ROW + 1; j < _grid.ROW - 3; j++)
            {
                for (int i = 1; i < _grid.COLUMN - 2; i++)
                {
                    _all_StopMoving = _grid.GridBlocks[i, j].GetComponent<Cookie>().isMoving ? false : true;
                    if (!_all_StopMoving) break;
                }
            }

            foreach (GameObject _cookie in _temp_Top_ROW)
            {
                _all_StopMoving = _cookie.GetComponent<Cookie>().isMoving ? false : true;
                if (!_all_StopMoving) break;
            }
        }

        if (_match_INFO.isMatched_COLUMN)
        {
            for(int i = currentMatched_COLUMN + 1; i < _grid.COLUMN - 3; i++)
            {
                for(int j = 1; j < _grid.ROW - 2; j++)
                {
                    _all_StopMoving = _grid.GridBlocks[i, j].GetComponent<Cookie>().isMoving ? false : true;
                    if (!_all_StopMoving) break;
                }
            }

            foreach(GameObject _cookie in _temp_Right_Column)
            {
                _all_StopMoving = _cookie.GetComponent<Cookie>().isMoving ? false : true;
                if (!_all_StopMoving) break;
            }
        }

        return _all_StopMoving;
    }

    private void Update_GRIDBLOCKS()
    {
        if (_match_INFO.isMatched_ROW)
        {
            for (int j = currentMatched_ROW; j < _grid.ROW - 2; j++)
            {
                for (int i = 1; i < _grid.COLUMN - 1; i++)
                {
                    _grid.GridBlocks[i, j] = _grid.GridBlocks[i, j + 1];
                    _grid.GridBlocks[i, j].GetComponent<Cookie>().Row = j;
                    _grid.GridBlocks[i, j].name = "(" + i + ", " + j + ")";
                }
            }

            int index = 0;

            for (int i = 1; i < _grid.COLUMN - 1; i++)
            {
                _grid.GridBlocks[i, _grid.ROW - 2] = _temp_Top_ROW[index];
                _grid.GridBlocks[i, _grid.ROW - 2].GetComponent<Cookie>().Row = _grid.ROW - 2;
                _grid.GridBlocks[i, _grid.ROW - 2].name = "(" + i + ", " + (_grid.ROW - 2) + ")";
                index++;
            }
        }

        if (_match_INFO.isMatched_COLUMN)
        {
            for(int i = currentMatched_COLUMN; i < _grid.COLUMN - 2; i++)
            {
                for(int j = 1; j < _grid.ROW - 1; j++)
                {
                    _grid.GridBlocks[i, j] = _grid.GridBlocks[i + 1, j];
                    _grid.GridBlocks[i, j].GetComponent<Cookie>().Column = i;
                    _grid.GridBlocks[i, j].name = "(" + i + ", " + j + ")";
                }
            }

            int index = 0;

            for (int j = 1; j < _grid.ROW - 1; j++)
            {
                _grid.GridBlocks[_grid.COLUMN - 2, j] = _temp_Right_Column[index];
                _grid.GridBlocks[_grid.COLUMN - 2, j].GetComponent<Cookie>().Column = _grid.COLUMN - 2;
                _grid.GridBlocks[_grid.COLUMN - 2, j].name = "(" + (_grid.COLUMN - 2) + ", " + j + ")";
                index++;
            }
        }
    }
}
