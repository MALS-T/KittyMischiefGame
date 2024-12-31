using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CurrentData currentData;

    //skin select things
    public GameObject selectedSkinPrefab;
    private Sprite spriteToUse;
    public GameObject catSprite;
    [SerializeField] private Animator animator;
    
    public GameObject gameOverUI;
    public GameObject gameScreenUI;
    public GameObject pauseScreenUI;

    public PlayerScript player;
    public Rigidbody2D playerRB;
    public Spawner spawner;

    public int score;

    public TextMeshProUGUI scoreCounter;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI breakCounterText;

    public TextMeshProUGUI finalStatsText;

    public int objectSpeed;
    public bool keepMovingObjects = true;

    [SerializeField] private GameObject foreGround;
    [SerializeField] private GameObject midGround;
    [SerializeField] private GameObject backGround;

    [SerializeField] private float foreGroundSpd;
    [SerializeField] private float midGroundSpd;
    [SerializeField] private float backGroundSpd;


    private bool stopWatchOn;
    private float currentTime;
    private int breakCount;

    // Start is called before the first frame update
    void Start()
    {
        currentData.LoadData();
        
        animator.SetInteger("catSelected", currentData.currentCat);
        
        gameOverUI.SetActive(false);
        gameScreenUI.SetActive(true);

        currentTime = 0;
        StartStopwatch();

        currentData.LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        if(keepMovingObjects)
        {
            // keep moving objects to left at set speed
            foreach(GameObject obj in spawner.gameObjectsSpawned)
            {
                if(obj != null)
                {
                    obj.transform.position += CalculateMovementVector(objectSpeed);
                }
            }

            //parallax background
            foreGround.transform.position += CalculateMovementVector(foreGroundSpd);
            midGround.transform.position += CalculateMovementVector(midGroundSpd);
            backGround.transform.position += CalculateMovementVector(backGroundSpd);

            //teleporter is in BGTeleporter Script
        }

        if(stopWatchOn)
        {
            currentTime += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);

            currentTimeText.text = time.ToString(@"mm\:ss\:ff");
        }
    }

    public void StartStopwatch()
    {
        stopWatchOn = true;
    }

    public void EndStopwatch()
    {
        stopWatchOn = false;
    }

    private Vector3 CalculateMovementVector(float speed)
    {
        return Vector3.left * speed * Time.deltaTime;
    }


    public void AddPoint(int points)
    {
        score += points;

        scoreCounter.text = score.ToString();
    }

    public void AddBreakCount()
    {
        breakCount++;
        breakCounterText.text = breakCount.ToString();
    }

    public void GameOver()
    {
        finalStatsText.text = $"Score: {score}\nTime Survived: {currentTimeText.text}\n Obstacles Broken: {breakCount}";

        gameScreenUI.SetActive(false);
        pauseScreenUI.SetActive(false);

        gameOverUI.SetActive(true);
        keepMovingObjects = false;

        spawner.continueHoles = false;
        spawner.continueObjects = false;
        spawner.continuePlatforms = false;
        spawner.continueCollectibles = false;
        player.canAttack = false;

        currentData.totalScore += score;
        currentData.SaveData();

        EndStopwatch();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ContinueGame()
    {
        playerRB.constraints = ~RigidbodyConstraints2D.FreezePositionY;
        playerRB.velocity = new Vector3(0, -0.1f, 0); // Reset Y velocity if not it floats...
        
        player.canAttack = true;
        pauseScreenUI.SetActive(false);

        StartStopwatch();
        keepMovingObjects = true;

        spawner.continueHoles = true;
        spawner.continueObjects = true;
        spawner.continuePlatforms = true;
        spawner.continueCollectibles = true;
    }

    public void PauseGame()
    {
        playerRB.constraints = RigidbodyConstraints2D.FreezePositionY;
        player.canAttack = false;
        pauseScreenUI.SetActive(true);

        EndStopwatch();
        keepMovingObjects = false;

        spawner.continueHoles = false;
        spawner.continueObjects = false;
        spawner.continuePlatforms = false;
        spawner.continueCollectibles = false;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }

}
