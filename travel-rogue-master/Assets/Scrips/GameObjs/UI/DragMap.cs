using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragMap : MonoBehaviour,IBeginDragHandler,IDragHandler
{
    public Camera mapCamera;
    public Vector2 Margin, Smoothing;
    public BoxCollider2D Bounds;

    private Vector3
        _min,
        _max;

    private Vector2 first = Vector2.zero; //鼠标第一次落下点
    private Vector2 second = Vector2.zero; //鼠标第二次位置（拖拽位置）
    private Vector3 vecPos = Vector3.zero;
    private bool IsNeedMove = false; //是否需要移动
    public float speed;

    private void Awake()
    {
    }

    public void Start()
    {
        _min = Bounds.bounds.min; //包围盒
        _max = Bounds.bounds.max;
        first.x = mapCamera.transform.position.x; //初始化
        first.y = mapCamera.transform.position.y;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        first = Input.mousePosition;

    }

    public void OnDrag(PointerEventData eventData)
    {
        second = Input.mousePosition;
        Vector3 fir = mapCamera.ScreenToWorldPoint(new Vector3(first.x, first.y, 0)); //转换至世界坐标
        Vector3 sec = mapCamera.ScreenToWorldPoint(new Vector3(second.x, second.y, 0));
        vecPos = speed*(sec - fir); //需要移动的 向量
        first = second;
        var x = mapCamera.transform.position.x;
        var y = mapCamera.transform.position.y;
        x = x - vecPos.x; //向量偏移
        y = y - vecPos.y;

        var cameraHalfWidth = mapCamera.orthographicSize * ((float) Screen.width / Screen.height);
        //保证不会移除包围盒
        x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
        y = Mathf.Clamp(y, _min.y + mapCamera.orthographicSize, _max.y - mapCamera.orthographicSize);

        mapCamera.transform.position = new Vector3(x,y, mapCamera.transform.position.z);
    }

    // public void OnGUI()
    // {
    //     if (Event.current.type == EventType.MouseDown)
    //     {
    //         // Debug.Log("aaaaaaaaaaa");
    //         // float xInWorld=
    //         // //记录鼠标按下的位置 　　
    //         // if (Mathf.Clamp(Event.current.mousePosition.x, _min.x, _max.x) == Event.current.mousePosition.x
    //         //     && Mathf.Clamp(Event.current.mousePosition.y, _min.y, _max.y) == Event.current.mousePosition.y)
    //         // {
    //         // Ray ray = Camera.main.ScreenPointToRay(Event.current.mousePosition);
    //         // RaycastHit hitInfo;
    //         // Debug.Log("wwwwwwwwwww");
    //         // if (Physics.Raycast(ray, out hitInfo,-200))
    //         // {
    //         //     Debug.Log(hitInfo.transform.name);
    //         //     if (hitInfo.transform.CompareTag("MiniMap"))
    //         //     {
    //         //         Debug.Log("aaaaaaaaaaaaaaaaaaaaaa");
    //         //         first = Event.current.mousePosition;
    //         //         IsNeedMove = true;
    //         //     }
    //         first = Event.current.mousePosition;
    //     }
    //
    //     if (Event.current.type == EventType.MouseDrag)
    //     {
    //         second = Event.current.mousePosition;
    //         Vector3 fir = mapCamera.ScreenToWorldPoint(new Vector3(first.x, first.y, 0)); //转换至世界坐标
    //         Vector3 sec = mapCamera.ScreenToWorldPoint(new Vector3(second.x, second.y, 0));
    //         vecPos = speed*(sec - fir); //需要移动的 向量
    //         first = second;
    //         IsNeedMove = true;
    //         //记录鼠标拖动的位置 　　
    //     }
    //     else
    //     {
    //         IsNeedMove = false;
    //     }
    // }

    // public void Update()
    // {
    //     if (IsNeedMove == false)
    //     {
    //         return;
    //     }
    //
    //     var x = mapCamera.transform.position.x;
    //     var y = mapCamera.transform.position.y;
    //     x = x - vecPos.x; //向量偏移
    //     y = y + vecPos.y;
    //
    //     var cameraHalfWidth = mapCamera.orthographicSize * ((float) Screen.width / Screen.height);
    //     //保证不会移除包围盒
    //     x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
    //     y = Mathf.Clamp(y, _min.y + mapCamera.orthographicSize, _max.y - mapCamera.orthographicSize);
    //
    //     mapCamera.transform.position = new Vector3(x,y, mapCamera.transform.position.z);
    // }
}