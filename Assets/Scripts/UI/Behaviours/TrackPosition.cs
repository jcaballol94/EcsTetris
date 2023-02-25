using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class TrackPosition : MonoBehaviour
    {
        public Transform position;

        private RectTransform myTrans;

        private void Awake()
        {
            myTrans = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (!position) return;

            var camera = Camera.main;
            var pos = camera.WorldToScreenPoint(position.position);

            myTrans.anchorMin = myTrans.anchorMax = Vector2.zero;
            myTrans.anchoredPosition = new Vector2(pos.x, pos.y);
        }
    }
}
