using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapPiece : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public RoomType.RoomDoorType roomDoorType;
    public int roomDoorIndex;
    public Image picOfRoomDoorType;
    public Room roomPrefab;
    public Camera mapCamera;

    private Level level;
    // public int offset;

    // [SerializeField] private RectTransform canvas;


    private Vector2 iniCoord;
    public Transform uiCenterCoord;
    private Vector2 first = Vector2.zero; //鼠标第一次落下点
    private Room newRoom;
    private bool isCreated = false;

    public float offsetX;

    public GameObject shape;

    private GameObject square;

    private Vector2 roomCoord;
    // public float offsetY;


    // private void Update()
    // {
    //     if (isCreated)
    //     {
    //         Vector2 coordInMapCamera = new Vector2((Input.mousePosition.x), (Input.mousePosition.y));
    //         newRoom.transform.position = new Vector3(mapCamera.ScreenToWorldPoint(coordInMapCamera).x,
    //             mapCamera.ScreenToWorldPoint(coordInMapCamera).y, 0);
    //
    //     }
    // }
    


    private void Start()
    {
        mapCamera = GameObject.Find("MapCamera").GetComponent<Camera>();
        // canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        uiCenterCoord = GameObject.Find("Canvas/MiniMapPanel/MiniMap").transform;
        level = GameManager.Instance.level;
        // Debug.Log("asjkasjxakjxbaskbx");
    }

    // 点击生成
    // public void onpointer(PointerEventData eventData)
    // {
    //     Debug.Log("hhhhhhhhhhhh");
    //     first = Camera.main.ScreenToWorldPoint(Input.mousePosition);//鼠标转为世界坐标
    //     // uiCenterCoord = GameObject.Find("Canvas/MiniMapPanel/MiniMap").transform;
    //     //
    //     //
    //     iniCoord = first - new Vector2(uiCenterCoord.transform.position.x, uiCenterCoord.transform.position.y);
    //     newRoom = Instantiate(roomPrefab,new Vector3(mapCamera.ScreenToWorldPoint(iniCoord).x,mapCamera.ScreenToWorldPoint(iniCoord).y,0), Quaternion.identity);
    //     newRoom.roomDoorType = roomDoorType;
    //     newRoom.roomTypeIndex = roomDoorIndex;
    //     isCreated = true;
    // }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     Debug.Log("hhhhhhhhhhhh");
    //     first = Camera.main.ScreenToWorldPoint(Input.mousePosition);//鼠标转为世界坐标
    //     // uiCenterCoord = GameObject.Find("Canvas/MiniMapPanel/MiniMap").transform;
    //     //
    //     //
    //     iniCoord = first - new Vector2(uiCenterCoord.transform.position.x, uiCenterCoord.transform.position.y);
    //     newRoom = Instantiate(roomPrefab,new Vector3(mapCamera.ScreenToWorldPoint(iniCoord).x,mapCamera.ScreenToWorldPoint(iniCoord).y,0), Quaternion.identity);
    //     newRoom.roomDoorType = roomDoorType;
    //     newRoom.roomTypeIndex = roomDoorIndex;
    //     isCreated = true;
    // }
    //
    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     Debug.Log("skasaksbakjbaksjbca");
    // }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("hhhhhhhhhhhh");
        first = Camera.main.ScreenToWorldPoint(Input.mousePosition); //鼠标转为世界坐标
        // uiCenterCoord = GameObject.Find("Canvas/MiniMapPanel/MiniMap").transform;
        //
        //
        iniCoord = first - new Vector2(uiCenterCoord.transform.position.x, uiCenterCoord.transform.position.y);
        newRoom = Instantiate(roomPrefab,
            new Vector3(mapCamera.ScreenToWorldPoint(iniCoord).x, mapCamera.ScreenToWorldPoint(iniCoord).y, 0),
            Quaternion.identity);
        newRoom.roomDoorType = roomDoorType;
        newRoom.roomTypeIndex = roomDoorIndex;
        // isCreated = true;

        roomCoord = level.ChangeTransPosToVector(newRoom.transform.position);

        square = Instantiate(shape,
            new Vector3(roomCoord.x * level.offsetX, roomCoord.y * level.offsetY + 1, 0),
            Quaternion.identity);
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector3 coordInMapCamera = mapCamera.WorldToScreenPoint(newRoom.transform.position) +
                                   new Vector3(eventData.delta.x / offsetX, eventData.delta.y / offsetX, 0);
        newRoom.transform.position = mapCamera.ScreenToWorldPoint(coordInMapCamera);
        roomCoord = level.ChangeTransPosToVector(newRoom.transform.position);

        square.transform.position = new Vector3(roomCoord.x * level.offsetX, roomCoord.y * level.offsetY + 1, 0);


     
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (level.CanBePut(newRoom))
        {
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(newRoom.gameObject);
            Destroy(square.gameObject);
        }
        
    }
}