using UnityEngine;

public class FireBulletsAtTarget : MonoBehaviour
{
    
    [SerializeField] GameObject activationMark;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject head;
    [SerializeField] Transform[] firePositions;
    [SerializeField] [Range(1f, 10f)] private float minBulletSpeed;
    [SerializeField] [Range(5f, 20f)] private float maxBulletSpeed;
    [SerializeField] [Range(1f, 180f)] private float rotateSpeed = 60; 
    [SerializeField] [Range(-1f, 1f)] private float fireAngle = 0.8f;

    private Transform _target;

    private float _fireRate = 1;
    private float _fireDistance = 1;
    private float nextFire;
    private AudioSource _aso;

    [SerializeField] private bool debugDetectionAngle = false;
    [SerializeField] private bool debugArea = false;

    public void Configure(float fireRate, float fireDistance, Transform trackTarget)
    {
        _target = trackTarget;
        _fireRate = fireRate;
        _fireDistance = fireDistance;
    }
    
    private void Start()
    {
        _aso = GetComponentInParent<AudioSource>();
        activationMark.SetActive(false);
    }

// Update is called once per frame
// Frent function

    void Update()
    {
        if (!_target) return;

        if (Vector3.Distance(transform.position, _target.position) > _fireDistance)
        {
            head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, Quaternion.identity,
                rotateSpeed * Time.deltaTime);
            activationMark.SetActive(false);
            return;
        }

        activationMark.SetActive(true);

        //Vector3 directionStart = firePositions[0].position.normalized;
        Vector3 directionStart = head.transform.forward;
        Vector3 directionEnd = ( _target.transform.position - firePositions[0].transform.position ).normalized;

        float angle = Vector3.Angle(directionStart, directionEnd);

        float dot = Vector3.Dot(directionStart, directionEnd);

        if (debugDetectionAngle) Debug.LogWarning($"{gameObject.name}: angle: {angle} dot: {dot}", gameObject);

        var q = Quaternion.LookRotation(_target.position - head.transform.position);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, q, rotateSpeed * Time.deltaTime);
        
        //inverted cause i prefered to visualize the fire angle basing on the fact that dot = 1 mean that the tower is alligned on the target
        //so fireAngle become a index of proximity to the alligment on target if 1 we firePosition perfect alligned with target 
        //WARNING: -1 means will never shoot, and the firePosizion allignement could not be equal to tower alligment (better set fireAngle under 9.5)
        if (dot <= fireAngle) return; 
        nextFire += Time.deltaTime;

        if (nextFire >= _fireRate)
        {
            foreach (Transform firePosition in firePositions)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePosition.position, Quaternion.Euler(90, 0, 0));
                bullet.GetComponent<MoveBullet>().Configure(Random.Range(minBulletSpeed, maxBulletSpeed));
                bullet.transform.up = _target.position - firePosition.position;
            }

            _aso.Play();
            nextFire = 0f;
        }
    }

// Event Function
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !debugArea) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _fireDistance);
    }
}
