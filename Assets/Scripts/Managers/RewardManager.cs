using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class RewardManager : MonoBehaviour
    {
        [SerializeField] private GameObject _coinsPanel;
        // [SerializeField] private TextMeshProUGUI _counter;
        
        [SerializeField] private Vector3[] _initialPosition;
        [SerializeField] private Quaternion[] _initialRotation;

        [SerializeField] private List<GameObject> _coins = new List<GameObject>();

        [SerializeField] private int _coinsNumber;


        private static RewardManager _instance;

        // public static RewardManager Instance
        // {
        //     get => _instance;
        //     set => _instance = value;
        // }
        //
        // private void Awake()
        // {   
        //     if (Instance != null && Instance != this)
        //     {
        //         Destroy(this);
        //     }
        //
        //     else
        //     {
        //         Instance = this;
        //     }
        // }

        private void Start()
        {
            _initialPosition = new Vector3[_coinsNumber];
            _initialRotation = new Quaternion[_coinsNumber];
            
            for (int i = 0; i < _coinsPanel.transform.childCount; i++)
            {
                _initialPosition[i] = _coinsPanel.transform.GetChild(i).position;
                _initialRotation[i] = _coinsPanel.transform.GetChild(i).rotation;
            }
        }

        private void OnEnable()
        {
            _initialPosition = new Vector3[_coinsNumber];
            _initialRotation = new Quaternion[_coinsNumber];
            
            for (int i = 0; i < _coinsPanel.transform.childCount; i++)
            {
                _initialPosition[i] = _coinsPanel.transform.GetChild(i).position;
                _initialRotation[i] = _coinsPanel.transform.GetChild(i).rotation;
            }
        }

        private void Reset()
        {
            for (int i = 0; i < _coinsPanel.transform.childCount; i++)
            { 
                _coinsPanel.transform.GetChild(i).position = _initialPosition[i];
                _coinsPanel.transform.GetChild(i).rotation = _initialRotation[i];
            }
        }

        public void RewardCoins()
        {
            _coinsPanel.SetActive(true);
            
            Reset();
           
            var delay = 0f;

            for (int i = 0; i < _coinsPanel.transform.childCount; i++)
            {

                _coinsPanel.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
                

                _coinsPanel.transform.GetChild(i).GetComponent<RectTransform>().
                    DOAnchorPos(new Vector2(440f, 804f), 0.5f).SetDelay(delay + 0.3f).SetEase(Ease.OutBack);
                
                _coinsPanel.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.3f)
                    .SetEase(Ease.Flash);
                
                _coinsPanel.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.4f).SetEase(Ease.OutBack);

                delay += 0.1f;
            }
        }
    }
}