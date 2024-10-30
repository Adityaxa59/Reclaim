using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Giacomo
{
    public class GameManager : Singleton<GameManager>
    {
        public static int gameTickFrequency = 20;
        public List<Enemy> enemies;


        private void Awake()
        {
            enemies = new List<Enemy>();
        }

        public static void AddEnemy(Enemy enemy)
        {
            if (!Instance.enemies.Contains(enemy))
                Instance.enemies.Add(enemy);
        }

        public static void RemoveEnemy(Enemy enemy)
        {
            if (Instance.enemies.Contains(enemy))
                Instance.enemies.Remove(enemy);
        }

        public static List<Enemy> GetEnemies() => Instance.enemies;

        public void LoseGame()
        {
            Debug.Log("GAME OVER");
        }
    }
}