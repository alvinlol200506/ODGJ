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

        [Header("Databases")]
        [SerializeField] private List<GhostData> allGhosts;
        [SerializeField] private List<RequestData> allMissions;

        [Header("Spawn Settings")]
        [SerializeField] private float minSpawnDelay = 3f;
        [SerializeField] private float maxSpawnDelay = 8f;

        private List<RequestData> _availableMissions;

        private void Start()
        {
            foreach (var ghostData in allGhosts)
            {
                if (PlayerPrefs.GetInt("Unlock_" + ghostData.name, 0) == 1)
                {
                    UIGhostCard newGhost = Instantiate(ghostPrefab, ghostContainer);
                    newGhost.Setup(ghostData);
                }
                else
                {
                    // Kalau belum terbuka, lewati (jangan di-spawn)
                    Debug.Log($"[GameSpawner] {ghostData.ghostName} masih terkunci di Padepokan, skip spawn.");
                }
            }

            _availableMissions = new List<RequestData>(allMissions);
            StartCoroutine(SpawnMissionRoutine());
        }

        private IEnumerator SpawnMissionRoutine()
        {
            while (_availableMissions.Count > 0)
            {
                float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
                float elapsed = 0f;

                // Loop custom buat timer spawn
                while (elapsed < waitTime)
                {
                    // Timer nunggu misi baru cuma jalan kalau panel dispatch LAGI DITUTUP
                    if (UIDispatchController.Instance != null && !UIDispatchController.Instance.IsOpen)
                    {
                        elapsed += Time.deltaTime;
                    }

                    // Tunggu sampe frame berikutnya
                    yield return null;
                }

                // Kalau udah nunggu waktunya, spawn misi random
                int randomIndex = Random.Range(0, _availableMissions.Count);
                RequestData chosenMission = _availableMissions[randomIndex];

                UIMissionCard newMission = Instantiate(missionPrefab, missionContainer);
                newMission.Setup(chosenMission);

                _availableMissions.RemoveAt(randomIndex);
            }

            Debug.Log("Semua misi sudah habis!");
        }
    }
}