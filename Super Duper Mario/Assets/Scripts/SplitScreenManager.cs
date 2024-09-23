using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    public Camera cameraPlayer1;  // המצלמה של שחקן 1
    public Camera cameraPlayer2;  // המצלמה של שחקן 2

    public Transform leftBound;  // גבול שמאלי משותף
    public Transform rightBound; // גבול ימני משותף

    public Transform player1;  // השחקן הראשון
    public Transform player2;  // השחקן השני

    private CameraFollow cameraFollowPlayer1;
    private CameraFollow cameraFollowPlayer2;

    void Start()
    {
        // הגדרת ה-Viewport של מצלמה 1 (חצי שמאלי של המסך)
        cameraPlayer1.rect = new Rect(0, 0, 0.5f, 1);

        // הגדרת ה-Viewport של מצלמה 2 (חצי ימני של המסך)
        cameraPlayer2.rect = new Rect(0.5f, 0, 0.5f, 1);

        // קבלת הרכיב CameraFollow עבור כל מצלמה
        cameraFollowPlayer1 = cameraPlayer1.GetComponent<CameraFollow>();
        cameraFollowPlayer2 = cameraPlayer2.GetComponent<CameraFollow>();

        // הגדרת גבולות המצלמה
        cameraFollowPlayer1.LeftBound = leftBound;
        cameraFollowPlayer1.RightBound = rightBound;
        cameraFollowPlayer2.LeftBound = leftBound;
        cameraFollowPlayer2.RightBound = rightBound;

        // חישוב הגבולות
        cameraFollowPlayer1.CalculateBounds();
        cameraFollowPlayer2.CalculateBounds();

        // חיבור כל מצלמה לשחקן המתאים
        cameraFollowPlayer1.PlayerTransform = player1;
        cameraFollowPlayer2.PlayerTransform = player2;
    }
}