using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Range(0.1f,5)] private float movementSpeed = 4.5f;
    [SerializeField] [Range(1, 100)] private float rSpeed = 80;  //grades for second
    [SerializeField] [Range(1, 150)] private float mouseSpeed = 100f;  
    [SerializeField] [Range(1, 10)] private float turboSpeed = 1.5f;
        
    [SerializeField] private bool useMouseLook = true;  // moves rotating with keyboard or mouse
    [SerializeField] private CursorLockMode useLockState = CursorLockMode.Locked;
    
    [Header("Footsteps Settings")]
    [SerializeField] private AudioClip[] footstepSounds; // Differents steps sounds
    [SerializeField] private float stepInterval = 0.5f; // Time between one step and the after one
    private float nextStepTime = 0f; // Timer for next step
    private AudioSource _audioSource;
    
    //variables for update
    private float _turbo;
    private float _h;
    private float _v;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (useMouseLook)
        {
            Cursor.lockState = useLockState;
        }
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        _turbo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? turboSpeed : 1;
        
        float mouse = Input.GetAxis("Mouse X");
        _v = Input.GetAxis("Vertical");
        _h = useMouseLook ? mouse : Input.GetAxis("Horizontal");


        float xDirection = useMouseLook ? Input.GetAxis("Horizontal")  : 0 ;
        float zDirection = _v  ;
        
         //added movementSpeed here cause it was canceled by the normalization
         Vector3 direction = new Vector3(xDirection, 0, zDirection).normalized *  (_turbo * Time.deltaTime * movementSpeed);
         transform.Translate(direction);
         
         // if it moves, play steps sound
         if (direction.magnitude > 0.01f && Time.time >= nextStepTime)
         {
             @Debug.Log("passo");
             PlayFootstepSound();
             nextStepTime = Time.time + stepInterval / _turbo; // Più veloci -> passi più frequenti
         }

         if (useMouseLook) // use mouse horizontal rotation  
         {
             transform.Rotate(new Vector3(0, mouseSpeed * Time.deltaTime * _h , 0));
         }
         else //use A and D 
         {
             transform.Rotate(Vector3.up * (rSpeed * Time.deltaTime * _h * _turbo));
         }
    }
    
    private void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0 || !_audioSource ) return;

        AudioClip stepSound = footstepSounds[Random.Range(0, footstepSounds.Length)]; // Suono casuale
        _audioSource.PlayOneShot(stepSound);
    }
}
