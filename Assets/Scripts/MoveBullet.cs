using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    private float _speed = 1;

    public void Configure(float speed)
    {
        _speed = speed;
        Destroy(gameObject, 14);
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * (_speed * Time.deltaTime), Space.Self);
    }
}
