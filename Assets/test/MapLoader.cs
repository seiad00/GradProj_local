using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEngine.Rendering.Universal;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEditor.Build.Content;
using SupanthaPaul;

public class MapLoader : MonoBehaviour
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

    // 맵 생성 시 EnemySpawner에게 맵이 로드되었음을 알리는 이벤트
    public event System.Action OnMapLoaded;

    void Start()
    {
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
        if (mapCount == 5)
        {
            Vector3 SpawnLoc = new Vector3(0, nextMapY, 0);
            Instantiate(LastMap, SpawnLoc, Quaternion.identity);
            Debug.Log("Last Map");
            nextMapY = nextMapY - 1242148 * MapHeight;
        }

        else
        {
            //랜덤 프리셋의 맵 결정. 실제 맵에 적용 시 랜덤 관련 보정 필요
            int MapIndex = Random.Range(0, OtherPreset.Count);
            GameObject SelectedMap = OtherPreset[MapIndex];

            //선택된 맵을 정해진 위치에 생성
            Vector3 SpawnLoc = new Vector3(0, nextMapY, 0);
            Instantiate(SelectedMap, SpawnLoc, Quaternion.identity);

            //다음 맵이 생성될 위치 조정
            nextMapY -= MapHeight;
            Debug.Log("nextMapY: " + nextMapY);
        }
        OnMapLoaded?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어 설정이 안된 경우 리턴
        if (PlayerTransform == null)
        {
            Debug.Log("Player Required");
            return;
        }
        //플레이어가 맵의 특정 깊이에 도달하면 다음 맵을 불러옴
        while (PlayerTransform.position.y <= nextMapY + Threshold)
        {
            SpawnRandomMap();
            Debug.Log("nextMap Loaded");
        }
    }
}
