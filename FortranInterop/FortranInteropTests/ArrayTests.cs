using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FortranInterop
{
    public class ArrayTests
    {

        [Fact]
        public void SumArrayFixed()
        {

            const int length = 10;
            var data = new double[length];

            // Fill the array with the series n_i=i^2 divided by 3.
            for (int i = 0; i < length; i++)
                data[i] = Math.Pow(i, 2) / 3D;

            var result1 = Interop.SumArrayFixed(data);
            Assert.Equal(data.Sum(), result1);

            var result2 = Interop.SumArrayFixedSimple(data);
            Assert.Equal(data.Sum(), result2);
        }

        [Fact]
        public void SumArray()
        {

            int length = 8;
            var data = new double[length];

            // Fill the array with the series n_i=i^2 divided by 3.
            for (int i = 0; i < length; i++)
                data[i] = Math.Pow(i, 2) / 3D;

            var result = Interop.SumArray(data, ref length);
            Assert.Equal(data.Sum(), result);
        }

        [Fact]
        public void SumArrayIsoC()
        {

            int length = 8;
            var data = new double[length];

            // Fill the array with the series n_i=i^2 divided by 3.
            for (int i = 0; i < length; i++)
                data[i] = Math.Pow(i, 2) / 3D;

            var result = Interop.SumArrayIsoC(data, ref length);
            Assert.Equal(data.Sum(), result);
        }

        [Fact]
        public void SumArray2D()
        {

            int length1 = 8;
            int length2 = 5;
            var data = new double[length1, length2];
            double sum = 0;

            // Fill the array with the series n_i=i^2 divided by 3.
            for (int i = 0; i < length1; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    data[i, j] = Math.Pow(i, 2) + j / 3D;
                    sum += data[i, j];
                }
            }

            var result = Interop.SumArrayIsoC(data, ref length1, ref length2);
            Assert.Equal(sum, result);
        }

        [Fact]
        public void RotateVectors()
        {

            // Use unit vectors to test.
            var vectors = new double[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            };
            var expected = new double[,]
            {
                { 0, -1, 0 },
                { 1, 0, 0 },
                { 0, 0, 1 },
            };

            // Ask for a 90 degree rotation ( = 1/2 Pi radiant)
            var angle = Math.PI / 2;
            var vectorCount = 3;

            Interop.RotateVectors(vectors, ref vectorCount, ref angle);

            // Calculate the error as due to numerical calculation the result
            // won't be exact.
            var error = 0D;
            for (int i = 0; i < vectors.GetLength(0); i++)
            {
                error += Math.Abs(vectors[i, 0] - expected[i, 0]);
                error += Math.Abs(vectors[i, 1] - expected[i, 1]);
                error += Math.Abs(vectors[i, 2] - expected[i, 2]);
            }

            // We expect the error to be small if this works.
            Assert.True(error < 1e-15);
        }

        [Fact]
        public void RotateVectorsIsoC()
        {

            // Use unit vectors to test.
            var vectors = new double[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            };
            var expected = new double[,]
            {
                { 0, -1, 0 },
                { 1, 0, 0 },
                { 0, 0, 1 },
            };

            // Ask for a 90 degree rotation ( = 1/2 Pi radiant)
            var angle = Math.PI / 2;
            var vectorCount = 3;

            Interop.RotateVectorsIsoC(vectors, ref vectorCount, ref angle);

            // Calculate the error as due to numerical calculation the result
            // won't be exact.
            var error = 0D;
            for (int i = 0; i < vectors.GetLength(0); i++)
            {
                error += Math.Abs(vectors[i, 0] - expected[i, 0]);
                error += Math.Abs(vectors[i, 1] - expected[i, 1]);
                error += Math.Abs(vectors[i, 2] - expected[i, 2]);
            }

            // We expect the error to be small if this works.
            Assert.True(error < 1e-15);
        }
    }
}
