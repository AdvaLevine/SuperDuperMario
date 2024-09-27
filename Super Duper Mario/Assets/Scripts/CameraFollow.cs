
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

    void Update()
    {
        if(PlayerTransform != null)
        {
            float targetX = Mathf.Max(_cameraMinX, Mathf.Min(_cameraMaxX, PlayerTransform.position.x));
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.rotation = Quaternion.identity;
        }
    }
    
    public void CalculateBounds(int numberOfPlayers)
    {
        Camera myCamera = GetComponent<Camera>();
        _cameraHeight = 2f * myCamera.orthographicSize;
        
        if(numberOfPlayers>1)
        {
            _cameraWidth = _cameraHeight * (myCamera.aspect / 2f);
        }
        else{
            _cameraWidth = _cameraHeight * myCamera.aspect;
        }
        
        float leftBoundWidth = LeftBound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        float rightBoundWidth = RightBound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        
        _cameraMinX = LeftBound.position.x + leftBoundWidth + (_cameraWidth / 2f);
        _cameraMaxX = RightBound.position.x - rightBoundWidth - (_cameraWidth / 2f);
    }
}
