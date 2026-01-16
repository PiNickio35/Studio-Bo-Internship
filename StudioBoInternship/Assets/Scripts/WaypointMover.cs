using System;
using System.Collections;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWayPoints = true;

    private Transform[] _waypoints;
    private int _currentWaypointIndex;
    private bool _isWaiting;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _waypoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            _waypoints[i] = waypointParent.GetChild(i);
        }
    }

    private void Update()
    {
        if (PauseController.IsGamePaused || _isWaiting)
        {
            _animator.SetBool("isWalking", false);
            return;
        }
        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Transform target = _waypoints[_currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        _animator.SetFloat("InputX", direction.x);
        _animator.SetFloat("InputY", direction.y);
        _animator.SetBool("isWalking", direction.magnitude > 0);
        if (Vector2.Distance(transform.position, target.position) <= 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        _isWaiting = true;
        _animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitTime);

        _currentWaypointIndex = loopWayPoints
            ? (_currentWaypointIndex + 1) % _waypoints.Length
            : Mathf.Min(_currentWaypointIndex + 1, _waypoints.Length - 1);

        _isWaiting = false;
    }
}
