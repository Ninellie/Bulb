using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Shapes
{
    public class GameObjectPlacer : MonoBehaviour
    {
        [ContextMenuItem(nameof(AddCircle),nameof(AddCircle))]
        [ContextMenuItem(nameof(AddRectangle),nameof(AddRectangle))]
        [SerializeReference] private List<IShape> addShapes;
        
        [ContextMenuItem(nameof(CutCircle),nameof(CutCircle))]
        [ContextMenuItem(nameof(CutRectangle),nameof(CutRectangle))]
        [SerializeReference] private List<IShape> cutShapes;

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            foreach (var shape in addShapes)
            {
                switch (shape)
                {
                    case Circle circle:
                        Gizmos.DrawWireSphere(circle.Center, circle.Radius);
                        break;
                    case Rectangle rectangle:
                        Gizmos.DrawWireCube(rectangle.Rect.center, rectangle.Rect.size);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(shape));
                }
            }
            Gizmos.color = Color.red;
            foreach (var shape in cutShapes)
            {
                switch (shape)
                {
                    case Circle circle:
                        Gizmos.DrawWireSphere(circle.Center, circle.Radius);
                        break;
                    case Rectangle rectangle:
                        Gizmos.DrawWireCube(rectangle.Rect.center, rectangle.Rect.size);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(shape));
                }
            }
        }

        public void PlaceObject(Transform objectTransform)
        {
            // Получает случайную точку на фигуре c учётом вырезанных фигур
            var point = RandomPointGenerator.GetRandomPoint(addShapes, cutShapes, CheckMode.On);
            
            if (point == null)
            {
                return;
            }

            objectTransform.position = point.Value;
        }

        public void AddShape(IShape shape)
        {
            addShapes.Add(shape);
        }
        
        public void CutShape(IShape shape)
        {
            cutShapes.Add(shape);
        }
        
        private void AddCircle()
        {
            AddShape(new Circle(0, 0, 0));
        }
        
        private void CutCircle()
        {
            CutShape(new Circle(0, 0, 0));
        }
        
        private void AddRectangle()
        {
            AddShape(new Rectangle(new Rect()));
        }
        
        private void CutRectangle()
        {
            CutShape(new Rectangle(new Rect()));
        }
    }
}