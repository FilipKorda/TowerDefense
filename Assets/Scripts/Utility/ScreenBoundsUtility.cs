using UnityEngine;

namespace TowerDefense.UI
{
   public static class ScreenBoundsUtility
    {
        public static Rect GetScreenRect(RectTransform rect, Canvas canvas)
        {
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);

            Camera screenCamera = GetScreenCamera(canvas);

            Vector2 min = RectTransformUtility.WorldToScreenPoint(screenCamera, corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(screenCamera, corners[2]);

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        public static Vector2 ClampToScreen(RectTransform panel, Canvas canvas, Vector2 desiredScreenPosition)
        {
            Vector2 size = GetScreenSize(panel, canvas);
            Vector2 pivot = panel.pivot;

            float minX = size.x * pivot.x;
            float maxX = Screen.width - size.x * (1f - pivot.x);
            float minY = size.y * pivot.y;
            float maxY = Screen.height - size.y * (1f - pivot.y);

            float clampedX = minX <= maxX ? Mathf.Clamp(desiredScreenPosition.x, minX, maxX) : (minX + maxX) * 0.5f;
            float clampedY = minY <= maxY ? Mathf.Clamp(desiredScreenPosition.y, minY, maxY) : (minY + maxY) * 0.5f;

            return new Vector2(clampedX, clampedY);
        }

        public static void SetScreenPosition(RectTransform panel, Canvas canvas, Vector2 screenPosition)
        {
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                panel.position = screenPosition;
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvas.worldCamera,
                out Vector2 localPoint);

            panel.anchoredPosition = localPoint;
        }

        public static void PositionNextToAnchor(
            RectTransform panel,
            RectTransform anchor,
            Canvas canvas,
            float spacing = 10f)
        {
            Rect anchorScreenRect = GetScreenRect(anchor, canvas);
            Vector2 panelSize = GetScreenSize(panel, canvas);

            float spaceRight = Screen.width - anchorScreenRect.xMax;
            float spaceLeft = anchorScreenRect.xMin;
            float spaceTop = Screen.height - anchorScreenRect.yMax;
            float spaceBottom = anchorScreenRect.yMin;

            float bestSpace = spaceRight;
            int bestSide = 0; 

            if (spaceLeft > bestSpace) { bestSpace = spaceLeft; bestSide = 1; }
            if (spaceTop > bestSpace) { bestSpace = spaceTop; bestSide = 2; }
            if (spaceBottom > bestSpace) { bestSpace = spaceBottom; bestSide = 3; }

            Vector2 anchorCenter = new Vector2(anchorScreenRect.center.x, anchorScreenRect.center.y);
            Vector2 desiredPosition;

            switch (bestSide)
            {
                case 1: 
                    desiredPosition = new Vector2(anchorScreenRect.xMin - spacing - panelSize.x * (1f - panel.pivot.x), anchorCenter.y);
                    break;
                case 2: 
                    desiredPosition = new Vector2(anchorCenter.x, anchorScreenRect.yMax + spacing + panelSize.y * panel.pivot.y);
                    break;
                case 3: 
                    desiredPosition = new Vector2(anchorCenter.x, anchorScreenRect.yMin - spacing - panelSize.y * (1f - panel.pivot.y));
                    break;
                default: 
                    desiredPosition = new Vector2(anchorScreenRect.xMax + spacing + panelSize.x * panel.pivot.x, anchorCenter.y);
                    break;
            }

            Vector2 clampedPosition = ClampToScreen(panel, canvas, desiredPosition);
            SetScreenPosition(panel, canvas, clampedPosition);
        }

        private static Vector2 GetScreenSize(RectTransform rect, Canvas canvas)
        {
            float scaleFactor = canvas != null ? canvas.scaleFactor : 1f;
            return rect.rect.size * scaleFactor;
        }

        private static Camera GetScreenCamera(Canvas canvas)
        {
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }

            return canvas.worldCamera;
        }
    }
}