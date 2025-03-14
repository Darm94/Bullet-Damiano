using UnityEngine;

public class DestroyBulletOnCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void onCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<FireBulletsAtTarget>()) return;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
