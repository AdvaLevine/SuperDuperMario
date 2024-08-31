
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform LeftBound { set; get;}
    public Transform RightBound { set; get;}
    public Transform PlayerTransform { set; private get; }
    private float _cameraWidth;
    private float _cameraHeight;
    private float _cameraMinX;
    private float _cameraMaxX;
    private float _smoothDampTime = 0.15f;
    private Vector3 _smoothDampVelocity = Vector3.zero;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraHeight = 2f * Camera.main.orthographicSize;
        _cameraWidth = _cameraHeight * Camera.main.aspect;
        
        float leftBoundWidth = LeftBound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        float rightBoundWidth = RightBound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        
        _cameraMinX = LeftBound.position.x + leftBoundWidth + (_cameraWidth / 2);
        _cameraMaxX = RightBound.position.x - rightBoundWidth - (_cameraWidth / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerTransform != null)
        {
            // Clamp the camera's x position to be within the bounds
            float targetX = Mathf.Max(_cameraMinX, Mathf.Min(_cameraMaxX, PlayerTransform.position.x));
            // Smoothly move the camera to the target x position
            float x = Mathf.SmoothDamp(transform.position.x, targetX, ref _smoothDampVelocity.x, _smoothDampTime);
            // Set the camera's position to the new x position
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
