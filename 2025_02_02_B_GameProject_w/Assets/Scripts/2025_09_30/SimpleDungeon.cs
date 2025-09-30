using System;
using System.Collections.Generic;
using UnityEngine;



public class SimpleDungeon : MonoBehaviour
{
    [Header("던전 설정")]
    public int roomCount = 8;       // 전체 생성하고 싶은 방 개수  (시작/보스/보물 /일반 포함)
    public int minsize = 4;     //방 최소 크기
    public int maxsize = 8;     // 방 최대 크기
                                // 둘다 타일단위

    [Header("스포너 설정")]
    public bool spawnEnemies = true;        // 일반 방과 보스 방에 적을 생성 할지 여부
    public bool spawnTreasures = true;      // 보물 방에 보물을 생성 할지 여부
    public int enemisPerRoom = 2;               // 일반 방 1개당 생성할 적의 수

    private Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();    //  방 중심 좌표 -> 방정보 매칭 방 메타데이터 보관
    private HashSet<Vector2Int> floors = new HashSet<Vector2Int>();             // floors: 바닥 타일 좌표 집합 어떤칸이 바닥인지 조회
    private HashSet<Vector2Int> walls = new HashSet<Vector2Int>();              // walls 벽 타일 좌표 집합 바닥 주변을 자동으로 채운다
    
            

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Clear();
            Generate();
        }
    }

    public void Generate()
    {
        // 방 여러개를 규칙적으로 만든다
        CreateRooms();
        // 방과 방 사이를 복도로 연결한다
        // 바닥 주변 타일에 벽을 자동 배치한다
        // 실제 Unity 상에서 cube로 타일을 그린다
        // 방 타입에 따라 적/ 보물을 배치한다

        ConnectRooms();

        CreateWalls();
        Render();

        SpawnObjects();

    }
    // 시작 방 1개 생성 , 나머지는 기존ㅌ방 근처 상 하 좌 우 에 오프셋을 두고 시도
    // 마지막 생성 방은 보스방으로 지정 , 일단 방 일부를 보물방으로 변환
    void CreateRooms()
    {
        //시작방 : 기준점 (0,0)에 배치
        Vector2Int pos = Vector2Int.zero;
        int size = UnityEngine.Random.Range(minsize , maxsize);
        AddRoom(pos, size, RoomType.Start);

        for(int i = 0; i< roomCount; i++)
        {
            var roomList = new List<Room>(rooms.Values);        // 이미 만들어진 방 중 하나를 기준으로

            Room baseRoom = roomList[UnityEngine.Random.Range(0, roomList.Count)];
            Vector2Int[] dirs =
            {
                Vector2Int.up * 6 , Vector2Int.down * 6, Vector2Int.left * 6 ,Vector2Int.right * 6
            };

            foreach(var dir in dirs)
            {
                Vector2Int newPos = baseRoom.center + dir;          // 새방 중심 좌표
                int newSize = UnityEngine.Random.Range(minsize , maxsize);      // 새방 크기 설정
                RoomType type = (i == roomCount - 1) ? RoomType.Boss : RoomType.Normal;
                if(AddRoom(newPos ,newSize , type))break;       // 방 영역이 기존 바닥과 겹치지 않으면 추가성공-> 새로운 방 생성으로 진행
            }

            //
        }

        int treasureCount = Mathf.Max(1, roomCount / 4);
        var normalRooms = new List<Room>();

        foreach(var room in rooms.Values)
        {
            if(room.type == RoomType.Normal)
                normalRooms.Add(room);  

        }

        for (int i = 0; i < treasureCount && normalRooms.Count > 0; i++)    //무작위 일반방을 보물방으로 바꾼다
        {
            int idx = UnityEngine.Random.Range(0, normalRooms.Count);
            normalRooms[idx].type = RoomType.Treasure;
            normalRooms.RemoveAt(idx);
        }

    }

    // 실제로 방 하나를 floor 타일로 추가
    //기존 바닥과 겹치면 false 반환 , 겹치지 않ㅇ들 경우 floor 타일로 채우고 rooms 에 방 메타를 등록
    bool AddRoom(Vector2Int center , int size, RoomType type)
    {
        // 겹침 검사
        for (int x = -size / 2; x < size / 2; x++)
        {
            for (int y = -size / 2; y < size / 2; y++)
            {
                Vector2Int tile = center + new Vector2Int(x, y);
                if(floors.Contains(tile))
                {
                    return false;
                }
            }

        }
        // 방 메타 데이터 등록


        Room room = new Room(center, size, type);
        rooms[center] = room;

        // 3 . 방 영역을 floors 에 채운다

        for (int x = -size / 2; x < size / 2; x++)
        {
            for (int y = -size / 2; y < size / 2; y++)
            {
                floors.Add(center + new Vector2Int(x, y));
            }

        }
        return true;

    }

    void ConnectRooms()
    {
        var roomList = new List<Room>(rooms.Values);
        
        for(int i = 0; i < roomList.Count -1; i++)
        {
            CreateCorridor(roomList[i].center, roomList[i + 1].center);
        }
         
    }
    // 두 좌표 사이를 x축 -> y축 순서로 직선 복도로 판다
    // 굽이 치는 L자 모양이 나온다
    void CreateCorridor (Vector2Int start , Vector2Int end )
    {
        Vector2Int current = start;
        //x축 정렬 : start.x -> end.x 로 한칸식 이동하며 바닥 타일 추가 
        while(current.x != end.x)
        {
            floors.Add(current);
            current.x += (end.x > current.x) ? 1 : -1;
        }

        // y축 정렬 : x가 같아진 뒤 start.y -> end .y 로 한칸씩 이동
        while (current.x != end.x)
        {
            floors.Add(current);
            current.y += (end.y > current.y) ? 1 : -1;
        }
        floors.Add (end);// 마지막 목적지 칸도 바닥처리
    }

    // 바닥 주변의 8방향을 스캔하여 , 바닥이 아닌 칸을 walls 로 채운다
    void CreateWalls()
    {
        Vector2Int[] dirs =
            {
                Vector2Int.up  , Vector2Int.down , Vector2Int.left  ,Vector2Int.right,
                new Vector2Int(1,1),new Vector2Int(1,-1),new Vector2Int(-1,1),new Vector2Int(-1,-1)
            };

        foreach(var floor in floors)
        {
            foreach(var dir in dirs)
            {
                Vector2Int wallPos = floor + dir;
                if(!floors.Contains(wallPos))
                {
                    walls.Add(wallPos);  
                }
            }
        }

    }

    // 타일을 Unity 오브젝트로 렌더러
    // 바닥: Cube (0.1) , 벽 Cube(1) , 방 색 구분 
    // 
    void Render()
    {
        foreach(var pos in floors)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(pos.x, 0, pos.y);
            cube.transform.localScale = new Vector3(1f,0.1f,1f);
            cube.transform.SetParent(transform);

            Room room = GetRoom(pos);
            if(room != null)
            {
                cube.GetComponent<Renderer>().material.color = room.GetColor();
            }
            else
            {
                cube.GetComponent<Renderer>().material.color    = Color.white;
            }

        }
        foreach (var pos in walls)
        {
            GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
            cube.transform.position = new Vector3(pos.x, 0.5f, pos.y);
            cube.transform.SetParent (transform);
            cube.GetComponent<Renderer>().material.color = Color.black;
        }

    }

    // 어떤 바닥 좌표가 어느 방에 속하는지 역추적
    Room GetRoom(Vector2Int pos)
    {
        foreach(var room in rooms.Values)
        {
            int halfsize = room.size / 2;
            if(Mathf.Abs(pos.x - room .center.x) < halfsize && Mathf .Abs (pos.y  - room.center.y) < halfsize)
            {
                return room;
            }
        }
        return null;
    }


    void SpawnObjects()
    {
        foreach (var room in rooms.Values)
        {
            switch(room.type)
            {
                case RoomType.Start:
                    break; 

                    case RoomType.Normal:
                    if (spawnEnemies) SpawnEnemiesInRoom(room);
                    break;
                case RoomType.Treasure:
                    if (spawnEnemies) SpawnTreasureInRoom(room);
                    break;
                case RoomType.Boss:
                    if (spawnEnemies) SpawnBossInRoom(room);
                    break;

            }
        }
    }
    Vector3 GetRandomPositionInRoom(Room room)
    {
        float halfsize = room.size / 2f -1f;
        float randomX = room.center.x + UnityEngine.Random.Range(-halfsize, halfsize);
        float randomZ = room.center.y+ UnityEngine.Random.Range(-halfsize, halfsize);
        return new Vector3(randomX, 0.5f, randomZ);

    }

    void CreateEnemy(Vector3 position )
    {
        GameObject enemy = GameObject.CreatePrimitive (PrimitiveType.Cube); 
        enemy.transform.position = position;
        enemy.transform.localScale = Vector3.one * 0.8f;
        enemy.transform.SetParent(transform);
        enemy.name = "Enemy";
        enemy.GetComponent<Renderer>().material.color = Color.red;
        
    }
    void CreateBoss(Vector3 position)   // 보스 생성
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boss.transform.position = position;
        boss.transform.localScale = Vector3.one * 2f;
        boss.transform.SetParent(transform);
        boss.name = "Boss";
        boss.GetComponent<Renderer>().material.color = Color.cyan;

    }

    void CreateTreasure(Vector3 position)   // 보물 생성
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boss.transform.position = position;
        boss.transform.localScale = Vector3.one * 2f;
        boss.transform.SetParent(transform);
        boss.name = "Treasure";
        boss.GetComponent<Renderer>().material.color = Color.black;

    }
    void SpawnEnemiesInRoom(Room room)
    {
        for (int i = 0; i < enemisPerRoom; i++)
        {
            Vector3 spawnPos = GetRandomPositionInRoom (room);
        }
    }

    void SpawnBossInRoom(Room room)
    {
       Vector3 spawnPos = new Vector3(room.center.x , 1f , room.center.y);

        CreateBoss(spawnPos);
       
       
    }

    void SpawnTreasureInRoom(Room room)
    {
        Vector3 spawnPos = new Vector3(room.center.x, 0.5f, room.center.y);

        CreateTreasure(spawnPos);


    }

    void Clear()
    {
        rooms. Clear();
        floors.Clear();
        walls.Clear();

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject); 
        }
    }

}
