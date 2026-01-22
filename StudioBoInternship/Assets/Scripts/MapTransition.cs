using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D mapBoundary;
    CinemachineConfiner2D confiner;
    [SerializeField] Direction direction;
    [SerializeField] private float additvePos = 3;
    
    enum Direction { Up, Down, Left, Right }

    private void Awake()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            confiner.BoundingShape2D = mapBoundary;
            UpdatePlayerPosition(collision.gameObject);
            
            MapControllerManual.Instance?.HighlightArea(mapBoundary.name);
        }
    }
    
    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPosition = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPosition.y += additvePos;
                break;
            case Direction.Down:
                newPosition.y -= additvePos;
                break;
            case Direction.Left:
                newPosition.x -= additvePos;
                break;
            case Direction.Right:
                newPosition.x += additvePos;
                break;
        }
        
        player.transform.position = newPosition;
    }
}
