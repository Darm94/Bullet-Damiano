
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerLifeOnCollision : MonoBehaviour
{
    private GameManager _turretsManager;

    [SerializeField] private int lifePoints = 10;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private Image redOverlay;
    // Variable to track if the game is over
    private bool isGameOver = false;
    
    [Header("Health Recovery Settings")]
    private float timeSinceLastDamage = 0f;//vatiable to track if time goes on without damage
    [SerializeField] private float recoveryDelay = 6f;// Set the time of Life points recovery 
    [SerializeField] private float recoveryRate = 1f;
    private float timeAccumulated = 0f; //Variable to accumulate time for recovery delay
    
    private AudioSource _audioSource;
    int actualLifePoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _turretsManager = GetComponent<GameManager>();
        actualLifePoints = lifePoints;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isGameOver) return;
        // Logic to change the alpha based on life points
        float targetAlpha = Mathf.Clamp(1 - (actualLifePoints / (float)lifePoints), 0, 0.8f);
        redOverlay.color = Color.Lerp(redOverlay.color, new Color(1, 0, 0, targetAlpha), Time.deltaTime * 5f);
        // if no damage for 'recoveryDelay' seconds, start to recovery LP
        if (timeSinceLastDamage >= recoveryDelay && actualLifePoints < lifePoints)
        {
            RecoverHealth();
        }
        else
        {
            timeSinceLastDamage += Time.deltaTime; // add time without damage
        }
        
    }
    private void RecoverHealth()
    {
        // add time to accumulated time
        timeAccumulated += Time.deltaTime;
        
        if (timeAccumulated >= recoveryRate)
        {
            // Increase Life Points
            actualLifePoints = Mathf.Min(actualLifePoints + 1, lifePoints);

            // Reset timeAccumulated
            timeAccumulated -= recoveryRate;

            Debug.Log($"Recupero salute: {actualLifePoints}");
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision Enter");
        if (!other.gameObject.GetComponent<MoveBullet>()) return;
        actualLifePoints--;
        Debug.Log($"LP: {actualLifePoints}");
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
        }
        // Reset del timer per il recupero
        timeSinceLastDamage = 0f;
        
        //Game Over
        if (actualLifePoints <= 0)
        {
            Debug.Log("Game Over");
            // Stop every audio running
            _audioSource.Stop();
        
            Destroy(GetComponent<Rigidbody>());
            // Disable collider to avoit collisions in future
            Destroy(GetComponent<Collider>());
        
            // Disable Camera Controller to remove the movement of the player
            GetComponent<CameraController>().enabled = false;
        
            
            // Call Death animation and Sound
            StartCoroutine(RotateAndGameOver());
            _audioSource.PlayOneShot(deathClip);
            // Game over set for update avoid
            isGameOver = true;
            //Set fade to black background
            StartCoroutine(FadeOverlayToBlack());
        }
    }
    // Coroutine per fare fade out (nero) nell'overlay
    IEnumerator FadeOverlayToBlack()
    {
        float duration = 2f;
        Color targetColor = new Color(0, 0, 0, 0.5f); // Black
        float elapsedTime = 0f;

        Color initialColor = redOverlay.color;

        while (elapsedTime < duration)
        {
            redOverlay.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        redOverlay.color = targetColor; // Final color result
    }
    
    IEnumerator RotateCameraToSky(float duration)
    {
        // Set starting point and final point of rotation
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(-90f, startRotation.y, startRotation.z); // Rotating 90° on X (to look at the sky)
    
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, 2f, startPosition.z); 

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // calculate % for lerp (0 → 1)
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);  
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);    

            elapsedTime += Time.deltaTime;
            yield return null; // Wait next frame
        }

        // Set final rotations
        transform.rotation = targetRotation;
        transform.position = targetPosition;
        
    }
    IEnumerator RotateAndGameOver()
    {
        yield return StartCoroutine(RotateCameraToSky(1f)); //wait for rotation
        yield return new WaitForSeconds(1f);
        _turretsManager.GameOver(); // Call GAME MANAGER gameOver
    }
    
}
