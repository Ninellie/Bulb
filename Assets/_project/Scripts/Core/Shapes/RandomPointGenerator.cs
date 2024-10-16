using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Shapes
{
    public enum CheckMode
    {
        On,
        Inside,
        InsideOn,
        Outside,
    }

    public static class RandomPointGenerator
    {
        public static Vector2? GetRandomPoint(List<IShape> addShapes, List<IShape> cutShapes, CheckMode checkMode)
        {
            if (addShapes.Count == 0)
            {
                return null;
            }

            while (true)
            {
                var selectedShape = addShapes[Random.Range(0, addShapes.Count)];

                var randomPoint = checkMode switch
                {
                    CheckMode.On => GetRandomPointOnShape2D(selectedShape),
                    CheckMode.Inside => GetRandomPointInsideShape2D(selectedShape),
                    CheckMode.InsideOn => throw new NotImplementedException(),
                    CheckMode.Outside => throw new NotImplementedException(),
                    _ => throw new ArgumentOutOfRangeException(nameof(checkMode), checkMode, null)
                };

                var isInsideCutShape = cutShapes.Any(cutShape => cutShape.Contains(randomPoint));

                if (!isInsideCutShape)
                {
                    return randomPoint;
                }
            }
        }

        private enum RectSide
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3
        }

        private static Vector2 GetRandomPointOnShape2D(IShape shape)
        {
            switch (shape)
            {
                case Circle circle:
                    return (Vector2)Random.onUnitSphere * circle.Radius + circle.Center;
                case Rectangle rectangle:
                    float x;
                    float y;
                    var side = (RectSide)Random.Range(0, 4);
                    switch (side)
                    {
                        case RectSide.Left:
                            x = rectangle.Rect.xMin;
                            y = Random.Range(rectangle.Rect.yMin, rectangle.Rect.yMax);
                            break;
                        case RectSide.Right:
                            x = rectangle.Rect.xMax;
                            y = Random.Range(rectangle.Rect.yMin, rectangle.Rect.yMax);
                            break;
                        case RectSide.Top:
                            x = Random.Range(rectangle.Rect.xMin, rectangle.Rect.xMax);
                            y = rectangle.Rect.yMax;
                            break;
                        case RectSide.Bottom:
                            x = Random.Range(rectangle.Rect.xMin, rectangle.Rect.xMax);
                            y = rectangle.Rect.yMin;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return new Vector2(x, y);
                default:
                    throw new ArgumentException("Unknown shape type");
            }
        }

        private static Vector2 GetRandomPointInsideShape2D(IShape shape)
        {
            switch (shape)
            {
                case Circle circle:
                    return Random.insideUnitCircle * circle.Radius + circle.Center;
                case Rectangle rectangle:
                {
                    var x = Random.Range(rectangle.Rect.xMin, rectangle.Rect.xMax);
                    var y = Random.Range(rectangle.Rect.yMin, rectangle.Rect.yMax);
                    return new Vector2(x, y);
                }
                default:
                    throw new ArgumentException("Unknown shape type");
            }

        }
    }
}