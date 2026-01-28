using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Yuha
{
    /// <summary>
    /// <c>Building</c> Establishes how the building should behave, to burn or to be extinguished.
    /// </summary>
    public class Building : MonoBehaviour
    {
        [Header("COMPONENTS")] private Animator _anim;
        
        #region BURN
        [Header("BURN")]
        [SerializeField] private float _burnTimeLimit;
        private float _burnTime;
        private Coroutine _burnCoroutine;
        public bool IsBurning { get; private set; }
        public bool IsProtected { get; private set; }
        #endregion

        #region FLAME
        [Header("FLAME")]
        [SerializeField] private List <GameObject> _flames;
        private List<string> _spawnedFlames = new List<string>();
        private Dictionary<string, GameObject> _flameDict = new();
        private float _flameInspawnTime = 1.5f;
        #endregion
        
        #region PROTECT
        [Header("PROTECT")]
        [SerializeField] private float _protectionDuration = 5.75f;
        [SerializeField] private int _maxhits = 20;
        private int _currentHits;
        #endregion

        public Transform buildingCheckpoint;
        
        public static event Action<Building> OnBurnedDown;
        public static event Action OnExtinguished;

        public BuildingData data;
        
        private void Awake()
        {
            _anim = GetComponent<Animator>();
            DefineFlames();
        }

        private void Start()
        {
            DeactivateFlames();
        }

        private void DefineFlames()
        {
            for (int i = 0; i < _flames.Count; i++)
            {
                _flameDict.Add("Flame" + (i + 1), _flames[i]);
            }
        }
        
        private void DeactivateFlames()
        {
            foreach (var flame in _flames)
            {
                flame.SetActive(false);
            }
        }

        private void UpdateFlames()
        {
            float burnProgress = _burnTime / _burnTimeLimit;
            if (burnProgress >= 0.75)
            {
                SpawnFlame("Flame3");
            }
            else if (burnProgress >= 0.5)
            {
                SpawnFlame("Flame2");
            }
            else if (_burnTime > 1)
            {
                SpawnFlame("Flame1");
            }
        }

        private void SpawnFlame(string currentFlame)
        {
            if (_spawnedFlames.Contains(currentFlame)) return;
            
            _flameDict[currentFlame].SetActive(true);
            
            StartCoroutine(GrowFlame(currentFlame));
            
            _spawnedFlames.Add(currentFlame);
        }


        private IEnumerator GrowFlame(string currentFlame)
        {
            float elapsed = 0f;
            Vector3 startScale = Vector3.one;
            Vector3 targetScale = Vector3.one * 100;
    
            while (elapsed < _flameInspawnTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _flameInspawnTime;
                _flameDict[currentFlame].transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }

            _flameDict[currentFlame].transform.localScale = targetScale;
            
            yield return null;
        }

        #region BURN
        /// <summary>
        /// <c>Burn</c> Responsible for making the building burn while it's not destroyed.
        /// </summary>
        public void Burn()
        {
            if (IsBurning) return;
            
            IsBurning = true;
            _burnCoroutine = StartCoroutine(Ignite());
        } 

        private IEnumerator Ignite()
        {
            while (_burnTime < _burnTimeLimit)
            {
                _burnTime += Time.deltaTime;
                UpdateFlames();
                yield return null;
            }

            yield return BurnDownBuilding();
        }
        
        private IEnumerator BurnDownBuilding()
        {
            DeactivateFlames();
            IsBurning = false;
            _anim.SetTrigger("HouseDown");
            OnBurnedDown?.Invoke(this);
            yield break;
        }
        #endregion

        #region EXTINGUISH
        private void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("WaterParticle"))
            {
                TryExtinguish();
            }
        }

        /// <summary>
        /// <c>TryExtinguish</c> Adds a Current hit to the int and checks if the current hits is
        /// equal or over the maximum hits needed, in addition to also making sure the building is
        /// on fire and the hp is above 1.
        /// <c>TryExtinguish</c> If the all of those parameters are met, The Extinguish function activates.
        /// </summary>
        private void TryExtinguish()
        {
            if (!IsBurning) return;
                
            _currentHits++;
            if (_currentHits >= _maxhits)
            {
                StartCoroutine(Extinguish());
            }
        }
        
        /// <summary>
        /// <c>Extinguish</c> Extinguishes the building and starts the protection timer,
        /// during that time the building can be lit on fire again.
        /// </summary>
        private IEnumerator Extinguish()
        {
            IsBurning = false;
            if (_burnCoroutine != null)
            {
                StopCoroutine(_burnCoroutine);
                _burnCoroutine = null;
            }
            
            _currentHits = 0;
            StartCoroutine(Protect());
            DeactivateFlames();
            _burnTime = 0;
            OnExtinguished?.Invoke();
            yield break;
        }
        
        private IEnumerator Protect()
        {
            IsProtected = true;
            yield return new WaitForSeconds(_protectionDuration);
            IsProtected = false;
        }
        #endregion
    }
}