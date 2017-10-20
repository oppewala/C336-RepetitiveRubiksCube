using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace C336_RepetitiveRubiksCube
{
    class Program
    {
        static void Main(string[] args)
        {
            Cube initialCube = new Cube();
            Cube cube = new Cube();

            int moves = 0;
            do { 
                cube.RotateSideClockwise(cube.Front, cube.Upper, cube.Bottom, cube.Left, cube.Right);

                moves++;
                Console.WriteLine(moves);
            } while (!cube.Equals(initialCube) && moves < 100);

            Console.Read();
        }
    }

    public class Cube
    {
        public Cube()
        {
            // Needs to pass in reference to the specific set of sqaures of each side
            // since the orientation of the side will determine which row/column to use
            // when rotating.
            Upper = new Face(Colours.Yellow, Back, Front, Left, Right);
            Bottom = new Face(Colours.Blue, Front, Back, Left, Right);
            Left = new Face(Colours.Green, Upper, Bottom, Back, Front);
            Right = new Face(Colours.White, Upper, Bottom, Front, Back);
            Front = new Face(Colours.Orange, Upper, Bottom, Left, Right);
            Back = new Face(Colours.Red, Upper, Bottom, Right, Left);
        }

        public Face Upper { get; set; }
        public Face Bottom { get; set; }
        public Face Left { get; set; }
        public Face Right { get; set; }
        public Face Front { get; set; }
        public Face Back { get; set; }

        public bool Equals(Cube comparison)
        {
            // Reflection?
            if (!this.Upper.Equals(comparison.Upper))
                return false;

            if (!this.Bottom.Equals(comparison.Bottom))
                return false;

            if (!this.Left.Equals(comparison.Left))
                return false;

            if (!this.Right.Equals(comparison.Right))
                return false;
            
            if (!this.Front.Equals(comparison.Front))
                return false;

            if (!this.Back.Equals(comparison.Back))
                return false;

            return true;
        }

        public void RotateSideClockwise(Face side, Face above, Face below, Face left, Face right)
        {
            Face initialAbove = Helpers.DeepCopy(above);

            side.RotateClockwise();
            above.TwistRow(2, left.GetColumn(2));
            left.TwistColumn(2, below.GetRow(0));
            below.TwistRow(0, right.GetColumn(0));
            right.TwistColumn(0, initialAbove.GetRow(2));
        }
    }

    [Serializable]
    public class Face
    {
        public Face(Colours colour, Face above, Face below, Face left, Face right)
        {
            Positions = new Colours[3, 3] {
                {colour, colour, colour}, 
                {colour, colour, colour},
                {colour, colour, colour}
            };
        }

        public Colours[,] Positions { get; set; }
        private Face Above { get; }
        private Face Below { get; }
        private Face Left { get; }
        private Face Right { get; }

        public Colours[] GetRow(int row)
        {
            Colours[] colours = new Colours[]{ Positions[row, 0], Positions[row, 1], Positions[row, 2] };

            return colours;
        }
        public Colours[] GetColumn(int column)
        {
            Colours[] colours = new Colours[]{ Positions[0, column], Positions[1, column], Positions[2, column] };

            return colours;
        }

        public void TwistRow(int row, Colours[] newRow)
        {
            Positions[row, 0] = newRow[0];
            Positions[row, 1] = newRow[1];
            Positions[row, 2] = newRow[2];
        }
        public void TwistColumn(int column, Colours[] newColumn)
        {
            Positions[0, column] = newColumn[0];
            Positions[1, column] = newColumn[1];
            Positions[2, column] = newColumn[2];
        }

        public void RotateClockwise()
        {
            var InitialPositions = Positions;
            Positions[0, 0] = InitialPositions[2, 0];
            Positions[0, 1] = InitialPositions[1, 0];
            Positions[0, 2] = InitialPositions[0, 0];

            Positions[1, 0] = InitialPositions[2, 1];
            Positions[1, 1] = InitialPositions[1, 1];
            Positions[1, 2] = InitialPositions[0, 1];

            Positions[2, 0] = InitialPositions[2, 2];
            Positions[2, 1] = InitialPositions[1, 2];
            Positions[2, 2] = InitialPositions[0, 2];
        }

        public void RotateAntiClockwise()
        {
            var InitialPositions = Positions;
            Positions[0, 0] = InitialPositions[0, 2];
            Positions[0, 1] = InitialPositions[1, 2];
            Positions[0, 2] = InitialPositions[2, 2];

            Positions[1, 0] = InitialPositions[0, 1];
            Positions[1, 1] = InitialPositions[1, 1];
            Positions[1, 2] = InitialPositions[2, 1];

            Positions[2, 0] = InitialPositions[0, 0];
            Positions[2, 1] = InitialPositions[1, 0];
            Positions[2, 2] = InitialPositions[2, 0];
        }

        public bool Equals(Face comparionFace)
        {
            for (int i = 0; i < Positions.GetLength(0); i++)
            {
                for (int j = 0; j < Positions.GetLength(1); j++)
                {
                    if (Positions[i, j] != comparionFace.Positions[i, j])
                        return false;
                }
            }
            return true;
        }
    }

    public enum Colours
    {
        White,
        Blue,
        Red,
        Yellow,
        Green,
        Orange
    }

    public static class Helpers
    {
        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
