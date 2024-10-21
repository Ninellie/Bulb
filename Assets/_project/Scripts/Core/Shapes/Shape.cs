using System;
using UnityEngine;

namespace Core.Shapes
{
    public interface IShape
    {
        bool Contains(Vector2 point);
    }

    [Serializable]
    public class Circle : IShape
    {
        [HideInInspector] public string name;
        
        [SerializeField] private float radius;
        [SerializeField] private Vector2 center;

        public Vector2 Center => center;
        public float Radius => radius;
        
        public Circle(float radius, Vector2 center)
        {
            this.radius = radius;
            this.center = center;
            name = nameof(Circle);
        }

        public virtual bool Contains(Vector2 point)
        {
            var xDistPow = Mathf.Pow(point.x - center.x, 2);
            var yDistPow = Mathf.Pow(point.y - center.y, 2);
            var radiusPow = Mathf.Pow(radius, 2);
            return xDistPow + yDistPow <= radiusPow;
        }
    }

    public class Circumference : Circle
    {
        public Circumference(float radius, Vector2 center) : base(radius, center)
        {
        }

        public override bool Contains(Vector2 point)
        {
            var xDistPow = Mathf.Pow(point.x - Center.x, 2);
            var yDistPow = Mathf.Pow(point.y - Center.y, 2);
            var radiusPow = Mathf.Pow(Radius, 2);
            return MathF.Abs(xDistPow + yDistPow - radiusPow) < 0.01f;
        }
    }

    [Serializable]
    public class Rectangle : IShape
    {
        [HideInInspector] public string name;
        
        [SerializeField] private Rect rect;
        
        public Rect Rect => rect;

        public Rectangle(Rect rect)
        {
            this.rect = rect;
            name = nameof(Rectangle);
        }

        public bool Contains(Vector2 point)
        {

            return point.x >= rect.xMin && point.x <= rect.xMax &&
                   point.y >= rect.yMin && point.y <= rect.yMax;
        }
    }
}