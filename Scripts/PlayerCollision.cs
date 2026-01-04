using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    bool dead = false;
    bool canTakeDamage = true;
    int maxApples = 10;
    float CoolDownTime;
    float speedIncForEveryChecPoint = 1f;
    float loopTimeOut = 10f;
    int Score = 0,ScoreInc = 100;
    int AppleCountForPower = 0;
    int activeHealthIndex = 0,appleCountForHealth = 0;

    [SerializeField] Player player;
    bool collisionAvoidPower = false;
    const string AnimTrigger = "Hit";
    string Coin = "coin";
    string apple = "Apple";
    string checkPointTag = "CheckPoint";
    string obstacle = "Obstacle";

    [Header("Required Components")]
    Path path;
    [SerializeField] GameObject obstacleGB;
    [SerializeField] TMP_Text buttonText;
    [SerializeField] GameObject[] HealthIndicator = new GameObject[3];
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] Animator animator;
    AudioSource audioSource;
    [SerializeField] AudioClip coinAudioClip;
    [SerializeField] AudioClip appleAudioClip;
    [SerializeField] AudioClip checkPointAudioClip;
    [SerializeField] AudioClip obstacleAudioClip;

    void Start()
    {
        path = FindFirstObjectByType<Path>();
        ScoreText.text = "SCORE :" + Score;
        audioSource = GetComponentInParent<AudioSource>();
        buttonText.text = "Apples : 0/"+maxApples;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(Coin))
        {
            Score += ScoreInc;
            ScoreText.text = "SCORE :" + Score;
            audioSource.PlayOneShot(coinAudioClip);
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag(apple))
        {   
            audioSource.PlayOneShot(appleAudioClip);
            if(!collisionAvoidPower)
            {
                AppleCountForPower++;
                buttonText.text = "Apples : " + AppleCountForPower +"/"+maxApples;
                if(AppleCountForPower >= maxApples)
                {
                    buttonText.text = "Activated";
                }
            }
            Destroy(other.gameObject);
            HealthCheckMethod();
        }
        else if((other.gameObject.CompareTag(checkPointTag)))
        { 
            audioSource.PlayOneShot(checkPointAudioClip);
            path.SpeedModifier(speedIncForEveryChecPoint);       
        }
        else if((other.gameObject.CompareTag(obstacle)))
        {
            if(!collisionAvoidPower)
            {
                if(canTakeDamage)
                {
                    canTakeDamage = false;
                    PlayerCollisionDitection();
                    StartCoroutine(GapBetweenDamage());
                }
            }  
        }
    }

    void HealthCheckMethod()
    {
        if(HealthIndicator[0].activeSelf) return;

        appleCountForHealth++;
        if(appleCountForHealth == HealthIndicator.Length)
        {
            activeHealthIndex--;
            HealthIndicator[activeHealthIndex].SetActive(true);
            appleCountForHealth = 0;
        }
    }

    void PlayerCollisionDitection()
    {
        if(dead) return;
        
        audioSource.PlayOneShot(obstacleAudioClip);

        if(activeHealthIndex < HealthIndicator.Length)
        {
            HealthIndicator[activeHealthIndex].SetActive(false);

            if(activeHealthIndex == HealthIndicator.Length-1)
            {
                dead = true;
                float speedRef = path.speedReturn();
                path.SpeedModifier(-speedRef);
                UiManager.uiManager.Dead();
                obstacleGB.SetActive(false);
                player.enabled = false;
                animator.SetBool("dead",true);
            }
            activeHealthIndex++;
        }
        if(animator.GetBool("slide") || animator.GetBool("jump")) return;
        animator.SetBool("gotHit",true);
    }

    IEnumerator GapBetweenDamage()
    {
        yield return new WaitForSeconds(1f);
        canTakeDamage = true;
    }

    public void OnPowerPressed()
    {
        if(AppleCountForPower >= maxApples && !dead)
        {
            AppleCountForPower = 0;
            collisionAvoidPower = true;
            StartCoroutine(CollisionActivation());
        }
    }
    
    IEnumerator CollisionActivation()
    {
        float loopIncrementor = loopTimeOut;
        while(0 < loopIncrementor)
        {
            loopIncrementor -= Time.deltaTime;
            buttonText.text = "Time : " + loopIncrementor.ToString("F1");
            yield return null;
        }
        collisionAvoidPower = false;
        buttonText.text = "Deactivated";
    }
}
