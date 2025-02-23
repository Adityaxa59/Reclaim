using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;

namespace Giacomo
{
    public class WaveManager : Singleton<WaveManager>
    {
        public float defaultWaveDelay;
        [SerializeField] GameObject resumeSpawningButton;
        public bool SpawningPaused => spawningPaused;
        [SerializeField] protected bool spawningPaused;
        public int CurrentWave => currentWave;
        protected int currentWave;
        public float NextWaveTime => nextWaveTime;
        protected float nextWaveTime;

        public bool isSpawningEnemies { get; protected set; }

        public event Action<int> SpawningNewWave;
        public event Action FinishedSpawningWave;

        protected List<Spawner> spawners;
        protected bool gameStarted;

        private void Start()
        {
            StartGame();
            GridManager.Instance.OnTileAdded += t => { if (t.tileId == "spawner") spawners.Add(t.GetComponent<Spawner>()); };
            GridManager.Instance.OnTileRemoved += p => { spawners.RemoveAll(x => x.GetComponent<Tile>().position == p); };
        }


        public void StartGame()
        {
            if (gameStarted) return;
            gameStarted = true;
            spawners = FindObjectsByType<Spawner>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

            StartCoroutine(SpawnWaves());
        }

        public void PauseSpawning(bool showResumeButton)
        {
            spawningPaused = true;
            if (showResumeButton)
                ShowResumeButton();
        }

        public void ShowResumeButton()
        {
            resumeSpawningButton?.SetActive(true);
        }

        protected void HideResumeBUtton()
        {
            resumeSpawningButton?.SetActive(false);
        }

        public void ResumeSpawning()
        {
            spawningPaused = false;
            nextWaveTime = Time.time;
            
        }

        protected IEnumerator SpawnWaves()
        {
            while (!CheckGameOver())
            {
                if (spawningPaused)
                    yield return new WaitUntil(() => !spawningPaused);

                HideResumeBUtton();

                isSpawningEnemies = true;
                SpawningNewWave?.Invoke(currentWave);
                float maxDelay = -1;
                foreach (Spawner spawner in spawners)
                {
                    float waveDelay = spawner.SpawnNextWave();
                    maxDelay = Mathf.Max(maxDelay, waveDelay);
                }

                if (maxDelay <= 0)
                    nextWaveTime = Time.time + defaultWaveDelay;
                else
                    nextWaveTime = Time.time + maxDelay;

                yield return new WaitUntil(() => spawners.All(x => x.isSpawning == false));
                FinishedSpawningWave?.Invoke();
                isSpawningEnemies = false;

                yield return new WaitUntil(IsTimerOver);
                currentWave++;
            }
        }

        protected bool IsTimerOver() => nextWaveTime <= Time.time;


        protected bool CheckGameOver()
        {
            //TODO: have a fixed max wave in WaveManager
            /*foreach(Spawner spawner in spawners)
                if (!spawner.finishedWaves)*/
                    return false;

            //return true;
        }

    }

}