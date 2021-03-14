using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseSlide : MonoBehaviour
{
    public static MouseSlide sharedInstance { get; set; }

    public UnityEvent OnSlide;

    public int COLUMN_onClicked;
    public int ROW_onClicked;

    private Vector2 _mouseDownPos;
    private Vector2 _mouseUpPos;

    private bool _dragEnabled = false;

    public enum SLIDE_ACTION
    {
        IDLE, LEFT, RIGHT, UP, DOWN
    }

    public SLIDE_ACTION mouseAction = SLIDE_ACTION.IDLE;

    private void Awake()
    {
        sharedInstance = this;
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                COLUMN_onClicked    = hit.collider.GetComponent<Cookie>().Column;
                ROW_onClicked       = hit.collider.GetComponent<Cookie>().Row;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !_dragEnabled)
        {
            _mouseDownPos = Input.mousePosition;
            _dragEnabled = true;
        }

        if (_dragEnabled)
        {
            Vector2 onDragPos = Input.mousePosition;
            float diff_x = Mathf.Abs(onDragPos.x - _mouseDownPos.x);
            float diff_y = Mathf.Abs(onDragPos.y - _mouseDownPos.y);

            if(diff_x > diff_y)
            {
                if (onDragPos.x > _mouseDownPos.x)
                    mouseAction = SLIDE_ACTION.RIGHT;
                if (onDragPos.x < _mouseDownPos.x)
                    mouseAction = SLIDE_ACTION.LEFT;
            }else if(diff_y > diff_x)
            {
                if (onDragPos.y > _mouseDownPos.y)
                    mouseAction = SLIDE_ACTION.UP;
                if (onDragPos.y < _mouseDownPos.y)
                    mouseAction = SLIDE_ACTION.DOWN;
            }

           OnSlide.Invoke();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _mouseUpPos  = Input.mousePosition;
            _dragEnabled = false;
        }
    }
}
