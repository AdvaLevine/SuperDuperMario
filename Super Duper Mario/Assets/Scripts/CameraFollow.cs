
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

    void Update()
    {
        if(PlayerTransform != null)
        {
            // Clamp the camera's x position to be within the bounds
            float targetX = Mathf.Max(_cameraMinX, Mathf.Min(_cameraMaxX, PlayerTransform.position.x));
            // Smoothly move the camera to the target x position
            //float x = Mathf.SmoothDamp(transform.position.x, targetX, ref _smoothDampVelocity.x, _smoothDampTime);
            // Set the camera's position to the new x position
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            // שמירה על הסיבוב של המצלמה קבוע
            transform.rotation = Quaternion.identity;
        }
    }
    


    public void CalculateBounds()
    {
        Camera myCamera = GetComponent<Camera>();
        _cameraHeight = 2f * myCamera.orthographicSize;
        _cameraWidth = _cameraHeight * (myCamera.aspect / 2f);
        
        float leftBoundWidth = LeftBound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        float rightBoundWidth = RightBound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        
        _cameraMinX = LeftBound.position.x + leftBoundWidth + (_cameraWidth / 2f);
        _cameraMaxX = RightBound.position.x - rightBoundWidth - (_cameraWidth / 2f);
    }
}
