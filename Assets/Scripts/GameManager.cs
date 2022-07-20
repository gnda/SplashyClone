using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> platformPrefabs;
    [SerializeField] private int stepAmount = 30, levelGemScore = 15;
    [SerializeField] private GameObject playerPrefab, textPrefab, 
        gemPrefab, specialBonusPrefab, flagPrefab;
    [SerializeField] private Transform playerSpawnTransform;
    [SerializeField] private string[] messages;

    private GameObject _playerGo, _uiManagerGo;
    private Platform[] _platforms;
    private TMP_Text _scoreText, _gemText;
    private int _platformPoints = 1;
    private float _levelDuration;
    private bool _isCounting, _isPaused;

    private void Awake()
    {
        _uiManagerGo = GameObject.Find("UIManager");
        _uiManagerGo.SetActive(false);
        _scoreText = _uiManagerGo.transform.Find("ScorePanel").GetChild(0)
            .GetComponentInChildren<TMP_Text>();
        _gemText = _uiManagerGo.transform.Find("ScorePanel").GetChild(1)
            .GetComponentInChildren<TMP_Text>();
        _playerGo = Instantiate(playerPrefab, playerSpawnTransform);
        _playerGo.GetComponent<Player>().gameManager = this;
        _levelDuration = 0.666f * stepAmount;

        // Platform generation
        GameObject platformGo = null;
        
        var bonusTargetCount = 0;
        var specialBonusCount = 0;
        for (var i = 0; i < stepAmount; i++)
        {
            var platformAmount = 0;
            var platformIdx = 0;
            float genNumber;
            
            // Platform X offset
            var xOffset = 0f;
            if ((i > 0) && ((i + 1) < stepAmount))
            {
                genNumber = Random.Range(0.0f, 1.0f);
                platformAmount = genNumber switch
                {
                    < 0.35f => 1,
                    > 0.8f and < 1.0f => 2,
                    _ => platformAmount
                };
                xOffset = Random.Range(-1.8f, 1.8f);
            }
            
            for (var j = 0; j <= platformAmount; j++) {
                // X offset if more than 1 platform
                if ((i > 0) && ((i + 1) < stepAmount) && (j > 0)) {
                    xOffset += 3.8f;
                }
                
                // Handle bonus & gems
                if (i > (stepAmount / 3))
                {
                    if ((bonusTargetCount < 15) && ((i + 1) < stepAmount))
                    {
                        platformIdx = Random.Range(0, platformPrefabs.Count);
                        if (platformIdx > 0) {
                            bonusTargetCount++;
                        }
                    }
                    else
                    {
                        platformIdx = 0;
                    }
                    if ((platformIdx == 0) && specialBonusCount < 2) {
                        genNumber = Random.Range(0.0f, 1.0f);
                        if (genNumber > 0.8f)
                        {
                            Instantiate(specialBonusPrefab,
                                new Vector3(xOffset, 0.15f, 5.2f * i), Quaternion.identity);
                            specialBonusCount++;
                        }
                    }
                }
                platformGo = Instantiate(platformPrefabs[platformIdx], 
                    new Vector3(xOffset, 0, 5.2f * i), Quaternion.identity);
                
                if ((i + 1) >= stepAmount) continue;
                genNumber = Random.Range(0.0f, 1.0f);
                if (genNumber > 0.85f) {
                    Instantiate(gemPrefab, platformGo.transform);
                }
            }
        }
        if (platformGo != null) {
            platformGo.GetComponent<Platform>().IsLast = true;
            Instantiate(flagPrefab, platformGo.transform);
        }
        _platforms = FindObjectsOfType<Platform>();

        // Camera setup
        var cameraGo = FindObjectOfType<Camera>().gameObject;
        cameraGo.GetComponent<Follow>().targetTransform = _playerGo.transform;
    }

    private void Start()
    {
        Time.timeScale = 1;
        
        foreach (var platform in _platforms)
        {
            var plate = platform.transform.Find("Plate");
            plate.transform.DOScaleX(0.7f, _levelDuration * 3);
            plate.transform.DOScaleZ(0.7f, _levelDuration * 3);
        }
        
        // Level movement
        _playerGo.transform.DOMoveZ(_playerGo.transform.position.z + stepAmount * 6, _levelDuration)
            .SetEase(Ease.Linear);
        
    }

    private void Update()
    {
        // Accelerate over time
        if (!_isPaused) {
            Time.timeScale += 0.00002f;
        }
    }

    private void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0;
    }

    public void HandleCollision(GameObject sourceGo, GameObject targetGo, Vector3 contactPoint)
    {
        if ((targetGo.name != "Plate") && (targetGo.name != "BonusTarget")) return;
        var platform = targetGo.GetComponentInParent<Platform>();
        if (platform.IsLast)
        {
            Win();
        } else {
            platform.Remove();
            var player = sourceGo.GetComponent<Player>();
            if (!player) return;
            if (!_uiManagerGo.activeInHierarchy)
            {
                _uiManagerGo.SetActive(true);
            }
            
            //Handle bonus
            if (targetGo.name == "BonusTarget")
            {
                _platformPoints++;
                if (_platformPoints == 6) {
                    foreach (var p in _platforms)
                    {
                        p.transform.DOMoveX(0, 0.5f);
                    }
                    player.transform.DOMoveX(0, 0.5f);
                    Time.timeScale = 2.0f;
                }
                var messageGo = Instantiate(
                    textPrefab, 
                    contactPoint + (Vector3.up) * 2, Quaternion.identity
                );
                var messageIdx = Random.Range(0, messages.Length);
                messageGo.GetComponentInChildren<TMP_Text>().text = messages[messageIdx];
            }
            
            player.Score += _platformPoints;
            _scoreText.text = $"{player.Score}";
            
            var scorePointGo = Instantiate(
                textPrefab, 
                contactPoint + (Vector3.up), Quaternion.identity
            );
            scorePointGo.GetComponentInChildren<TMP_Text>().text = $"+{_platformPoints}";
        }
    }

    public void HandleTrigger(GameObject sourceGo, GameObject targetGo)
    {
        var gem = targetGo.GetComponent<Gem>();
        if (gem != null)
        {
            targetGo.GetComponent<Gem>().Remove();
            sourceGo.GetComponent<Player>().GemCount++;
            _gemText.text = $"Gems: {sourceGo.GetComponent<Player>().GemCount}";
            if (!_uiManagerGo.activeInHierarchy)
            {
                _uiManagerGo.SetActive(true);
            }
        }
        else if (targetGo.name.Contains(specialBonusPrefab.name))
        {
            Destroy(targetGo);
            foreach (var platform in _platforms)
            {
                var plate = platform.transform.Find("Plate");
                plate.DOKill();
                var bonusColor1 = new Color(0.7f, 0f, 0.75f, 1);
                if (plate.GetComponent<Renderer>().material.color != bonusColor1)
                {
                    plate.DOScaleX(3.2f, 1.2f);
                    plate.DOScaleZ(3.2f, 1.2f);
                    plate.GetComponent<Renderer>().material.color = bonusColor1;
                }
                else
                {
                    plate.DOScaleX(3.6f, 1.2f);
                    plate.DOScaleZ(3.6f, 1.2f);
                    plate.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f, 1);
                }
            }
        }
    }

    public void Lose()
    {
        StopMovements();
        Destroy(_playerGo.GetComponent<Bounceable>());
        _playerGo.transform.DOMoveY(-5, 1).SetEase(Ease.Linear)
            .OnComplete(Exit);
    }

    private static void Exit()
    {
        Application.Quit();
    }

    private void Win()
    {
        if (_isCounting) return;
        StopMovements();
        StartCoroutine(CountGemsCoroutine());
    }

    private void StopMovements()
    {
        _playerGo.transform.DOKill();
        foreach (var platform in _platforms)
        {
            platform.transform.DOKill();
        }
        _playerGo.GetComponentInParent<PlayerController>().IsDisabled = true;
    }

    private IEnumerator CountGemsCoroutine()
    {
        _isCounting = true;
        var gemCount = _playerGo.GetComponent<Player>().GemCount;
        for (var i = gemCount; i <= gemCount + levelGemScore; i++)
        {
            _gemText.text = $"Gems: {i}";
            yield return new WaitForSeconds(0.4f);
        }
        _playerGo.GetComponent<Player>().GemCount += levelGemScore;
        Pause();
        Exit();
        _isCounting = false;
    }
}
