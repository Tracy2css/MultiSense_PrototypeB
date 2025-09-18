using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public static class Util
    {
        /// <summary>
        /// Convert a RenderTexture into a Texture2D.
        /// </summary>
        public static Texture2D ToTexture2D(RenderTexture renderTexture)
        {
            var tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Convert a Texture2D into a sprite.
        /// </summary>
        public static Sprite ToSprite(Texture2D texture)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f),
                100);
            return sprite;
        }
        
        /// <summary>
        /// Set the pivot of a RectTransform.
        /// </summary>
        public static void SetPivot(RectTransform rectTransform, Vector2 pivot)
        {
            if (rectTransform == null) return;
 
            var size = rectTransform.rect.size;
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x * rectTransform.lossyScale.x, deltaPivot.y * size.y * rectTransform.lossyScale.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
        
        // UI OFFSET FUNCTIONS ---------
        
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }
 
        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }
 
        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }
 
        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static void SetAllOffsets(this RectTransform rt, float offset)
        {
            rt.offsetMax = new Vector2(-offset, -offset);
            rt.offsetMin = new Vector2(offset, offset);
        }
        
        // ----------
        
        // UI SMOOTH TRANSITIONS ----------
        
        /// <summary>
        /// Smoothly scale a transform.
        /// </summary>
        /// <param name="transform">The transform to scale.</param>
        /// <param name="scale">The desired final scale.</param>
        /// <param name="speed">The speed of scaling (2 recommended).</param>
        public static void SmoothScale(this Transform transform, Vector3 scale, float speed = 2)
        {
            if (transform.localScale == scale) return;
            var adjustedScale = Vector3.MoveTowards(transform.localScale, scale, Time.deltaTime * speed);
            transform.localScale = adjustedScale;
        }

        /// <summary>
        /// Smooth alpha transition for image.
        /// </summary>
        /// <param name="image">The image to do the transition on.</param>
        /// <param name="alpha">Ranges from 0 (Transparent) to 1 (Opaque).</param>
        /// <param name="speed">Speed of transition (2 recommended).</param>
        public static void SmoothAlpha(this Image image, float alpha, float speed = 2)
        {
            var clampedAlpha = Mathf.Clamp(alpha, 0, 1);
            // If alpha is already at the desired level
            if (Math.Abs(image.color.a - clampedAlpha) < 0.0001f) return;
            // If not, move towards desired alpha level
            var adjustedAlpha = Mathf.MoveTowards(image.color.a, alpha, Time.deltaTime * speed);
            var newColor = image.color;
            newColor.a = adjustedAlpha;
            image.color = newColor;

        }

        /// <summary>
        /// Smoothly move an transform over time.
        /// </summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="position">The desired final position.</param>
        /// <param name="speed">The speed of movement (2 recommended).</param>
        public static void SmoothMovement(this Transform transform, Vector3 position, float speed = 2)
        {
            if (transform.localPosition == position) return;
            var newPosition = Vector3.MoveTowards(transform.localPosition, position, Time.deltaTime * speed * 1000);
            transform.localPosition = newPosition;
        }
        
        /// <summary>
        /// Smoothly move an transform over time.
        /// </summary>
        /// <param name="rt">The RectTransform to scale width of.</param>
        /// <param name="width">The desired final width.</param>
        /// <param name="speed">The speed of scaling (3 recommended).</param>
        public static void SmoothWidth(this RectTransform rt, float width, float speed = 3)
        {
            if (Math.Abs(rt.rect.width - width) < 0.0001f) return;
            var newWidth = Mathf.MoveTowards(rt.rect.width, width, Time.deltaTime * speed * 1000);
            rt.sizeDelta = new Vector2(newWidth, rt.sizeDelta.y);
        }
        
        // ----------

        // Set the alpha of an image.
        public static void SetAlpha(this Image image, float alpha)
        {
            var colorCopy = image.color;
            colorCopy.a = alpha;
            image.color = colorCopy;
        }
        
        // Set the width of an RectTransform.
        public static void SetWidth(this RectTransform rt, float width)
        {
            var newSize = rt.sizeDelta;
            newSize.x = width;
            rt.sizeDelta = newSize;
        }
        
        // Set X local position of transform.
        public static void SetLocalX(this Transform transform, float newX, bool addToOriginal = false)
        {
            var localPositionCopy = transform.localPosition;
            localPositionCopy.x = addToOriginal ? newX + localPositionCopy.x : newX;
            transform.localPosition = localPositionCopy;
        }
        
        // Set Y position of transform.
        public static void SetY(this Transform transform, float newY, bool addToOriginal = false)
        {
            var positionCopy = transform.position;
            positionCopy.y = addToOriginal ? newY + positionCopy.y : newY;
            transform.position = positionCopy;
        }
        
        // COROUTINES ----------
        /// <summary>
        /// Transition an Image's alpha smoothly, speed from 1-2 is the usual.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="newAlpha"></param>
        /// <param name="speed"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator TransitionAlpha(this Image image, float newAlpha, int speed, Action callback)
        {
            while (Math.Abs(image.color.a - newAlpha) > 0.0001f)
            {
                var a = Mathf.MoveTowards(image.color.a, newAlpha, Time.deltaTime * speed);
                var color = image.color;
                color.a = a;
                image.color = color;

                yield return null;
            }
            callback?.Invoke();
        }
        
        /// <summary>
        /// Transition a Canvas Group's alpha smoothly, speed from 1-2 is the usual.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="newAlpha"></param>
        /// <param name="speed"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator TransitionAlpha(this CanvasGroup group, float newAlpha, int speed, Action callback)
        {
            while (Math.Abs(group.alpha - newAlpha) > 0.0001f)
            {
                var a = Mathf.MoveTowards(group.alpha, newAlpha, Time.deltaTime * speed);
                group.alpha = a;

                yield return null;
            }
            callback?.Invoke();
        }
        
        // ----------
        
        // COLOR UTILITIES ----------

        /// <summary>
        /// Hex string to Color.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex, float alpha = 1)
        {
            ColorUtility.TryParseHtmlString(hex, out var color);
            color.a = alpha;
            return color;
        } 
        
        // ----------
    }
}
