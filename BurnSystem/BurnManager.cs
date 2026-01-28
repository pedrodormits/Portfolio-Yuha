using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Yuha
{
    /// <summary>
    /// <c>BurnManager<c> Decides which building should burn or be saved.
    /// </summary>
    public class BurnManager : MonoBehaviour
    {
        public static BurnManager Instance;
        
        [Header("BURNING")]
        [SerializeField] private float _startDelay = 3;
        [SerializeField] private float _burnDelay = 1.5f;
        [SerializeField] private int _buildingsInSequence = 5;
        private Coroutine _burnSequence; 
        
        [Header("BUILDING")] [SerializeField] private List<Building> _buildingsInGame = new List<Building>();
        
        [Header("AUDIO")]
        [SerializeField] private AudioClip _startFireClip;
        [SerializeField] private AudioClip _burningClip;
        private bool _burningClipIsPlaying;

        public static event Action OnBurnLoopEnd;

        private void OnEnable() => PlayerHealth.OnGameOver += EndBurnSequence;

        private void OnDisable() => PlayerHealth.OnGameOver -= EndBurnSequence;

        private void Awake() => Instance = this;
        
        /// <summary>
        /// <c>StartBurnSequence</c> starts the burning sequence.
        /// </summary>
        [ContextMenu("Burn")]
        public void StartBurnSequence() => _burnSequence = StartCoroutine(BurnSequence());
        
        private IEnumerator BurnSequence()
        {
            yield return new WaitForSeconds(_startDelay);
            for (int i = 0; i < _buildingsInSequence; i++)
            {
                BurnRandomBuilding();
                yield return new WaitForSeconds(_burnDelay);
            }
            
            yield return new WaitUntil(() => { return !AreBuildingsBurning();});
            OnBurnLoopEnd?.Invoke();
            yield return null;
        }

        private bool AreBuildingsBurning()
        {
            var arebuildingsBurning = _buildingsInGame.Any(building => building.IsBurning);
            return arebuildingsBurning;
        }
        
        /// <summary>
        /// <c>EndBurnSequence</c> is the method that is called when the player has zero health.
        /// </summary>
        private void EndBurnSequence()
        {
            if (_burnSequence != null)
            {
                StopCoroutine(BurnSequence());
                AudioManager.Instance.StopAudioClip();
            }
        }

        /// <summary>
        /// <c>BurnRandomBuilding</c> makes a random building from the _buildingsInGame List burn.
        /// </summary>
        private void BurnRandomBuilding()
        {
            if (IsBuildingListEmpty()) return; 
            
            List<Building> availableBuildings = new List<Building>();
            foreach (Building building in _buildingsInGame)
            {
                if (building.IsProtected) continue;

                if (building.IsBurning) continue;
                
                availableBuildings.Add(building);
            }

            if (availableBuildings.Count == 0) return;
            
            int buildingsSelection = Random.Range(0, availableBuildings.Count);
            Building selectedBuilding = availableBuildings[buildingsSelection];
            selectedBuilding.Burn();
            AudioManager.Instance.PlayAudioClip(_startFireClip);
            AudioManager.Instance.PlayAudioClipLoop(_burningClip, true);
        }
        
        /// <summary>
        /// <c>BurnSpecificBuilding</c> makes a specific building burn based on the provided index.
        /// </summary>
        /// <param name="selectedBuildingIndex">The given building in the editor</param>
        public void BurnSpecificBuilding(Building selectedBuilding) => selectedBuilding.Burn();
        
        /// <summary>
        /// <c>IsBuildingListEmpty</c> checks if there are buildings in the list.
        /// </summary>
        /// <returns>a bool depending on if there are buildings in the list</returns>
        private bool IsBuildingListEmpty()
        {
            if (_buildingsInGame == null || _buildingsInGame.Count <= 0)
            {
                return true;
            }

            return false;
        }
    }
}