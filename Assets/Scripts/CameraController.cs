using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Range(0.1f,5)] private float movementSpeed = 4.5f;
    [SerializeField] [Range(1, 100)] private float rSpeed = 80;  //gradi al secondo
    [SerializeField] [Range(1, 150)] private float mouseSpeed = 100f; //muoversi ruotandosi o solo con la testiera o col mouse  
    [SerializeField] [Range(1, 10)] private float turboSpeed = 1.5f;
        
    [SerializeField] private bool useMouseLook = true;  //così ruotiamo col mouse
    [SerializeField] private CursorLockMode useLockState = CursorLockMode.Locked;
    //variabili per update
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

         if (useMouseLook) // use mouse horizontal rotation  
         {
             transform.Rotate(new Vector3(0, mouseSpeed * Time.deltaTime * _h , 0));
         }
         else //use A and D 
         {
             transform.Rotate(Vector3.up * (rSpeed * Time.deltaTime * _h * _turbo));
         }
    }
}
