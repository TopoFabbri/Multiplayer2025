using System;

namespace CustomMath
{
    [Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        #region Variables

        public float x;
        public float y;
        public float z;

        public float sqrMagnitude
        {
            get { return x * x + y * y + z * z; }
        }

        public Vector3 normalized
        {
            get
            {
                float mag = this.magnitude;
                if (mag > 0)
                {
                    return new Vector3(x / mag, y / mag, z / mag);
                }

                return new Vector3(0, 0, 0);
            }
        }

        public float magnitude
        {
            get { return MathF.Sqrt(sqrMagnitude); }
        }

        #endregion

        #region constants

        public const float epsilon = 1e-05f;

        #endregion

        #region Default Values

        public static Vector3 Zero
        {
            get { return new Vector3(0.0f, 0.0f, 0.0f); }
        }

        public static Vector3 One
        {
            get { return new Vector3(1.0f, 1.0f, 1.0f); }
        }

        public static Vector3 Forward
        {
            get { return new Vector3(0.0f, 0.0f, 1.0f); }
        }

        public static Vector3 Back
        {
            get { return new Vector3(0.0f, 0.0f, -1.0f); }
        }

        public static Vector3 Right
        {
            get { return new Vector3(1.0f, 0.0f, 0.0f); }
        }

        public static Vector3 Left
        {
            get { return new Vector3(-1.0f, 0.0f, 0.0f); }
        }

        public static Vector3 Up
        {
            get { return new Vector3(0.0f, 1.0f, 0.0f); }
        }

        public static Vector3 Down
        {
            get { return new Vector3(0.0f, -1.0f, 0.0f); }
        }

        public static Vector3 PositiveInfinity
        {
            get { return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); }
        }

        public static Vector3 NegativeInfinity
        {
            get { return new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); }
        }

        #endregion

        #region Constructors

        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        #endregion

        #region Operators

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            float diffX = left.x - right.x;
            float diffY = left.y - right.y;
            float diffZ = left.z - right.z;
            
            float sqrmag = diffX * diffX + diffY * diffY + diffZ * diffZ;
            
            return sqrmag < epsilon * epsilon;
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !(left == right);
        }

        public static Vector3 operator +(Vector3 leftV3, Vector3 rightV3)
        {
            return new Vector3(leftV3.x + rightV3.x, leftV3.y + rightV3.y, leftV3.z + rightV3.z);
        }

        public static Vector3 operator -(Vector3 leftV3, Vector3 rightV3)
        {
            return new Vector3(leftV3.x - rightV3.x, leftV3.y - rightV3.y, leftV3.z - rightV3.z);
        }

        public static Vector3 operator -(Vector3 v3)
        {
            return new Vector3(-v3.x, -v3.y, -v3.z);
        }

        public static Vector3 operator *(Vector3 v3, float scalar)
        {
            return new Vector3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }

        public static Vector3 operator *(float scalar, Vector3 v3)
        {
            return new Vector3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }

        public static Vector3 operator /(Vector3 v3, float scalar)
        {
            return new Vector3(v3.x / scalar, v3.y / scalar, v3.z / scalar);
        }

        #endregion

        #region Functions

        public override string ToString()
        {
            return "X = " + x.ToString() + "   Y = " + y.ToString() + "   Z = " + z.ToString();
        }

        public static float Angle(Vector3 from, Vector3 to)
        {
            float dot = Dot(from.normalized, to.normalized);
            float angle = MathF.Acos(dot) * 360 / (MathF.PI * 2);
            return angle;
        }

        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            float sqrMagnitude = vector.sqrMagnitude;

            if (sqrMagnitude > maxLength * maxLength)
            {
                float magnitude = MathF.Sqrt(sqrMagnitude);
                vector *= maxLength / magnitude;
            }

            return vector;
        }

        public static float Magnitude(Vector3 vector)
        {
            return MathF.Sqrt(Dot(vector, vector));
        }

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            Vector3 cross;
            
            cross.x = a.y * b.z - a.z * b.y;
            cross.y = a.z * b.x - a.x * b.z;
            cross.z = a.x * b.y - a.y * b.x;

            return cross;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            return MathF.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y) + (b.z - a.z) * (b.z - a.z));
        }

        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            float newX = a.x > b.x ? a.x : b.x;
            float newY = a.y > b.y ? a.y : b.y;
            float newZ = a.z > b.z ? a.z : b.z;

            return new Vector3(newX, newY, newZ);
        }

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            float newX = a.x < b.x ? a.x : b.x;
            float newY = a.y < b.y ? a.y : b.y;
            float newZ = a.z < b.z ? a.z : b.z;

            return new Vector3(newX, newY, newZ);
        }

        public static float SqrMagnitude(Vector3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            float sqrMag = onNormal.sqrMagnitude;
            if (sqrMag < epsilon)
                return new Vector3(0f, 0f, 0f);
            else
                return onNormal * Dot(vector, onNormal) / sqrMag;
        }

        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            return inDirection - 2f * Dot(inDirection, inNormal) * inNormal;
        }

        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public void Scale(Vector3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        public static Vector3 Normalize(Vector3 vec)
        {
            float mag = vec.magnitude;
            if (mag > epsilon)
            {
                vec.x /= mag;
                vec.y /= mag;
                vec.z /= mag;
            }
            else
            {
                vec.x = 0f;
                vec.y = 0f;
                vec.z = 0f;
            }

            return vec;
        }
        
        public void Normalize()
        {
            float mag = magnitude;
            if (mag > epsilon)
            {
                x /= mag;
                y /= mag;
                z /= mag;
            }
            else
            {
                x = 0f;
                y = 0f;
                z = 0f;
            }
        }
        
        #endregion

        #region Internals

        public override bool Equals(object other)
        {
            if (!(other is Vector3)) return false;
            return Equals((Vector3)other);
        }

        public bool Equals(Vector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }

        #endregion
    }
}