using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public Transform[] points;
    private int i;

    void Start()
    {
        transform.position = points[0].position;
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length) i = 0;
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.fixedDeltaTime);
    }
}