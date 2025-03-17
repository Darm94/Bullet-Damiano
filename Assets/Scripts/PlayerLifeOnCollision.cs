
using UnityEngine;


public class PlayerLifeOnCollision : MonoBehaviour
{
    private GameManager _turretsManager;

    [SerializeField] private int lifePoints = 10;
    [SerializeField] private AudioClip[] audioClips;
    private AudioSource _audioSource;
    int actualLifePoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _turretsManager = GetComponent<GameManager>();
        actualLifePoints = lifePoints;
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Enter");
        if (!other.gameObject.GetComponent<MoveBullet>()) return;
        actualLifePoints--;
        Debug.Log($"LP: {actualLifePoints}");
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
        }
        //Game Over
        if (actualLifePoints <= 0)
        {
            Debug.Log("Game Over");
            // Interrompe qualsiasi audio in esecuzione
            _audioSource.Stop();
        
            Destroy(GetComponent<Rigidbody>());
            // Disattiva il Collider per evitare collisioni future
            Destroy(GetComponent<Collider>());
        
            // Disattiva il controllo della camera
            GetComponent<CameraController>().enabled = false;
        
            // Chiama il Game Over nel GameManager
            _turretsManager.GameOver();
        }
    }
}
