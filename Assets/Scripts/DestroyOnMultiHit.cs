using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestroyOnMultiHit : MonoBehaviour
{
    [SerializeField] private int maxHitCount = 10;
    [SerializeField] private bool randomHitCount = true;
    private int currentHitCount;
    private int startHitCount;//if i want to reset the wall

    private bool stopCollisions = false;
    //To change color or transparency
    private Material _material;
    private float _destroyStepsPercent = 1;
    
    //to notify gameManager the destructions
    private GameManager _gameManager;
    //GameManager Property
    public GameManager GameManager {set => _gameManager = value;} //only setter at creation
    
    //AudioSource
    private AudioSource _audioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _audioSource = GetComponent<AudioSource>();

        if (randomHitCount)
        {
            startHitCount = Random.Range(1, maxHitCount);
        }
        _destroyStepsPercent = 1f / startHitCount;
        currentHitCount = startHitCount;
    }

    private void OnCollisionEnter(Collision other)
    {
        //2:03:33
        if (!other.gameObject.GetComponent<MoveBullet>() || stopCollisions) return;
        
        currentHitCount -= 1;
        _material.color -= new Color(0f, 0f, 0f, _destroyStepsPercent);

        Debug.Log($"{_destroyStepsPercent} --> alpha = {_material.color.a}");
        
        if (currentHitCount <= 0)
        {
            _gameManager.DidDestroyWall();

            if (_audioSource.clip)
            {
                Debug.Log("Playing audio...");
                _audioSource.Play();
                Debug.Log($"Audio clip length: {_audioSource.clip.length}");
                GetComponent<MeshRenderer>().enabled = false;
                Invoke(nameof(DestroyMe), _audioSource.clip.length); // wait the full duration of the audio before destroying 
                
            }
            else
            {
                Debug.Log("No audio clip found");
                
                Destroy(gameObject);
            }

            stopCollisions = true;

        }
        
    }

    private void DestroyMe()
    {
        _gameManager = null;
        Debug.Log("DESTROYING WALL"); 
        Destroy(gameObject);
    }
}
