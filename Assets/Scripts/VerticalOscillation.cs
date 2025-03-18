using UnityEngine;

public class VerticalOscillation : MonoBehaviour
{
    [SerializeField] private float verticalOscillationSpeed = 1.1f;
    [SerializeField] private float rotateSpeed = 30;
    
    float minY = 0.1f;   // Lowest position
    float maxY = 1.0f;   // Uppest position
    float startYPosition; //to add it on oscillation fuction 
    void Start()
    {
        startYPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            startYPosition+ Mathf.Lerp(minY, maxY, Mathf.PingPong(Time.time * verticalOscillationSpeed, 1)),
            transform.position.z);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
