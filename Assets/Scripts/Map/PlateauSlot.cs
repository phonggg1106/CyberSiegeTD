using UnityEngine;
using AnutoTD.Towers;

namespace AnutoTD.Map
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlateauSlot : MonoBehaviour
    {
        [Header("Trạng thái")]
        public bool isOccupied = false;
        public GameObject currentTower = null;

        private void Awake()
        {
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null) col.isTrigger = true;
        }

        public void PlaceTower(GameObject tower)
        {
            currentTower = tower;
            isOccupied = true;
            tower.transform.position = transform.position;

            // Báo cho tháp biết nó đang đứng trên bệ này
            TowerController tc = tower.GetComponent<TowerController>();
            if (tc != null) tc.SetPlateau(this);
        }

        public void ClearTower()
        {
            currentTower = null;
            isOccupied = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isOccupied ? Color.red : Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
        }
    }
}