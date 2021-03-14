using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller sharedInstane { get; set; }

    private Grid        _grid;
    private MouseSlide  _mouseSlide;

    public GameObject currentSelectedBlock;

    public Color unselectedColor;
    public Color selectedColor;

    public int current_row;
    public int current_column;

    public float slideSpeed = 1;

    private Vector3 targetPos = Vector3.zero;

    public bool isSlidingRow = false;

    private GameObject TempCookie = null;

    enum MOVE_DIR
    {
        left, right , up, down
    }

    public enum ACTION
    {
        IDLE, MATCHING, SLIDEROW_R,SLIDEROW_L, SLIDECOLUMN_U, SLIDECOLUMN_D
    }

    public  ACTION currentAction    = ACTION.IDLE;
    private ACTION previousAction   = ACTION.IDLE;

    private void Awake()
    {
        sharedInstane = this;
    }

    private void Start()
    {
        _grid       = Grid.sharedInstance;
        _mouseSlide = MouseSlide.sharedInstance;

        current_row     = 3;
        current_column  = 3;

        currentSelectedBlock = _grid.GridBlocks[current_column,current_row];
    }

    private void Update()
    {
        if (currentAction == ACTION.IDLE && !Matching.sharedInstance.isUpdateMatching)
        {
            current_column  = Mathf.Clamp(_mouseSlide.COLUMN_onClicked, 1, _grid.COLUMN - 2);
            current_row     = Mathf.Clamp(_mouseSlide.ROW_onClicked, 1, _grid.ROW - 2);

            if (_mouseSlide.mouseAction == MouseSlide.SLIDE_ACTION.RIGHT)
            {
                currentAction = ACTION.SLIDEROW_R;
                TempCookie = CreateTempCookie(currentAction);
            }

            if (_mouseSlide.mouseAction == MouseSlide.SLIDE_ACTION.LEFT)
            {
                currentAction = ACTION.SLIDEROW_L;
                TempCookie = CreateTempCookie(currentAction);
            }

            if (_mouseSlide.mouseAction == MouseSlide.SLIDE_ACTION.UP)
            {
                currentAction = ACTION.SLIDECOLUMN_U;
                TempCookie = CreateTempCookie(currentAction);
            }

            if (_mouseSlide.mouseAction == MouseSlide.SLIDE_ACTION.DOWN)
            {
                currentAction = ACTION.SLIDECOLUMN_D;
                TempCookie = CreateTempCookie(currentAction);
            }
        }

        UpdateSlide(currentAction);
    }

    private GameObject CreateTempCookie(ACTION _action)
    {
        GameObject _tempCookie = null;

        Vector3 spawnPos; 

        switch (_action)
        {
            case ACTION.SLIDEROW_R:
                spawnPos = _grid.GridPositions[0, current_row];
                _tempCookie = Instantiate(_grid.GridBlocks[_grid.COLUMN - 2, current_row], spawnPos, Quaternion.identity, _grid.transform);
                _tempCookie.GetComponent<Cookie>().Column = 0;
                _tempCookie.GetComponent<Cookie>().Row = current_row;
                break;
            case ACTION.SLIDEROW_L:
                spawnPos = _grid.GridPositions[_grid.COLUMN - 1, current_row];
                _tempCookie = Instantiate(_grid.GridBlocks[1, current_row], spawnPos, Quaternion.identity, _grid.transform);
                _tempCookie.GetComponent<Cookie>().Column = _grid.COLUMN - 1;
                _tempCookie.GetComponent<Cookie>().Row = current_row;
                break;
            case ACTION.SLIDECOLUMN_U:
                spawnPos = _grid.GridPositions[current_column, 0];
                _tempCookie = Instantiate(_grid.GridBlocks[current_column, _grid.ROW - 2], spawnPos, Quaternion.identity, _grid.transform);
                _tempCookie.GetComponent<Cookie>().Column = current_column;
                _tempCookie.GetComponent<Cookie>().Row = 0;
                break;
            case ACTION.SLIDECOLUMN_D:
                spawnPos = _grid.GridPositions[current_column, _grid.ROW - 1];
                _tempCookie = Instantiate(_grid.GridBlocks[current_column, 1], spawnPos, Quaternion.identity, _grid.transform);
                _tempCookie.GetComponent<Cookie>().Column = current_column;
                _tempCookie.GetComponent<Cookie>().Row = _grid.ROW - 1;
                break;
        }

        return _tempCookie;
    }

    private void UpdateSlide(ACTION _action)
    {
        List<GameObject> _row       = _grid.GetRow(current_row);
        List<GameObject> _column    = _grid.GetColumn(current_column);

        switch (_action)
        {
            case ACTION.SLIDEROW_R:
                Move_Cookies(_row);
                break;
            case ACTION.SLIDEROW_L:
                Move_Cookies(_row);
                break;
            case ACTION.SLIDECOLUMN_U:
                Move_Cookies(_column);
                break;
            case ACTION.SLIDECOLUMN_D:
                Move_Cookies(_column);
                break;
        }
    }

    //Move Cookies as a whole by selected ROW/COLUMN//
    private void Move_Cookies(List<GameObject> _cookies)
    {
        Move(TempCookie, currentAction);
   
        foreach (GameObject cookie in _cookies)
        {
            Move(cookie, currentAction);
        }

        bool All_StopMoving = false;

        foreach (GameObject cookie in _cookies)
        {
            All_StopMoving = cookie.GetComponent<Cookie>().isMoving ? false : true;
        }

        if (All_StopMoving)
        {
            previousAction = currentAction;
            currentAction  = ACTION.IDLE;
            _mouseSlide.mouseAction = MouseSlide.SLIDE_ACTION.IDLE;
        }

        if (currentAction == ACTION.IDLE)
        {
            switch (previousAction)
            {
                case ACTION.SLIDEROW_R:
                    Destroy(_grid.GridBlocks[_grid.COLUMN - 2, current_row]);
                    break;
                case ACTION.SLIDEROW_L:
                    Destroy(_grid.GridBlocks[1, current_row]);
                    break;
                case ACTION.SLIDECOLUMN_U:
                    Destroy(_grid.GridBlocks[current_column, _grid.ROW - 2]);
                    break;
                case ACTION.SLIDECOLUMN_D:
                    Destroy(_grid.GridBlocks[current_column, 1]);
                    break;
            }

            Update_GRID();
        }
    }

    public void Move(GameObject cookie, ACTION _action)
    {
        cookie.GetComponent<Cookie>().isMoving = true;

        Vector3 target = Vector3.zero;

        switch (_action)
        {
            case ACTION.SLIDEROW_R:
                target = _grid.GridPositions[cookie.GetComponent<Cookie>().Column + 1, current_row];
                break;
            case ACTION.SLIDEROW_L:
                target = _grid.GridPositions[cookie.GetComponent<Cookie>().Column - 1, current_row];
                break;
            case ACTION.SLIDECOLUMN_U:
                target = _grid.GridPositions[current_column, cookie.GetComponent<Cookie>().Row + 1];
                break;
            case ACTION.SLIDECOLUMN_D:
                target = _grid.GridPositions[current_column, cookie.GetComponent<Cookie>().Row - 1];
                break;
        }

        if (Vector3.Distance(cookie.transform.position, target) > 0.02)
        {
            cookie.transform.position = Vector3.Lerp(cookie.transform.position, target, slideSpeed * Time.deltaTime);
        }
        else
        {
            cookie.transform.position = target;
            cookie.GetComponent<Cookie>().isMoving = false;
        }
    }

    private void Update_GRID()
    {

        switch (previousAction)
        {
            case ACTION.SLIDEROW_R:

                for (int i = _grid.COLUMN - 2; i > 1; i--)
                {
                    _grid.GridBlocks[i, current_row] = _grid.GridBlocks[i - 1, current_row];
                    _grid.GridBlocks[i, current_row].GetComponent<Cookie>().Column = i;
                    _grid.GridBlocks[i, current_row].name = "(" + i + ", " + current_row + ")";
                }

                if (TempCookie != null)
                {
                    _grid.GridBlocks[1, current_row] = TempCookie;
                    _grid.GridBlocks[1, current_row].GetComponent<Cookie>().Column = 1;
                    _grid.GridBlocks[1, current_row].name = "(" + 1 + ", " + current_row + ")";

                    TempCookie = null;
                }

                break;
            case ACTION.SLIDEROW_L:

                for (int i = 1; i < _grid.COLUMN - 2; i++)
                {
                    _grid.GridBlocks[i, current_row] = _grid.GridBlocks[i + 1, current_row];
                    _grid.GridBlocks[i, current_row].GetComponent<Cookie>().Column = i;
                    _grid.GridBlocks[i, current_row].name = "(" + i + ", " + current_row + ")";
                }

                if (TempCookie != null)
                {
                    _grid.GridBlocks[_grid.COLUMN - 2, current_row] = TempCookie;
                    _grid.GridBlocks[_grid.COLUMN - 2, current_row].GetComponent<Cookie>().Column = _grid.COLUMN - 2;
                    _grid.GridBlocks[_grid.COLUMN - 2, current_row].name = "(" + (_grid.COLUMN - 2) + ", " + current_row + ")";

                    TempCookie = null;
                }

                break;
            case ACTION.SLIDECOLUMN_U:

                for (int j = _grid.ROW - 2; j > 1; j--)
                {
                    _grid.GridBlocks[current_column, j] = _grid.GridBlocks[current_column, j - 1];
                    _grid.GridBlocks[current_column, j].GetComponent<Cookie>().Row = j;
                    _grid.GridBlocks[current_column, j].name = "(" + current_column + ", " + j + ")";
                }

                if (TempCookie != null)
                {
                    _grid.GridBlocks[current_column, 1] = TempCookie;
                    _grid.GridBlocks[current_column, 1].GetComponent<Cookie>().Row = 1;
                    _grid.GridBlocks[current_column, 1].name = "(" + current_column + ", " + 1 + ")";

                    TempCookie = null;
                }

                break;
            case ACTION.SLIDECOLUMN_D:

                for (int j = 1; j < _grid.ROW - 2; j++)
                {
                    _grid.GridBlocks[current_column, j] = _grid.GridBlocks[current_column, j + 1];
                    _grid.GridBlocks[current_column, j].GetComponent<Cookie>().Row = j;
                    _grid.GridBlocks[current_column, j].name = "(" + current_column + ", " + j + ")";
                }

                if (TempCookie != null)
                {
                    _grid.GridBlocks[current_column, _grid.ROW - 2] = TempCookie;
                    _grid.GridBlocks[current_column, _grid.ROW - 2].GetComponent<Cookie>().Row = _grid.ROW - 2;
                    _grid.GridBlocks[current_column, _grid.ROW - 2].name = "(" + current_column + ", " + (_grid.ROW - 2) + ")";

                    TempCookie = null;
                }

                break;
        }

        //currentSelectedBlock = _grid.GridBlocks[current_column, current_row];
        //Highlight_Block(currentSelectedBlock);
    }

    void MoveSelectedBlock(MOVE_DIR moveDir)
    {
        // SetColor_ROW_n_COLUMN(false);
       // UnHightlight_Block(currentSelectedBlock);

        switch (moveDir)
        {
            case MOVE_DIR.left:
                current_column--;
                break;
            case MOVE_DIR.right:
                current_column++;
                break;
            case MOVE_DIR.up:
                current_row++;
                break;
            case MOVE_DIR.down:
                current_row--;
                break;
        }

        current_column  = Mathf.Clamp(current_column, 1, _grid.COLUMN - 2);
        current_row     = Mathf.Clamp(current_row, 1, _grid.ROW - 2);

        currentSelectedBlock = _grid.GridBlocks[current_column, current_row];

        // SetColor_ROW_n_COLUMN(true);

       // Highlight_Block(currentSelectedBlock);

    }

    void Highlight_Block(GameObject _block)
    {
        Color tempColor = _block.GetComponent<MeshRenderer>().material.color;
        tempColor.a = 0.5f;
        _block.GetComponent<MeshRenderer>().material.color = tempColor;
    }

    void UnHightlight_Block(GameObject _block)
    {
        Color tempColor = _block.GetComponent<MeshRenderer>().material.color;
        tempColor.a = 1.0f;
        _block.GetComponent<MeshRenderer>().material.color = tempColor;
    }

    void SetColor_ROW_n_COLUMN(bool isSelected)
    {
        List<GameObject> Selected_ROWCOL = _grid.Get_ROWCOLUMN(current_row, current_column);

        if (isSelected)
        {
            foreach(GameObject block in Selected_ROWCOL)
            {
                block.GetComponent<MeshRenderer>().material.color = selectedColor;
            }
        }
        else
        {
            foreach (GameObject block in Selected_ROWCOL)
            {
                block.GetComponent<MeshRenderer>().material.color = unselectedColor;
            }
        }
    }
}
