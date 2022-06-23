using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "房间类型")]
public class RoomType : MonoBehaviour
{
    public enum RoomFuncType{Start,Normal,Boss,Treasure,Monster}
    public enum RoomDoorType{A,B,C,D}

    public  List<Sprite> RoomDoorTypeImages;


    public static int GetRoomTypeMaxIndexNum(RoomDoorType type)
    {
        switch (type)
        {
            case RoomDoorType.A:
                return 1;
            case RoomDoorType.B:
                return 3;
            case RoomDoorType.C:
                return 4;
            case RoomDoorType.D:
                return 3;
        }
        return 0;
    }
    
    
    //获取门的延伸方向
    public static List<Vector2> GetRoomTypeDoorDir(RoomDoorType type,int index)
    {
        List<Vector2> doorDir = new List<Vector2>();
        switch (type)
        {
            case RoomDoorType.A:
                doorDir.Add(Vector2.up);
                doorDir.Add(Vector2.down);
                doorDir.Add(Vector2.left);
                doorDir.Add(Vector2.right);
                break;
            case RoomDoorType.B:
                switch (index)
                {
                    case 1:
                        doorDir.Add(Vector2.down);
                        doorDir.Add(Vector2.left);
                        doorDir.Add(Vector2.right);
                        break;
                    case 2:
                        doorDir.Add(Vector2.up);
                        doorDir.Add(Vector2.down);
                        doorDir.Add(Vector2.left);
                        break;
                    case 3:
                        doorDir.Add(Vector2.up);
                        doorDir.Add(Vector2.down);
                        doorDir.Add(Vector2.right);
                        break;
                }
                break;
            case RoomDoorType.C:
                switch (index)
                {
                    case 1:
                        doorDir.Add(Vector2.left);
                        doorDir.Add(Vector2.right);
                        break;
                    case 2:
                        doorDir.Add(Vector2.up);
                        doorDir.Add(Vector2.down);
                        break;
                    case 3:
                        doorDir.Add(Vector2.up);
                        doorDir.Add(Vector2.left);
                        break; 
                    case 4:
                        doorDir.Add(Vector2.up);
                        doorDir.Add(Vector2.right);
                        break;
                }
                break;
            case RoomDoorType.D:
                switch (index)
                {
                    case 1:
                        doorDir.Add(Vector2.left);
                        break;
                    case 2:
                        doorDir.Add(Vector2.down);
                        break;
                    case 3:
                        doorDir.Add(Vector2.right);
                        break;
                }
                break;
        }
        return doorDir;
    }


    //获取图片
    public  Sprite GetRoomDoorTypeImage(RoomDoorType type, int index)
    {
        switch (type)
        {
            case RoomDoorType.A:
                return RoomDoorTypeImages[0];
            case RoomDoorType.B:
                switch (index)
                {
                    case 1:
                        return RoomDoorTypeImages[1];
                    case 2:
                        return RoomDoorTypeImages[2];
                    case 3:
                        return RoomDoorTypeImages[3];
                }
                break;
            case RoomDoorType.C:
                switch (index)
                {
                    case 1:
                        return RoomDoorTypeImages[4];
                    case 2:
                        return RoomDoorTypeImages[5];
                    case 3:
                        return RoomDoorTypeImages[6];
                    case 4:
                        return RoomDoorTypeImages[7];
                }
                break;
            case RoomDoorType.D:
                switch (index)
                {
                    case 1:
                        return RoomDoorTypeImages[8];
                    case 2:
                        return RoomDoorTypeImages[9];
                    case 3:
                        return RoomDoorTypeImages[10];
                }
                break;
        }
        return null;
    }
}
