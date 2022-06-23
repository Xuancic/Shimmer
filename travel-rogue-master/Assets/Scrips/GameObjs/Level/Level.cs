using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    [SerializeField] private int dis; //终点和起点的距离
    [SerializeField] private int back_dis; //从终点返回的距离
    [SerializeField] private int mblock_num; //怪物房数量
    [SerializeField] private int tblock_num; //宝藏房数量
    [SerializeField] private int tblock_dis; //收藏品地形块和主干道路的最大距离

    [SerializeField] private List<Vector2> wayFromBeginToBoss; //主路线
    [SerializeField] private Vector2 bossBlockCoord; //boss房坐标
    [SerializeField] private List<Vector2> treasureBlockCoord; //宝藏房坐标
    [SerializeField] private List<Vector2> monsterBlockCoord; //怪物房坐标
    [SerializeField] private List<Vector2> aroundBlockCoordList;//起始周边房坐标
    [SerializeField]private List<Vector2> allExistBlockList;//用于判断是否有重合

    [Header("Prefabs")]
    public GameObject ori;
    public GameObject boss;
    public GameObject tre;
    public GameObject monster;
    // public GameObject test;
    [Header("偏移")]
    public int offsetX;
    public int offsetY;

    public GameObject bossRoom;
    // public List<GameObject> tRooms;
    // public List<GameObject> mRooms;
    public Dictionary<Vector2, GameObject> roomArrays = new Dictionary<Vector2, GameObject>();

    private Player player;
    private MapPiecesController mapPiecesController;
    private Room currentRoom;

    private void Awake()
    {
        
    }

    private void Start()
    {
        player = GameManager.Instance.player;
        mapPiecesController = GameManager.Instance.MapPiecesController;
        InitializeRooms();
        currentRoom = roomArrays[Vector2.zero].GetComponent<Room>();
        FindFirstMapPiece();

    }


    private void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.LoadScene("GameScene_0");
        }

        for (int i = 0; i < wayFromBeginToBoss.Count-1; i++)
        {
            Debug.DrawLine(new Vector3(wayFromBeginToBoss[i].x*offsetX,wayFromBeginToBoss[i].y*offsetY),new Vector3(wayFromBeginToBoss[i+1].x*offsetX,wayFromBeginToBoss[i+1].y*offsetY),Color.red);
        }
    }


    #region 房间初始化

    private void InitializeRooms()
    {
        CreateRouteToBossBlock();
        CreateBossBlock();
        allExistBlockList.Add(new Vector2(0,0));//加入起点
        allExistBlockList.Add(bossBlockCoord);//加入boss房
        CreateAroundStart();//生成周边
        InitializeStartRoom();
        CreateTBlocks();
        CreateTBlockRoom();
        CreateMBlock();
        CreateMBlockRoom();
        roomArrays.Add(bossBlockCoord,bossRoom);
    }
    
    //生成初始房间类型
    private void InitializeStartRoom()
    {
        GameObject startRoom = Instantiate(ori, Vector3.zero+Vector3.up, Quaternion.identity);//生成初始房间
        roomArrays.Add(Vector2.zero, startRoom);
        List<Vector2> possDir = new List<Vector2>();
        List<int> possIndex = new List<int>();
        foreach (var vec in aroundBlockCoordList)
        {
            possDir.Add(new Vector2(vec.x,0).normalized);
            possDir.Add(new Vector2(0,vec.y).normalized);
        }
        //可能的索引
        for (int i = 0; i < 4; i++)
        {
            foreach (var vec in possDir)
            {
                if (RoomType.GetRoomTypeDoorDir(RoomType.RoomDoorType.C,i+1).Contains(vec))
                {
                    possIndex.Add(i+1);
                }
            }
        }

        startRoom.GetComponent<Room>().roomDoorType = RoomType.RoomDoorType.C;
        startRoom.GetComponent<Room>().roomTypeIndex = possIndex[Random.Range(0, possIndex.Count)];
    }

    
    
    //得到主干和boss房间坐标
    private void CreateRouteToBossBlock()
    {
        wayFromBeginToBoss.Add(new Vector2(0, 0));
        int randTime = Random.Range(0, 2);
        for (int i = 0; i < dis + randTime; i++)
        {
            Vector2 curCoordinate = wayFromBeginToBoss[i];
            Vector2 lastStep = new Vector2(0, 0);
            if (curCoordinate != new Vector2(0, 0))
            {
                lastStep = wayFromBeginToBoss[i - 1];
            }

            int dirRand = Random.Range(1, 101); //随机数
            if (dirRand >= 1 && dirRand <= 20) //左移一格
            {
                if (lastStep == curCoordinate+Vector2.left)
                {
                    wayFromBeginToBoss.Add(curCoordinate + Vector2.right);
                }
                else
                {
                    wayFromBeginToBoss.Add(curCoordinate +Vector2.left);
                }
                
            }
            else if (dirRand > 20 && dirRand <= 40) //右移一格
            {
                if (lastStep == curCoordinate + Vector2.right)
                {
                    wayFromBeginToBoss.Add(curCoordinate + Vector2.left);
                }
                else
                {
                    wayFromBeginToBoss.Add(curCoordinate + Vector2.right);
                }
                
            }
            else //上移一格
            {
                wayFromBeginToBoss.Add(curCoordinate + new Vector2(0, 1));
            }
        }

        bossBlockCoord = wayFromBeginToBoss[wayFromBeginToBoss.Count - 1];
        if (Math.Abs(bossBlockCoord.x) + bossBlockCoord.y <= dis - 2)
        {
            wayFromBeginToBoss.Clear();
            CreateRouteToBossBlock();
        }
    }
    //生成boss房
    private void CreateBossBlock()
    {
        bossRoom=Instantiate(boss, new Vector3(offsetX * bossBlockCoord.x, offsetY * bossBlockCoord.y + 1, 0),
            quaternion.identity);
        // roomArrays.Add(bossBlockCoord,bossRoom);
        int randTypeNum = Random.Range(1, 101);
        // Debug.Log("randnum="+randTypeNum);
        if (randTypeNum >= 1 && randTypeNum <= 60)
        {
            bossRoom.GetComponent<Room>().roomDoorType=RoomType.RoomDoorType.B;
            bossRoom.GetComponent<Room>().roomTypeIndex=1;
        }
        else if (randTypeNum>60 && randTypeNum<=90)
        {
            bossRoom.GetComponent<Room>().roomDoorType=RoomType.RoomDoorType.C;
            bossRoom.GetComponent<Room>().roomTypeIndex=1;
        }
        else
        {
            bossRoom.GetComponent<Room>().roomDoorType=RoomType.RoomDoorType.D;
            bossRoom.GetComponent<Room>().roomTypeIndex=Random.Range(1,4);
        }

    }
    
    
    
    //生成宝藏房坐标
    private void CreateTBlocks()
    {
        wayFromBeginToBoss.Remove(new Vector2(0,0));
        wayFromBeginToBoss.Remove(bossBlockCoord);
        int piece = (wayFromBeginToBoss.Count) / tblock_num;
        for (int i = 0; i < tblock_num; i++)
        {
            int randNum = Random.Range(1 + i * piece, i * piece + piece + 1);
            Vector2 originMainRoadCoord = wayFromBeginToBoss[randNum - 1];
            // Instantiate(tre,new Vector3(offsetX * originMainRoadCoord.x, offsetY * originMainRoadCoord.y + 1, 0),
            //     quaternion.identity);//测试
            Vector2 thisCoord = CreateTBlockCoord(originMainRoadCoord);
            treasureBlockCoord.Add(thisCoord);
            allExistBlockList.Add(thisCoord);
        }
    }
    //宝藏房的生成路线
    private Vector2 CreateTBlockCoord(Vector2 oriCoord)
    {
        List<Vector2> road = new List<Vector2>();
        road.Add(oriCoord);
        for (int j = 0; j < tblock_dis; j++)
        {
            Vector2 curCoord = road[j];
            int dirRand = Random.Range(1, 101);
            if (dirRand >= 1 && dirRand <= 40)
            {
                if (j > 0 && road[j-1]==curCoord+Vector2.left) 
                    road.Add(curCoord+Vector2.right);
                else
                    road.Add(curCoord+Vector2.left);
            }
            else if (dirRand > 40 && dirRand <= 80)
            {
                if (j > 0 && road[j-1]==curCoord+Vector2.right) 
                    road.Add(curCoord+Vector2.left);
                else
                    road.Add(curCoord+Vector2.right);
            }
            else if (dirRand > 80 && dirRand <= 90)
            {
                if (j > 0 && road[j-1]==curCoord+Vector2.up) 
                    road.Add(curCoord+Vector2.down);
                else
                    road.Add(curCoord+Vector2.up);
            }
            else
            {
                if (j > 0 && road[j-1]==curCoord+Vector2.down) 
                    road.Add(curCoord+Vector2.up);
                else
                    road.Add(curCoord+Vector2.down);
            }
        }
        if (allExistBlockList.Contains(road[road.Count-1]) || wayFromBeginToBoss.Contains(road[road.Count-1]))
        {
            // Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaa");
            return CreateTBlockCoord(oriCoord);
        }
        else
        {
            return road[road.Count - 1];
        }
    }
    //生成宝藏房的房间类型
    private void CreateTBlockRoom()
    {
        foreach (Vector2 vec in treasureBlockCoord)
        {
            GameObject newTBlock = Instantiate(tre, new Vector3(offsetX * vec.x, offsetY * vec.y + 1, 0),
                Quaternion.identity);
            roomArrays.Add(vec,newTBlock);
            // float minX = 100;
            // float minY = 100;
            //得到距离主路最近的方向坐标
            float minDis = 100;
            Vector2 minDir_x = new Vector2();
            Vector2 minDir_y = new Vector2();
            Vector2 minDisCoord = new Vector2();
            // Debug.Log(minDir.x+","+minDir.y);
            foreach (var oriVec in wayFromBeginToBoss)
            {
                // float curX = oriVec.x - vec.x;
                // float curY = oriVec.y - vec.y;
                float curDis = (oriVec - vec).magnitude;
                if (curDis<minDis)
                {
                    minDis = curDis;
                    minDisCoord = oriVec;
                }
            }
            minDir_x = new Vector2(minDisCoord.x - vec.x, 0).normalized;
            minDir_y = new Vector2(0, minDisCoord.y - vec.y).normalized;
            // Debug.Log(minDir_x+"+++++"+minDir_y);
            
            
            
            
            //得到房间类型
            RoomType.RoomDoorType roomDoorType;
            int randTypeNum = Random.Range(1, 101);
            if (randTypeNum >= 1 && randTypeNum <= 35)
            {
                roomDoorType = RoomType.RoomDoorType.B;
            }
            else if (randTypeNum > 35 && randTypeNum <= 80)
            {
                roomDoorType = RoomType.RoomDoorType.C;
            }
            else
            {
                roomDoorType = RoomType.RoomDoorType.D;
            }

            //得到可能的索引
            List<int> possIndexs = new List<int>();
            for (int i = 0; i < RoomType.GetRoomTypeMaxIndexNum(roomDoorType); i++)
            {
                if (RoomType.GetRoomTypeDoorDir(roomDoorType, i + 1).Contains(minDir_x)
                    ||RoomType.GetRoomTypeDoorDir(roomDoorType, i + 1).Contains(minDir_y) )
                {
                    possIndexs.Add(i + 1);
                }
            }

            //得到索引
            int tbRoomindex;
            if (possIndexs.Count > 1)
            {
                tbRoomindex = possIndexs[Random.Range(0, possIndexs.Count)];
            }
            else if (possIndexs.Count == 0)
            {
                tbRoomindex = Random.Range(1, RoomType.GetRoomTypeMaxIndexNum(roomDoorType)+1);
            }
            else
            {
                tbRoomindex = possIndexs[0];
            }

            newTBlock.GetComponent<Room>().roomDoorType = roomDoorType;
            newTBlock.GetComponent<Room>().roomTypeIndex = tbRoomindex;
        }
    }
    
    
    
    //生成初始房间周围的格子
    private void CreateAroundStart()
    {
        aroundBlockCoordList = new List<Vector2>();
        //生成坐标
        for (int i = 0; i < Random.Range(1,3); i++)
        {
            int j = Random.Range(1, 3);
            if (j==1)
            {
                int dirRand = Random.Range(1, 101);
                //两个的话一左一右
                if (i==1 && randVec(dirRand).x*aroundBlockCoordList[0].x>0)
                {
                    if (!allExistBlockList.Contains(-randVec(dirRand)))
                    {
                        aroundBlockCoordList.Add(-randVec(dirRand));  
                    }
                }
                else 
                  aroundBlockCoordList.Add(randVec(dirRand));

                if (i==1 && aroundBlockCoordList[1]==aroundBlockCoordList[0])
                {
                    aroundBlockCoordList.Remove(aroundBlockCoordList[1]);
                }
            }
            else
            {
                int dirRand_1 = Random.Range(1, 101);
                int dirRand_2 = Random.Range(1, 101);
                Vector2 vecRand_1=randVec(dirRand_1);
                Vector2 vecRand_2 = randVec(dirRand_2);
                if (vecRand_2==-vecRand_1)
                {
                    vecRand_2 = -vecRand_2;
                }
                //一左一右
                if (i==1 && (vecRand_1+vecRand_2).x*aroundBlockCoordList[0].x>0)
                {
                    if (!allExistBlockList.Contains(new Vector2(-(vecRand_1.x+vecRand_2.x),vecRand_1.y+vecRand_2.y)))
                    {
                        aroundBlockCoordList.Add(new Vector2(-(vecRand_1.x+vecRand_2.x),vecRand_1.y+vecRand_2.y));  
                    }
                }
                else 
                    aroundBlockCoordList.Add(vecRand_1+vecRand_2);

                if (i==1 && aroundBlockCoordList[1]==aroundBlockCoordList[0])
                {
                    aroundBlockCoordList.Remove(aroundBlockCoordList[1]);
                }
            }

            foreach (var vec in aroundBlockCoordList)
            {
                allExistBlockList.Add(vec);
            }
        }
        
        
        //生成房间并决定房间类型
        foreach (var vec in aroundBlockCoordList)
        {
            GameObject newAroundBlock = Instantiate(ori, new Vector3(offsetX * vec.x, offsetY * vec.y + 1, 0),
                Quaternion.identity);
            roomArrays.Add(vec,newAroundBlock);
            // Debug.Log("?????????");
            RoomType.RoomDoorType roomDoorType;//返回的类型
            List<int> possIndexs = new List<int>();//可能的索引
            List<Vector2> possDirList = new List<Vector2>();
            possDirList.Add(new Vector2(-vec.x,0).normalized);
            possDirList.Add(new Vector2(0,-vec.y).normalized);
            // possDirList.Remove(Vector2.zero);

            int randType = Random.Range(1, 101);
            if (randType>=1 && randType<=70)
            {
                roomDoorType = RoomType.RoomDoorType.B;
                for (int i = 0; i < RoomType.GetRoomTypeMaxIndexNum(roomDoorType); i++)
                {
                    bool canExist = false;
                    foreach (var possDir in possDirList)
                    {
                        if (RoomType.GetRoomTypeDoorDir(roomDoorType,i+1).Contains(possDir))
                        {
                            canExist = true;
                        }
                    }
                    if (canExist)
                    {
                        possIndexs.Add(i+1); 
                        // Debug.Log("BBBBBBBBB=="+possIndexs[possIndexs.Count-1]);
                    }
                                         
                }
            }
            else
            {
                roomDoorType = RoomType.RoomDoorType.C;
                for (int i = 0; i < RoomType.GetRoomTypeMaxIndexNum(roomDoorType); i++)
                {
                    bool canExist = false;
                    foreach (var possDir in possDirList)
                    {
                        if (RoomType.GetRoomTypeDoorDir(roomDoorType,i+1).Contains(possDir))
                        {
                            canExist = true;
                        }
                    }

                    if (canExist)
                    {
                        possIndexs.Add(i+1); 
                        // Debug.Log("CCCCCCCCCCC=="+possIndexs[0]);
                    }
                        
                   
                }
            }
            newAroundBlock.GetComponent<Room>().roomDoorType = roomDoorType;
            // Debug.Log("aaaaaaaaaaaaaaaaa");
            int doorTypeIndex;
            if (possIndexs.Count > 1)
            {
                doorTypeIndex = possIndexs[Random.Range(0, possIndexs.Count)];
            }
            else if (possIndexs.Count == 0)
            {
                doorTypeIndex = Random.Range(1, RoomType.GetRoomTypeMaxIndexNum(roomDoorType)+1);
            }
            else
            {
                doorTypeIndex = possIndexs[0];
            }
            newAroundBlock.GetComponent<Room>().roomTypeIndex = doorTypeIndex;
        }
    }
    private Vector2 randVec(int randNum)
    {
        if (randNum>=1&& randNum<=30)
        {
            return Vector2.left;
        }
        else if (randNum>30 && randNum<=60)
        {
            return Vector2.right;
        }
        else
        {
            return Vector2.up;
        }
    }
    

    //怪物房
    private void CreateMBlock()
    {
        RoomType.RoomDoorType bossDoorType = bossRoom.GetComponent<Room>().roomDoorType;
        int bossDoorIndex = bossRoom.GetComponent<Room>().roomTypeIndex;
        int addNum;
        int randNum = Random.Range(1, 101);
        if (randNum<=55)
            addNum = 0;
        else if (randNum <= 95)
            addNum = 1;
        else
            addNum = 2;
        
        for (int i = 0; i < mblock_num+addNum; i++)
        {
            Vector2 oriCoord = bossBlockCoord +
                                       RoomType.GetRoomTypeDoorDir(bossDoorType, bossDoorIndex)[
                                           Random.Range(0, RoomType.GetRoomTypeDoorDir(bossDoorType, bossDoorIndex).Count)];
            Vector2 thisCoord = CreateMBlockCoord(oriCoord);
            // Instantiate(monster, new Vector3(offsetX * thisCoord.x, offsetY * thisCoord.y + 1, 0), Quaternion.identity);
            monsterBlockCoord.Add(thisCoord);
            allExistBlockList.Add(thisCoord);
        }
        
    }
    //生成最终坐标
    private Vector2 CreateMBlockCoord(Vector2 oriCoord)
    {
        List<Vector2> road = new List<Vector2>();
        road.Add(oriCoord);
        for (int j = 0; j < back_dis+Random.Range(-3,-1); j++)
        {
            Vector2 curCoord = road[j];
            int dirRand = Random.Range(1, 101);
            if (dirRand >= 1 && dirRand <= 30)
            {
                if (j > 0 && road[j - 1] == curCoord + Vector2.left)
                {
                    road.Add(curCoord+Vector2.right);
                }
                else
                    road.Add(curCoord+Vector2.left);
            }
            else if (dirRand > 30 && dirRand <= 60)
            {
                if (j > 0 && road[j - 1] == curCoord + Vector2.right)
                {
                    road.Add(curCoord+Vector2.left);
                }
                else
                    road.Add(curCoord+Vector2.right);
            }
            else
            {
                road.Add(curCoord+Vector2.down);
            }
        }
        if (allExistBlockList.Contains(road[road.Count-1]))
        {
            return CreateMBlockCoord(oriCoord);
        }
        else
        {
            return road[road.Count - 1];
        }
    }
    private void CreateMBlockRoom()
    {
        foreach (var vec in monsterBlockCoord)
        {
            GameObject newMBlock=Instantiate(monster,new Vector3(offsetX * vec.x, offsetY * vec.y + 1, 0),
                quaternion.identity);
            roomArrays.Add(vec,newMBlock);
            int randTypeNum = Random.Range(1, 101);
            RoomType.RoomDoorType roomDoorType;
            int doorTypeIndex;
            
            //得到房间类型和索引
            if (randTypeNum<=10)
            {
                roomDoorType = RoomType.RoomDoorType.A;
                doorTypeIndex = 1;
            }//A类
            else if (randTypeNum<=35)
            {
                roomDoorType = RoomType.RoomDoorType.B;
                
                //得到最近距离的主路坐标
                float minDis = 100;
                Vector2 minDir_x = new Vector2();
                Vector2 minDir_y = new Vector2();
                Vector2 minDisCoord = new Vector2();
                foreach (var oriVec in wayFromBeginToBoss)
                {
                    float curDis = (oriVec - vec).magnitude;
                    if (curDis<minDis)
                    {
                        minDis = curDis;
                        minDisCoord = oriVec;
                    }
                }
                minDir_x = new Vector2(minDisCoord.x - vec.x, 0).normalized;
                minDir_y = new Vector2(0, minDisCoord.y - vec.y).normalized;
            
            
                List<int> possIndexs = new List<int>();
                for (int i = 0; i < RoomType.GetRoomTypeMaxIndexNum(roomDoorType); i++)
                {
                    if (RoomType.GetRoomTypeDoorDir(roomDoorType, i + 1).Contains(minDir_x)
                        ||RoomType.GetRoomTypeDoorDir(roomDoorType, i + 1).Contains(minDir_y) )
                    {
                        possIndexs.Add(i + 1);
                    }
                }
            
                if (possIndexs.Count > 1)
                {
                    doorTypeIndex = possIndexs[Random.Range(0, possIndexs.Count)];
                }
                else if (possIndexs.Count == 0)
                {
                    doorTypeIndex = Random.Range(1, RoomType.GetRoomTypeMaxIndexNum(roomDoorType)+1);
                }
                else
                {
                    doorTypeIndex = possIndexs[0];
                }
            }//B类
            else if (randTypeNum<=80)
            {
                roomDoorType = RoomType.RoomDoorType.C;
                //得到最近距离的主路坐标
                float minDis = 100;
                Vector2 minDir_x = new Vector2();
                Vector2 minDir_y = new Vector2();
                Vector2 minDisCoord = new Vector2();
                foreach (var oriVec in wayFromBeginToBoss)
                {
                    float curDis = (oriVec - vec).magnitude;
                    if (curDis<minDis)
                    {
                        minDis = curDis;
                        minDisCoord = oriVec;
                    }
                }
                minDir_x = new Vector2(minDisCoord.x - vec.x, 0).normalized;
                minDir_y = new Vector2(0, minDisCoord.y - vec.y).normalized;
            
            
                List<int> possIndexs = new List<int>();
                for (int i = 0; i < RoomType.GetRoomTypeMaxIndexNum(roomDoorType); i++)
                {
                    if (RoomType.GetRoomTypeDoorDir(roomDoorType, i + 1).Contains(minDir_x)
                        ||RoomType.GetRoomTypeDoorDir(roomDoorType, i + 1).Contains(minDir_y) )
                    {
                        possIndexs.Add(i + 1);
                    }
                }
            
                if (possIndexs.Count > 1)
                {
                    doorTypeIndex = possIndexs[Random.Range(0, possIndexs.Count)];
                }
                else if (possIndexs.Count == 0)
                {
                    doorTypeIndex = Random.Range(1, RoomType.GetRoomTypeMaxIndexNum(roomDoorType)+1);
                }
                else
                {
                    doorTypeIndex = possIndexs[0];
                }
            }//C类
            else
            {
                roomDoorType = RoomType.RoomDoorType.D;
                //得到最近距离的主路坐标
                float minDis = 100;
                Vector2 minDir_x = new Vector2();
                Vector2 minDir_y = new Vector2();
                Vector2 minDisCoord = new Vector2();
                foreach (var oriVec in wayFromBeginToBoss)
                {
                    float curDis = (oriVec - vec).magnitude;
                    if (curDis<minDis)
                    {
                        minDis = curDis;
                        minDisCoord = oriVec;
                    }
                }
                minDir_x = new Vector2(minDisCoord.x - vec.x, 0).normalized;
                minDir_y = new Vector2(0, minDisCoord.y - vec.y).normalized;
            
            
                List<int> possIndexs = new List<int>();
                for (int i = 0; i < RoomType.GetRoomTypeMaxIndexNum(roomDoorType); i++)
                {
                    if (RoomType.GetRoomTypeDoorDir(roomDoorType, i + 1).Contains(minDir_x)
                        ||RoomType.GetRoomTypeDoorDir(roomDoorType, i + 1).Contains(minDir_y) )
                    {
                        possIndexs.Add(i + 1);
                    }
                }
            
                if (possIndexs.Count > 1)
                {
                    doorTypeIndex = possIndexs[Random.Range(0, possIndexs.Count)];
                }
                else if (possIndexs.Count == 0)
                {
                    doorTypeIndex = Random.Range(1, RoomType.GetRoomTypeMaxIndexNum(roomDoorType)+1);
                }
                else
                {
                    doorTypeIndex = possIndexs[0];
                }
            }//D类
            
            
            
            newMBlock.GetComponent<Room>().roomDoorType = roomDoorType;
            newMBlock.GetComponent<Room>().roomTypeIndex = doorTypeIndex;
            
        }
    }
    #endregion

    #region 角色移动
    //移动到下一个房间
    public void MoveToNextRoom(Vector2 moveDir)
    {
        if (allExistBlockList.Contains(player.coordInMap+moveDir) 
            && roomArrays[player.coordInMap + moveDir].GetComponent<Room>().doorDir.Contains(-moveDir) )
        {
            // Debug.Log("wuwuwu");
            StartCoroutine(MoveToTheRoom(moveDir));
        }
    }
    private IEnumerator MoveToTheRoom(Vector2 moveDir)
    {
        Camera cameraToMove = GameManager.Instance.playerCamera;
        float delaySeconds = 0.3f;
        currentRoom = roomArrays[player.coordInMap + moveDir].GetComponent<Room>();
        //如果没来过，生成房间里的内容
        if (!currentRoom.isArrived)
        {
            currentRoom.GenerateRoomContent();
            currentRoom.isArrived = true;
        }
        
        //角色移动
        player.Controller.Controlable = false;
        player.transform.position += new Vector3(moveDir.x * offsetX * 0.45f, moveDir.y * offsetY * 0.7f, 0);
        player.coordInMap += moveDir;
        
        
        //移动镜头
        Vector3 oriPos = cameraToMove.transform.position;
        Vector3 targetPos = currentRoom.transform.position;
        targetPos.z += cameraToMove.transform.position.z;
        float time = 0f;
        while (time<delaySeconds)
        {
            cameraToMove.transform.position =
                Vector3.Lerp(oriPos, targetPos, (1 / delaySeconds) * (time += Time.deltaTime));
            yield return null;
        }
        
        //恢复角色移动
        player.Controller.Controlable = true;
    }
    
    #endregion


    //初始的一块
    private void FindFirstMapPiece()
    {
        // Debug.Log(startRoomDoorDirList[0]);
        bool hasAround = false;
        List<Vector2> dirShouldHave = new List<Vector2>();
        foreach (var vec in currentRoom.doorDir)
        {
            if (!aroundBlockCoordList.Contains(vec))
            {
                foreach (var aroundBlockVec in aroundBlockCoordList)
                {
                    if (roomArrays[aroundBlockVec].GetComponent<Room>().doorDir.Contains(vec-aroundBlockVec))
                    {
                        dirShouldHave.Add(-vec);
                        dirShouldHave.Add(aroundBlockVec-vec);
                        RoomType.RoomDoorType roomDoorType = MapPieceType();
                        int index = FindMapPieceIndex(dirShouldHave, roomDoorType);
                        if (index!=0)
                        {
                            mapPiecesController.AddPieceInBag(roomDoorType,index);
                            mapPiecesController.iniPieceNum -= 1;
                        }
                    }
                }


            }
        }
        
        for (int i = 0; i < mapPiecesController.iniPieceNum; i++)
        {
            Debug.Log("absbasajsbajbak");
            RoomType.RoomDoorType type = MapPieceType();
            int index = Random.Range(1, RoomType.GetRoomTypeMaxIndexNum(type) + 1);
            mapPiecesController.AddPieceInBag(type,index);
        }
    }

  
    
    
    //找到有门的index
    private int FindMapPieceIndex(List<Vector2> doorDirs,RoomType.RoomDoorType roomDoorType)
    {

        for (int i = 0; i < RoomType.GetRoomTypeMaxIndexNum(roomDoorType); i++)
        {
            bool canAdd = true;
            foreach (var vec in doorDirs)
            {
                if (!RoomType.GetRoomTypeDoorDir(roomDoorType,i+1).Contains(vec))
                {
                    canAdd = false;
                }
            }
            if (canAdd)
            {
                return i + 1;
            }
        }
        return 0;
    }
    
    
    //地图块类型
    private RoomType.RoomDoorType MapPieceType()
    {
        int randNum = Random.Range(1, 101);
        var playerState = player.State;
        if (randNum<=playerState.m_luck)
        {
            return RoomType.RoomDoorType.A;
        }
        else if (randNum<=2*playerState.m_luck+30)
        {
            return RoomType.RoomDoorType.B;
        }
        else if (randNum<=2*playerState.m_luck+70)
        {
            return RoomType.RoomDoorType.C;
        }
        else
        {
            return RoomType.RoomDoorType.D;
        }
    }

    
    
    //位置转化为坐标
    public  Vector2 ChangeTransPosToVector(Vector2 pos)
    {
        
        Vector2 vecCoord = new Vector2((int) (pos.x/offsetX), (int) ((pos.y - 1)/offsetY));
        return vecCoord;
    }


    //判断能不能下放
    public bool CanBePut(Room room)
    {
        List<Vector2> fourDir = new List<Vector2>();
        fourDir.Add(Vector2.up);
        fourDir.Add(Vector2.down);
        fourDir.Add(Vector2.left);
        fourDir.Add(Vector2.right);
        
        // 找到没门的方向
        List<Vector2> noDoorDirList =new List<Vector2>();
        foreach (var vec in fourDir)
        {
            noDoorDirList.Add(vec);
        }
        foreach (var vec in room.doorDir)
        {
            noDoorDirList.Remove(vec);
        }
        
        bool canPut = true;
        Vector2 roomCoord = ChangeTransPosToVector(room.transform.position);
        if (allExistBlockList.Contains(roomCoord))
        {
            canPut = false;
        }
        
        //判定周边是否有房间
        bool haveBlockAround = false;
        foreach (var vec in fourDir)
        {
            if (allExistBlockList.Contains(roomCoord+vec))
            {
                haveBlockAround = true;
            }
        }

        if (!haveBlockAround)
        {
            // Debug.Log("sbasbcasbcab");
            canPut = false;
        }
        
        
        //有门的方向如果有房间则得有门
        foreach (var doorVec in room.doorDir)
        {
            Vector2 posHaveRoom = doorVec + ChangeTransPosToVector(room.transform.position);
            if (allExistBlockList.Contains(posHaveRoom))
            {
                if (!roomArrays[posHaveRoom].GetComponent<Room>().doorDir.Contains(-doorVec))
                {
                    canPut = false;
                }
            }
        }

        //没门的方向如果有房间有门则不行
        foreach (var noDoorVec in noDoorDirList)
        {
            if (allExistBlockList.Contains(roomCoord+noDoorVec)
                && roomArrays[roomCoord+noDoorVec].GetComponent<Room>().doorDir.Contains(-noDoorVec))
            {
                canPut = false;
            }
        }
        

        if (canPut)
        {
            allExistBlockList.Add(roomCoord);
            roomArrays.Add(roomCoord,room.gameObject);
            room.transform.position = new Vector3(roomCoord.x * offsetX, roomCoord.y * offsetY + 1, 0);
        }

        return canPut;
    }


    private void CreateClearedRoomMapPiece()
    {
        if (currentRoom.GetComponent<Room>().isCleared)
        {
            
        }
    }
    
}