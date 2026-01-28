using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Yuha
{
    /// <summary>
    /// <c>PlayerHealth</c> Controls everything about the player health.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("HEALTH")]
        public bool PlayerIsDead { get; private set; }
        [SerializeField] private List<GameObject> _hearts;

        [Header("ANIMATION")]
        [SerializeField] private float _RemovalSpeed;
        [SerializeField] private float _yPos;
        
        public static event Action OnGameOver;

        private void OnEnable() => Building.OnBurnedDown += PlayerLosesHealth;

        private void OnDisable() => Building.OnBurnedDown -= PlayerLosesHealth;

        /// <summary>
        /// <c>PlayerLosesHealth</c> Decreases Player's HP when a building gets burned down.
        /// </summary>
        private void PlayerLosesHealth(Building building)
        {
            if (PlayerIsDead) return;
            
            StartCoroutine(RemoveHeart());
        }

        /// <summary>
        /// <c>RemoveHeart</c> Moves the heart upwards and decreases Player's HP.
        /// </summary>
        private IEnumerator RemoveHeart()
        {
            GameObject heart = _hearts[0];
            _hearts.Remove(heart);
            Vector3 destination = new Vector3(
                heart.transform.position.x,
                heart.transform.position.y + _yPos,
                heart.transform.position.z);
            while (Vector3.Distance(heart.transform.position, destination) > 0.01f)
            {
                heart.transform.position = Vector3.MoveTowards(
                    heart.transform.position,
                    destination,
                    _RemovalSpeed * Time.deltaTime);
                yield return null;
            }
            
            if (!_hearts.Any() && !PlayerIsDead)
            {
                GameOver();
            }
        }
        
        /// <summary>
        /// <c>GameOver</c> Ends the game.
        /// </summary>
        private void GameOver()
        {
            PlayerIsDead = true;
            OnGameOver?.Invoke();
            Building.OnBurnedDown -= PlayerLosesHealth;
        }
    }
}