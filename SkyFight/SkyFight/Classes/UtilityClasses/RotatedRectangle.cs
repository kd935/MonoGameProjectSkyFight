using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//Faster linq-style convenience functions https://github.com/jackmott/LinqFaster
using JM.LinqFaster;

namespace SkyFight
{
    // Original code by George W. Clingerman -> http://www.xnadevelopment.com/tutorials/rotatedrectanglecollisions/rotatedrectanglecollisions.shtml
    // Performance improvements Jack Mott
    public class RotatedRectangle
    {
        public Rectangle CollisionRectangle;
        public float Rotation;
        public Vector2 Origin;
        public SpriteType spriteType;

        public Vector2 aUpperLeft;
        public Vector2 aUpperRight;
        public Vector2 aLowerLeft;
        public Vector2 aLowerRight;
        public RotatedRectangle(Rectangle theRectangle, Vector2 theOrigin, float theInitialRotation)
        {
            CollisionRectangle = theRectangle;
            Rotation = theInitialRotation;

            //Calculate the Rectangles origin. We assume the center of the Rectangle will
            //be the point that we will be rotating around and we use that for the origin
            //Origin = new Vector2((int)theRectangle.Width / 2, (int)theRectangle.Height / 2);
            Origin = theOrigin;
        }

        /// <summary>
        /// Used for changing the X and Y position of the RotatedRectangle
        /// </summary>
        /// <param name="theXPositionAdjustment"></param>
        /// <param name="theYPositionAdjustment"></param>
        public void ChangePosition(int theXPositionAdjustment, int theYPositionAdjustment)
        {
            CollisionRectangle.X += theXPositionAdjustment;
            CollisionRectangle.Y += theYPositionAdjustment;
        }


        /// <summary>
        /// Check to see if two Rotated Rectangls have collided
        /// </summary>
        /// <param name="theRectangle"></param>
        /// <returns></returns>
        public bool Intersects(RotatedRectangle theRectangle)
        {
            //Calculate the Axis we will use to determine if a collision has occurred
            //Since the objects are rectangles, we only have to generate 4 Axis (2 for
            //each rectangle) since we know the other 2 on a rectangle are parallel.
            var aRectangleAxis = new Vector2[4];
            aRectangleAxis[0] = (UpperRightCorner() - UpperLeftCorner());
            aRectangleAxis[1] = (UpperRightCorner() - LowerRightCorner());
            aRectangleAxis[2] = (theRectangle.UpperLeftCorner() - theRectangle.LowerLeftCorner());
            aRectangleAxis[3] = (theRectangle.UpperLeftCorner() - theRectangle.UpperRightCorner());

            //Cycle through all of the Axis we need to check. If a collision does not occur
            //on ALL of the Axis, then a collision is NOT occurring. We can then exit out 
            //immediately and notify the calling function that no collision was detected. If
            //a collision DOES occur on ALL of the Axis, then there is a collision occurring
            //between the rotated rectangles. We know this to be true by the Seperating Axis Theorem
            foreach (Vector2 aAxis in aRectangleAxis)
            {
                if (!IsAxisCollision(theRectangle, aAxis))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a collision has occurred on an Axis of one of the
        /// planes parallel to the Rectangle
        /// </summary>
        /// <param name="theRectangle"></param>
        /// <param name="aAxis"></param>
        /// <returns></returns>
        private bool IsAxisCollision(RotatedRectangle theRectangle, Vector2 aAxis)
        {
            //Project the corners of the Rectangle we are checking on to the Axis and
            //get a scalar value of that project we can then use for comparison
            var aRectangleAScalars = new int[4];
            aRectangleAScalars[0] = (GenerateScalar(theRectangle.UpperLeftCorner(), aAxis));
            aRectangleAScalars[1] = (GenerateScalar(theRectangle.UpperRightCorner(), aAxis));
            aRectangleAScalars[2] = (GenerateScalar(theRectangle.LowerLeftCorner(), aAxis));
            aRectangleAScalars[3] = (GenerateScalar(theRectangle.LowerRightCorner(), aAxis));

            //Project the corners of the current Rectangle on to the Axis and
            //get a scalar value of that project we can then use for comparison
            var aRectangleBScalars = new int[4];
            aRectangleBScalars[0] = (GenerateScalar(UpperLeftCorner(), aAxis));
            aRectangleBScalars[1] = (GenerateScalar(UpperRightCorner(), aAxis));
            aRectangleBScalars[2] = (GenerateScalar(LowerLeftCorner(), aAxis));
            aRectangleBScalars[3] = (GenerateScalar(LowerRightCorner(), aAxis));

            //Get the Maximum and Minium Scalar values for each of the Rectangles
            int aRectangleAMinimum = aRectangleAScalars.MinF();
            int aRectangleAMaximum = aRectangleAScalars.MaxF();
            int aRectangleBMinimum = aRectangleBScalars.MinF();
            int aRectangleBMaximum = aRectangleBScalars.MaxF();

            //If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
            //then we are detecting a collision between the rectangles on this Axis
            if (aRectangleBMinimum <= aRectangleAMaximum && aRectangleBMaximum >= aRectangleAMaximum)
            {
                return true;
            }
            else if (aRectangleAMinimum <= aRectangleBMaximum && aRectangleAMaximum >= aRectangleBMaximum)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates a scalar value that can be used to compare where corners of 
        /// a rectangle have been projected onto a particular axis. 
        /// </summary>
        /// <param name="theRectangleCorner"></param>
        /// <param name="theAxis"></param>
        /// <returns></returns>
        private int GenerateScalar(Vector2 theRectangleCorner, Vector2 theAxis)
        {
            //Using the formula for Vector projection. Take the corner being passed in
            //and project it onto the given Axis
            float aNumerator = (theRectangleCorner.X * theAxis.X) + (theRectangleCorner.Y * theAxis.Y);
            float aDenominator = (theAxis.X * theAxis.X) + (theAxis.Y * theAxis.Y);
            float aDivisionResult = aNumerator / aDenominator;
            Vector2 aCornerProjected = new Vector2(aDivisionResult * theAxis.X, aDivisionResult * theAxis.Y);

            //Now that we have our projected Vector, calculate a scalar of that projection
            //that can be used to more easily do comparisons
            float aScalar = (theAxis.X * aCornerProjected.X) + (theAxis.Y * aCornerProjected.Y);
            return (int)aScalar;
        }

        /// <summary>
        /// Rotate a point from a given location and adjust using the Origin we
        /// are rotating around
        /// </summary>
        /// <param name="thePoint"></param>
        /// <param name="theOrigin"></param>
        /// <param name="theRotation"></param>
        /// <returns></returns>
        private Vector2 RotatePoint(Vector2 thePoint, Vector2 theOrigin, float theRotation)
        {
            return new Vector2(
                (float)(theOrigin.X + (thePoint.X - theOrigin.X) * Math.Cos(theRotation)
                - (thePoint.Y - theOrigin.Y) * Math.Sin(theRotation)),

                (float)(theOrigin.Y + (thePoint.Y - theOrigin.Y) * Math.Cos(theRotation)
                + (thePoint.X - theOrigin.X) * Math.Sin(theRotation))
                );
        }

        public Vector2 UpperLeftCorner()
        {
            aUpperLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Top);
            //aUpperLeft = RotatePoint(aUpperLeft, aUpperLeft + Origin, Rotation);
            return aUpperLeft;
        }

        public Vector2 UpperRightCorner()
        {
            aUpperRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Top);
            //aUpperRight = RotatePoint(aUpperRight, aUpperRight + new Vector2(-Origin.X, Origin.Y), Rotation);
            if (spriteType == SpriteType.LeftSidedOrigin)
            {
                aUpperRight.X = (float)(X + Math.Cos(Rotation) * CollisionRectangle.Width);
                aUpperRight.Y = (float)(Y - Math.Sin(Rotation) * CollisionRectangle.Width);
            }
            else
            {
                aUpperRight.X = (float)(X + Math.Cos(MathHelper.Pi + Rotation) * CollisionRectangle.Width);
                aUpperRight.Y = (float)(Y - Math.Sin(MathHelper.Pi + Rotation) * CollisionRectangle.Width);
            }

            return aUpperRight;
        }

        public Vector2 LowerLeftCorner()
        {
            aLowerLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Bottom);
            //aLowerLeft = RotatePoint(aLowerLeft, aLowerLeft + new Vector2(Origin.X, -Origin.Y), Rotation);
            if (spriteType == SpriteType.LeftSidedOrigin)
            {
                aLowerLeft.X = (float)(X + Math.Cos(MathHelper.Pi + MathHelper.PiOver2 + Rotation) * CollisionRectangle.Height);
                aLowerLeft.Y = (float)(Y - Math.Sin(MathHelper.Pi + MathHelper.PiOver2 + Rotation) * CollisionRectangle.Height);
            }
            else
            {
                aLowerLeft.X = (float)(X + Math.Cos(MathHelper.PiOver2 + Rotation) * CollisionRectangle.Height);
                aLowerLeft.Y = (float)(Y - Math.Sin(MathHelper.PiOver2 + Rotation) * CollisionRectangle.Height);
            }
            return aLowerLeft;
        }

        public Vector2 LowerRightCorner()
        {
            aLowerRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Bottom);
            //aLowerRight = RotatePoint(aLowerRight, aLowerRight + new Vector2(-Origin.X, -Origin.Y), Rotation);
           
            if (spriteType == SpriteType.LeftSidedOrigin)
            {
                aLowerRight.X = (float)(aLowerLeft.X + Math.Cos(Rotation) * CollisionRectangle.Width);
                aLowerRight.Y = (float)(aLowerLeft.Y - Math.Sin(Rotation) * CollisionRectangle.Width);
            }
            else
            {
                aLowerRight.X = (float)(aLowerLeft.X + Math.Cos(MathHelper.Pi + Rotation) * CollisionRectangle.Width);
                aLowerRight.Y = (float)(aLowerLeft.Y - Math.Sin(MathHelper.Pi + Rotation) * CollisionRectangle.Width);
            }

            return aLowerRight;
        }

        public int X
        {
            get { return CollisionRectangle.X; }
        }

        public int Y
        {
            get { return CollisionRectangle.Y; }
        }

        public int Width
        {
            get { return CollisionRectangle.Width; }
        }

        public int Height
        {
            get { return CollisionRectangle.Height; }
        }

    }


}