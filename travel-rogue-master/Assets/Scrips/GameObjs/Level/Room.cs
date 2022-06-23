using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

public class Room : MonoBehaviour
{
    // public enum RoomType{Start,Normal,Boss,Treasure,Monster}
    // public enum RoomDoorType{A,B,C,D}
    public List<GameObject> doorList;
    public List<Vector2> doorDir;

    public  RoomType.RoomDoorType roomDoorType;
    public RoomType.RoomFuncType roomFuncType;
    public  int roomTypeIndex;

    public bool isArrived=false;
    public bool isCleared;


    // private void Awake()
    // {
    //     doorDir = RoomType.GetRoomTypeDoorDir(roomDoorType, roomTypeIndex);
    // }

    private void Start()
    {
        CreateDoors(roomDoorType,roomTypeIndex);
        doorDir = RoomType.GetRoomTypeDoorDir(roomDoorType, roomTypeIndex);
    }

    
    //生成门
    public void CreateDoors(RoomType.RoomDoorType type,int index)
    {
        switch (type)
        {
            case RoomType.RoomDoorType.A:
                break;
            case RoomType.RoomDoorType.B:
                switch (index)
                {
                    case 1:
                        doorList[0].SetActive(false);
                        break;
                    case 2:
                        doorList[3].SetActive(false);
                        break;
                    case 3:
                        doorList[2].SetActive(false);
                        break;
                }
                break;
            case RoomType.RoomDoorType.C:
                switch (index)
                {
                    case 1:
                        doorList[0].SetActive(false);
                        doorList[1].SetActive(false);
                        break;
                    case 2:
                        doorList[2].SetActive(false);
                        doorList[3].SetActive(false);
                        break;
                    case 3:
                        doorList[1].SetActive(false);
                        doorList[3].SetActive(false);
                        break; 
                    case 4:
                        doorList[1].SetActive(false);
                        doorList[2].SetActive(false);
                        break;
                }
                break;
            case RoomType.RoomDoorType.D:
                switch (index)
                {
                    case 1:
                        doorList[0].SetActive(false);
                        doorList[1].SetActive(false);
                        doorList[3].SetActive(false);
                        break;
                    case 2:
                        doorList[0].SetActive(false);
                        doorList[2].SetActive(false);
                        doorList[3].SetActive(false);
                        break;
                    case 3:
                        doorList[0].SetActive(false);
                        doorList[1].SetActive(false);
                        doorList[2].SetActive(false);
                        break;
                }
                break;
                
            // case RoomDoorType.B2:
            //     doorList[3].SetActive(false);
            //     return;
            // case RoomDoorType.B3:
            //     doorList[2].SetActive(false);
            //     return;
            // case RoomDoorType.C1:
            //     doorList[1].SetActive(false);
            //     doorList[2].SetActive(false);
            //     return;
            // case RoomDoorType.C2:
            //     doorList[3].SetActive(false);
            //     doorList[4].SetActive(false);
            //     return;
            // case RoomDoorType.C3:
            //     doorList[1].SetActive(false);
            //     doorList[3].SetActive(false);
            //     return;
            // case RoomDoorType.C4:
            //     doorList[0].SetActive(false);
            //     doorList[2].SetActive(false);
            //     return;
            // case RoomDoorType.D1:
            //     doorList[0].SetActive(false);
            //     doorList[1].SetActive(false);
            //     doorList[3].SetActive(false);
            //     return;
            // case RoomDoorType.D2:
            //     doorList[0].SetActive(false);
            //     doorList[2].SetActive(false);
            //     doorList[3].SetActive(false);
            //     return;
            // case RoomDoorType.D3:
            //     doorList[0].SetActive(false);
            //     doorList[1].SetActive(false);
            //     doorList[2].SetActive(false);
            //     return;
        }
    }


    //生成房间内的内容
    public void GenerateRoomContent()
    {
        
    }
}
