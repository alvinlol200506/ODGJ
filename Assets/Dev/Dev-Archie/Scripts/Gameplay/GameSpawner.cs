using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ODGJ.Dispatch
{
    public class GameSpawner : MonoBehaviour
    {
        [Header("Prefabs & Containers")]
        [SerializeField] private UIGhostCard ghostPrefab;
        [SerializeField] private UIMissionCard missionPrefab;
        [SerializeField] private Transform ghostContainer;
        [SerializeField] private Transform missionContainer;

        [Header("Databases (Drag ScriptableObjects kesini)")]
        [SerializeField] private List<GhostData> allGhosts;
        [SerializeField] private List<RequestData> allMissions;

        [Header("Spawn Settings")]
        [SerializeField] private float minSpawnDelay = 3f;
        [SerializeField] private float maxSpawnDelay = 8f;

        private List<RequestData> _availableMissions;

        private void Start()
        {
            // 1. Spawn semua hantu ke Ghost Container
            foreach (var ghostData in allGhosts)
            {
                UIGhostCard newGhost = Instantiate(ghostPrefab, ghostContainer);
                newGhost.Setup(ghostData);
            }

            // 2. Clone list misi biar kita bisa hapus yang udah ke-spawn (biar gak duplikat)
            _availableMissions = new List<RequestData>(allMissions);

            // 3. Mulai siklus spawn misi
            StartCoroutine(SpawnMissionRoutine());
        }

        private IEnumerator SpawnMissionRoutine()
        {
            while (_availableMissions.Count > 0)
            {
                // Tunggu waktu random
                float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
                yield return new WaitForSeconds(waitTime);

                // Pilih misi random dari sisa yang ada
                int randomIndex = Random.Range(0, _availableMissions.Count);
                RequestData chosenMission = _availableMissions[randomIndex];

                // Spawn di UI
                UIMissionCard newMission = Instantiate(missionPrefab, missionContainer);
                newMission.Setup(chosenMission);

                // Hapus dari list biar gak muncul lagi
                _availableMissions.RemoveAt(randomIndex);
            }

            Debug.Log("Semua misi sudah habis!");
        }
    }
}