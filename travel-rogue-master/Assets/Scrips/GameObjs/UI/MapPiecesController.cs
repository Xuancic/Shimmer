using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPiecesController : MonoBehaviour
{
    public MapPiece pieceInBag;
    public int iniPieceNum;
    public int clearMonsterPieceNum;
    
    public GameObject bagGrid;
    public List<Sprite> RoomDoorPicList;
    public LayerMask mapPieceLayer;
    public Transform miniMapCoord;
    public List<MapPiece> MapPieces;

    private void Start()
    {
        // MapPieces.Add(AddPieceInBag(RoomType.RoomDoorType.B,2));//test
        // MapPieces.Add(AddPieceInBag(RoomType.RoomDoorType.C,2));//test
        // // MapPieces[1].anotherScrollRect = MapPieces[0].thisScrollRect;
    }

    public void AddPieceInBag(RoomType.RoomDoorType type,int index)
    {
        MapPiece newMapPiece = Instantiate(pieceInBag,bagGrid.transform.position,Quaternion.identity);
        newMapPiece.gameObject.transform.SetParent(bagGrid.transform);
        newMapPiece.picOfRoomDoorType.sprite = ChoosePic(type, index);
        newMapPiece.roomDoorIndex = index;
        newMapPiece.roomDoorType = type;
        newMapPiece.GetComponent<RectTransform>().localScale = new Vector3(0.35f, 0.35f, 0);
        MapPieces.Add(newMapPiece);
    }

    private Sprite ChoosePic(RoomType.RoomDoorType type,int index)
    {
        switch (type)
        {
            case RoomType.RoomDoorType.A:
                return RoomDoorPicList[0];
            case RoomType.RoomDoorType.B:
                switch (index)
                {
                    case 1:
                        return RoomDoorPicList[1];
                    case 2:
                        return RoomDoorPicList[2];
                    case 3:
                        return RoomDoorPicList[3];
                }
                break;
            case RoomType.RoomDoorType.C:
                switch (index)
                {
                    case 1:
                        return RoomDoorPicList[4];
                    case 2:
                        return RoomDoorPicList[5];
                    case 3:
                        return RoomDoorPicList[6];
                    case 4:
                        return RoomDoorPicList[7];
                }
                break;
            case RoomType.RoomDoorType.D:
                switch (index)
                {
                    case 1:
                        return RoomDoorPicList[8];
                    case 2:
                        return RoomDoorPicList[9];
                    case 3:
                        return RoomDoorPicList[10];
                }
                break;
        }
        return null;
    }


    // public void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         OnClicked();
    //     }
    //     
    // }

    public void OnClicked()
    {
        RaycastHit2D _hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero, mapPieceLayer);
        if (_hit)
        {
            Debug.Log("bbbbbbbbbb");
        }
        // Debug.Log("hhhhhhhhhhhh");
        // first = Camera.main.ScreenToWorldPoint(Input.mousePosition);//鼠标转为世界坐标
        // uiCenterCoord = GameObject.Find("Canvas/MiniMapPanel/MiniMap").transform;
        //
        //
        // iniCoord = new Vector2(mapCamera.gameObject.transform.position.x, mapCamera.gameObject.transform.position.y)
        //            + (first - new Vector2(uiCenterCoord.transform.position.x, uiCenterCoord.transform.position.y));
        // Room newRoom = Instantiate(roomPrefab, mapCamera.ScreenToWorldPoint(iniCoord), Quaternion.identity);
        // newRoom.roomDoorType = roomDoorType;
        // newRoom.roomTypeIndex = roomDoorIndex;
    }
}
