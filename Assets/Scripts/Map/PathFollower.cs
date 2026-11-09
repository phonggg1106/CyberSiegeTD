using System.Collections.Generic;
using UnityEngine;
using AnutoTD.Map;

namespace AnutoTD.Enemies
{
    public class PathFollower : MonoBehaviour
    {
        private List<Transform> waypoints;
        private int currentWaypointIndex = 0;
        private float currentSpeed;
        private bool isMoving = false;

        public System.Action OnReachGoal; // Báo hiệu khi quái chạy tới nhà chính

        public void Initialize(List<Transform> pathWaypoints, float baseSpeed)
        {
            waypoints = pathWaypoints;
            currentSpeed = baseSpeed;
            currentWaypointIndex = 0;

            if (waypoints != null && waypoints.Count > 0)
            {
                transform.position = waypoints[0].position;
                isMoving = true;
            }
        }

        public void SetSpeed(float speed)
        {
            currentSpeed = speed;
        }

        private void Update()
        {
            if (!isMoving || waypoints == null || currentWaypointIndex >= waypoints.Count) return;

            Transform targetPoint = waypoints[currentWaypointIndex];
            Vector3 direction = (targetPoint.position - transform.position).normalized;

            // Di chuyển hướng về waypoint hiện tại
            transform.position += direction * currentSpeed * Time.deltaTime;

            // Xoay mặt theo hướng di chuyển (hoặc lật Sprite)
            if (direction.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
            }

            // Kiểm tra đã đến gần waypoint chưa (khoảng cách < 0.1)
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    isMoving = false;
                    OnReachGoal?.Invoke(); // Đã lọt vào đích!
                }
            }
        }
    }
}