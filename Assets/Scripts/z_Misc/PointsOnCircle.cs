using UnityEngine;
using System;

public static class PointsOnCircle
{
    private enum Direction {Undefined, Clockwise, Counterclockwise}
    private static readonly float twopi = 2*Mathf.PI;

    // private void Repaint()
    // {
    //     FindCircleFromPoints(anchorL.position, anchorC.position, anchorR.position, out circleCenter, out circleRadius);

    //     for (int i = 0; i < numberOfPoints; i++)
    //     {
    //         GameObject point = points[i];
    //         GetArcPosition(i, out Vector3 arcPosition, out Quaternion arcRotation);
    //         point.transform.position = circleCenter + arcPosition;
    //         point.transform.localRotation = arcRotation;
    //     }
    // }

    public static void GetArcPosition(Vector3 anchorL, Vector3 anchorC, Vector3 anchorR, 
                                      int i, int numberOfPoints, float usableRange,
                                      out Vector3 position, out Quaternion rotation)
    {
        // Complicated math. When called with different 0-1 values of progress,
        // returns a set of points that span arcSize portion of a circle. 
        //
        // The arc is bounded by anchorL and anchorR, and passes through 
        // anchorC. The range is further constrained by the 0-1 proportion
        // usableRange. The set of points will always be centered halfway
        // through the arc.
        // 
        // Derived from:
        // https://stackoverflow.com/a/16544330
        // ================

        float lerpIndex;

        if (numberOfPoints == 1) {
            lerpIndex = 0.5f;
        } else {
            lerpIndex = i/(float)(numberOfPoints-1);
        }

        GetArcPosition(anchorL, anchorC, anchorR, 
                       lerpIndex, usableRange,
                       out position, out rotation);
    }

    public static void GetArcPosition(Vector3 anchorL, Vector3 anchorC, Vector3 anchorR, 
                                      float progress, float usableRange,
                                      out Vector3 position, out Quaternion rotation)
    {
        // Complicated math. When called with different 0-1 values of progress,
        // returns a set of points that span arcSize portion of a circle. 
        //
        // The arc is bounded by anchorL and anchorR, and passes through 
        // anchorC. The range is further constrained by the 0-1 proportion
        // usableRange. The set of points will always be centered halfway
        // through the arc.
        // 
        // Derived from:
        // https://stackoverflow.com/a/16544330
        // ================

        // Finding the Circle ===============
        
        bool circleFound = FindCircleFromPoints(anchorL, anchorC, anchorR, 
                                                out Vector3 circleCenter, out float circleRadius);
        if (!circleFound) {
            // If circle is not found, all anchors are colinear.
            Debug.LogError("PointsOnCircle Error: GetArcPosition failed. All anchors were colinear.");
            position = new();
            rotation = new();
            return;

            // TODO: Add support for colinear arcs, where we just span a line.
        }



        // Arc Distance ===============

        // The amount, from 0-1, we need to shift forward along our arc to have our cards
        // be centered after accounting for usableRange.
        double usableRange_arcOffset = 0.5f*(1-usableRange);
        // How far, from 0-1, this card should be placed on our arc. 
        double distanceOnArc = usableRange*progress + usableRange_arcOffset;



        // Anchors and Direction ======

        Vector3 L = anchorL-circleCenter, 
                C = anchorC-circleCenter, 
                R = anchorR-circleCenter;
        Vector3 right = new Vector3(Mathf.Cos(0), Mathf.Sin(0)) * circleRadius;
        
        float angleLeft = (float) Math.Atan2((right.x*L.y)-(right.y*L.x), Vector3.Dot(right,L));
        float angleCenter = (float) Math.Atan2((right.x*C.y)-(right.y*C.x), Vector3.Dot(right,C));
        float angleRight = (float) Math.Atan2((right.x*R.y)-(right.y*R.x), Vector3.Dot(right,R));
        // Normalize our vectors.
        angleLeft = (angleLeft < 0) ? (angleLeft+twopi)/twopi : angleLeft/twopi;
        angleCenter = (angleCenter < 0) ? (angleCenter+twopi)/twopi : angleCenter/twopi;     
        angleRight = (angleRight < 0) ? (angleRight+twopi)/twopi : angleRight/twopi;    

        Direction direction = GetDirectionToCenter(angleLeft, angleCenter, angleRight);

        switch (direction) {
            case Direction.Clockwise: {
                // Radians normally increase counterclockwise, so we need to 
                // invert our distance if we go the other way.
                distanceOnArc*=-1;
                break;
            }
            case Direction.Counterclockwise: {
                break;
            }   
            case Direction.Undefined: {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return;
            }          
        }

        float arcSize = CalculateArcSize(angleLeft, angleRight, direction);
        Debug.Assert(arcSize>=0);



        // Final calculations =========

        // Our target angle is the result of the linear equation y = mx + b, where:
        //      y is our target angle
        //      m is our arcSize
        //      x is our distance along the arc
        //      b is our intercept angle, namely angleLeft
        // In order to convert this angle from a proportion to radians, we must also
        // multiply by 2pi.

        double radians = twopi * (arcSize*distanceOnArc + angleLeft);

        position = new Vector3(Mathf.Cos((float)radians),Mathf.Sin((float)radians)) * circleRadius;
        position += circleCenter;

        float degrees = (float)(radians*180/Mathf.PI);
        rotation = Quaternion.Euler(0,0,degrees-90);  // Minus 90 to face downwards instead of rightwards by default.
    }

    private static Direction GetDirectionToCenter(float L, float C, float R)
    {
        // Returns the direction to travel in order to pass through L->C->R.
        // We 'add' to L (rotate counterclockwise) until we hit another point. 
        // If the first point we hit is C, then counterclockwise was the correct direction.
        // If the first point we hit is R, then clockwise was the correct direction.
        
        // Case 1: Both C and R are above us.
        if (C>L && R>L) {
            // Return counterclockwise if C is lower (closer), and clockwise if R is lower (closer).
            return (C < R) ? Direction.Counterclockwise : Direction.Clockwise;
        }
        // Case 2: C is above us and R is below us.
        if (C>L && R<L) {
            return Direction.Counterclockwise;
        }
        // Case 3: C is below us and R is above us.
        if (C<L && R>L) {
            return Direction.Clockwise;
        }
        // Case 4: Both C and R are below us.
        if (C<L && R<L) {
            // Return counterclockwise if C is closer to 0, and clockwise if R is closer to 0.
            return (C < R) ? Direction.Counterclockwise : Direction.Clockwise;
        }

        // Fallback, true if C or R is coincident with L.
        else {
            Debug.LogWarning("PointOnCircle Warning: GetDirectionToCenter error. Two or more of the points are coincident.");
            return Direction.Undefined;
        }
    }

    private static float CalculateArcSize(float L, float R, Direction direction)
    {
        if (direction == Direction.Clockwise) {
            if (R>L) { // If R has wrapped past 0...
                // Return L's clockwise distance to 0 + R's clockwise distance from 1.
                return L + 1-R;
            }
            if (R<L) { // If R is closer to 0...
                return L - R;
            }
        } else if (direction == Direction.Counterclockwise) {
            if (R<L) { // If R has wrapped past 1...
                // Return L's counterclockwise distance to 1 + R's counterclockwise distance from 0.
                return 1-L + R;
            }
            if (R>L) { // If R is closer to 1...
                return R - L;
            }
        }
        
        return 0; // Undefined direction, no distance
    }

    public static bool FindCircleFromPoints(Vector3 pointA, Vector3 pointB, Vector3 pointC,
                                             out Vector3 center, out float radius)
    {
        // Algebraic formula which returns out the center and radius of the unique 
        // circle which passes through pointA, pointB, and pointC.
        // Returns true if such a circle exists, and false if not.
        //
        // Formula adapted from:
        // https://stackoverflow.com/a/50974391
        
        double temp = (pointB.x*pointB.x) + (pointB.y*pointB.y);
        double bc = ((pointA.x*pointA.x) + (pointA.y*pointA.y) - temp)/2f;
        double cd = (temp - (pointC.x*pointC.x) - (pointC.y*pointC.y))/2f;
        double det = ((pointA.x-pointB.x)*(pointB.y-pointC.y)) - ((pointB.x-pointC.x)*(pointA.y-pointB.y));

        if (Math.Abs(det) < double.Epsilon) {
            // Points are colinear, no center or radius exists.
            center = new(0,0);
            radius = -1;
            return false;
        }
        
        // Center of circle
        double centerX = ((bc*(pointB.y-pointC.y)) - (cd*(pointA.y-pointB.y))) / det;
        double centerY = ((cd*(pointA.x-pointB.x)) - (bc*(pointB.x-pointC.x))) / det;
        
        center = new((float)centerX,(float)centerY,0);
        radius = Vector3.Distance(center, pointA);
        return true;
    }
}
