using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEngine.Rendering.Universal;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEditor.Build.Content;
using SupanthaPaul;
//using Cinemachine;

// 아이템의 정보를 저장하는 클래스. 나중에 합칠 때 별도의 스크립트로 생성
public class ItemData
{
    public string id { get; set; }
    public int quantity { get; set; }
    /* 추가 필요 */
}

public class StageManager : MonoBehaviour
{
    private int mapCount = 0;
    //맵 생성을 위한 프리팹 지정
    [Header("StartingMap")]
    public GameObject StartingMap;
    [Header("Candidate")]
    public List<GameObject> OtherPreset;
    [Header("Last")]
    public GameObject LastMap;
    //씬에서 동작할 플레이어 프리팹 지정
    [Header("Player")]
    public GameObject playerPrefab;
    public Vector3 playerSpawn = new Vector3(0f, 0f, 0f);
    //public CinemachineVirtualCamera vcam; // Cinemachine 문제 고쳐지면 주석 해제

    //맵 생성 위치 결정을 위한 맵의 크기와 다음 맵 생성 위치변수. Y축만 고려. X축 필요 시 추가
    [Header("Map Height")]
    public float MapHeight = 16.5f; //실제 맵 크기에 따라 조정 필요
    private float nextMapY = 0f;

    //플레이어의 위치 추적을 위한 플레이어 지정
    [Header("Player Transform")]
    public Transform PlayerTransform;
    [Header("Threshold")]
    public float Threshold = 17f; // 맵 생성 타이밍 감지를 위한 임계값 설정

    //스크립트 참조
    [Header("Ref Player")]
    public PlayerController player;
    //[Header("Ref Enemy")]
    //public Enemy enemy;

    //이벤트 발행
    public event System.Action<bool> OnStageFail;
    public event System.Action<bool> OnStageClear;
    public event System.Action OnStageEscape;
    public event System.Action<int> OnAddScore;

    //스테이지 내에서 필요한 변수
    bool IsClear = false;
    bool CheckTrigger = false;
    int StageScore = 0;
    float SpawnBudget;
    //추가//

    // 아이템을 보관하는 임시 인벤토리 생성
    private List<ItemData> tmpinventory = new List<ItemData>(); // 멀티플레이의 경우 딕셔너리 사용 <PID, List<ItemData>>

    /* 합칠 때 주석 해제
    private void OnEnable()
    {
        // 다른 스크립트의 이벤트 구독
        player.OnDeath += GameOver;
        player.OnEscape += StageEscape;
        player.OnGetItem += GetItem;
        enemy.OnDeath += CalcPoint;
        enemy.OnDeath += ClearAssurance;
        enemy.OnDeath += StageClear;
    }
    */

    void Start()
    {
        IsClear = false;
        mapCount += 1;
        /* Cinemachine 문제 고쳐지면 주석해제
        //시작 맵 불러오기
        if (StartingMap != null)
        {
            Instantiate(StartingMap, Vector3.zero, Quaternion.identity);
            //다음 맵이 생성될 위치 조정
            nextMapY = -MapHeight;
            Debug.Log("StartingMap Called");
        }
        else Debug.LogError("StartingMap Required");

        //플레이어 불러오기
        if (playerPrefab != null)
        {
            GameObject SpawnedPlayer = Instantiate(playerPrefab, playerSpawn, Quaternion.identity);
            Debug.Log("Player Spawned");
            vcam.Follow = SpawnedPlayer.transform;
            vcam.LookAt = SpawnedPlayer.transform;
        }
        else Debug.Log("Player Setting Required");
        */
        nextMapY = -MapHeight;
        Debug.Log("nextMapY: " + nextMapY);
    }

    //함수 이름: SpawnRandomMap
    //기능: 플레이어의 진행을 추적하여 다음에 생성될 맵을 랜덤하게 결정하는 함수
    //파라미터: X
    //반환값: X
    public void SpawnRandomMap()
    {
        mapCount += 1;
        //랜덤 프리셋이 없을 경우 리턴
        if (OtherPreset.Count == 0)
        {
            Debug.Log("RandomMap Required"); return;
        }
        // 5번째 맵이 마지막
        if(mapCount == 5)
        {
            Vector3 SpawnLoc = new Vector3(0, nextMapY, 0);
            Instantiate(LastMap, SpawnLoc, Quaternion.identity);
            Debug.Log("Last Map");
            nextMapY = nextMapY - 1242148 * MapHeight;
        }

        else
        {
            //랜덤 프리셋의 맵 결정. 이후에 랜덤 관련 보정 필요
            int MapIndex = Random.Range(0, OtherPreset.Count);
            GameObject SelectedMap = OtherPreset[MapIndex];

            //선택된 맵을 정해진 위치에 생성
            Vector3 SpawnLoc = new Vector3(0, nextMapY, 0);
            Instantiate(SelectedMap, SpawnLoc, Quaternion.identity);

            //다음 맵이 생성될 위치 조정
            nextMapY -= MapHeight;
            Debug.Log("nextMapY: " + nextMapY);
        }

 
    }

    //함수 이름: ClearAssurance
    //기능: 클리어 조건을 전부 충족시키는 함수
    //파라미터: X
    //반환값: X
    public void ClearAssurance(bool boss)
    {
        if (boss)
        {
            IsClear = true; CheckTrigger = true;
        }
        Debug.Log("Meet Clear Condition");
    }


    /* 이 이하는 이벤트 체이닝으로, 나중에 합칠 때 인자, 기능 수정 필요 */

    //함수 이름: CalcPoint
    //기능: 적을 처치했을 때 점수와 획득골드를 받아와서 처리하는 함수
    //파라미터: int score -> 해당 스테이지의 점수 증가량
    //반환값: X (이벤트를 구독하는 스크립트에게 스테이지 점수를 전달)
    public void CalcPoint(int score)
    {
        StageScore += score;
        OnAddScore?.Invoke(StageScore);
    }
   

    //함수 이름: GameOver
    //기능: 스테이지 실패를 알리는 함수
    //파라미터: X
    //반환값: X (이벤트를 구독하는 스크립트에게 false 전달)
    public void GameOver()
    {
        OnStageFail?.Invoke(IsClear);
        Debug.Log("Game Over");
        tmpinventory.Clear();
    }

    //함수 이름: StageClear
    //기능: 스테이지 클리어를 알리는 함수
    //파라미터: X
    //반환값: X (이벤트를 구독하는 스크립트에게 true 전달)
    public void StageClear(bool boss)
    {
        if (CheckTrigger && boss) OnStageClear?.Invoke(IsClear);
        Debug.Log("Stage Clear");
        /* 게임 매니저의 글로벌 인벤토리에 추가 */
    }

    //함수 이름: StageEscape
    //기능: 게임이 중간에 중단되었음을 알리는 함수
    //파라미터: bool trigger -> player로부터 '정당한 방식'임을 true로 받거나 그렇지 않으면 false
    //반환값: X (이벤트 전달값도 없음)
    public void StageEscape(bool trigger)
    {
        OnStageEscape?.Invoke();
        Debug.Log("Stage Escape");
        if(!trigger) tmpinventory.Clear();
        /* else 게임매니저의 글로벌 인벤토리에 추가 */
    }


    //함수 이름: GetItem
    //기능: 획득한 아이템을 임시 인벤토리에 추가
    //파라미터: ItemData item -> 인벤토리에 추가할 아이템
    //반환값: X
    public void GetItem(ItemData item)
    {
        if (item.id != "clear") tmpinventory.Add(item); // 멀티플레이 구현 시 동기화 처리해야
        Debug.Log(item.id + " 획득");

        if (item.id == "tmp") CheckTrigger = true; // id는 임시. 특정 지역을 지나기 위한 트리거 체크(필요 시)
        if (item.id == "clear") // 클리어 처리 테스트용. "clear" id를 가진 아이템 필요
        {
            ClearAssurance(true);
            StageClear(true);
        }

    }

    /*   이벤트 체이닝 끝    */


    // 적 스폰 관련 함수 (추후에 추가)
    public void ModifyDifficulty()
    {
        // enemy spawncost에 가중치를 부여
    }

    public void SpawnEnemy()
    {

    }

    void Update()
    {
        //플레이어 설정이 안된 경우 리턴
        if (PlayerTransform == null)
        {
            Debug.Log("Player Required");
            return;
        }
        //플레이어가 맵의 특정 깊이에 도달하면 다음 맵을 불러옴
        while(PlayerTransform.position.y <= nextMapY + Threshold)
        {
            SpawnRandomMap();
            Debug.Log("nextMap Loaded");
        }

        /*
        //맵의 최상단으로 이동 시 탈출 시도임을 확인
        if (PlayerTransform.position.y > -Threshold)
        {
            StageEscape(false); // 플레이를 중단하되, 획득한 아이템이 초기화
        }
        */
    }

    /* 합칠 때 주석 해제
    private void OnDisable()
    {
        // 이벤트 구독 해제.
        player.OnDeath -= GameOver;
        player.OnEscape -= StageEscape;
        player.OnGetItem -= GetItem;
        enemy.OnDeath -= CalcPoint;
        enemy.OnDeath -= ClearAssurance;
        enemy.OnDeath -= StageClear;
    }
    */
}
