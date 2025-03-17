
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
    // Variabile per tenere traccia se il gioco è finito
    private bool isGameOver = false;
    
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
        // Aggiungiamo la logica per aumentare l'alpha in base ai punti vita
        float targetAlpha = Mathf.Clamp(1 - (actualLifePoints / (float)lifePoints), 0, 0.8f);
        redOverlay.color = Color.Lerp(redOverlay.color, new Color(1, 0, 0, targetAlpha), Time.deltaTime * 5f);
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
        
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, 0f), 5f);
            // Chiama il Game Over nel GameManager
            StartCoroutine(RotateAndGameOver());
            _audioSource.PlayOneShot(deathClip);
            // Una volta che il Game Over è completo, settiamo isGameOver su true
            isGameOver = true;
            // Ora imposta l'overlay su nero (completamente opaco)
            StartCoroutine(FadeOverlayToBlack());
        }
    }
    // Coroutine per fare fade out (nero) nell'overlay
    IEnumerator FadeOverlayToBlack()
    {
        float duration = 2f;
        Color targetColor = new Color(0, 0, 0, 0.5f); // Colore nero
        float elapsedTime = 0f;

        Color initialColor = redOverlay.color;

        while (elapsedTime < duration)
        {
            redOverlay.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        redOverlay.color = targetColor; // Imposta il colore finale
    }
    
    IEnumerator RotateCameraToSky(float duration)
    {
        // Imposta i punti iniziali e finali
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(-90f, startRotation.y, startRotation.z); // Ruota di 90° sull'asse X (verso l'alto)
    
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, 2f, startPosition.z); // Arriva a y = 2 (non 0)

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Normalizza il tempo (0 → 1)
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);  // Lerp per la rotazione
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);    // Lerp per la posizione

            elapsedTime += Time.deltaTime;
            yield return null; // Aspetta il frame successivo
        }

        // Imposta i valori finali esatti per evitare errori di arrotondamento
        transform.rotation = targetRotation;
        transform.position = targetPosition;
        
    }
    IEnumerator RotateAndGameOver()
    {
        yield return StartCoroutine(RotateCameraToSky(1f)); // Aspetta la rotazione
        yield return new WaitForSeconds(1f);
        _turretsManager.GameOver(); // Chiama il metodo di Game Over
    }
    
}
