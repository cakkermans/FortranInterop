using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FortranInterop
{
    public class DataStructureTests
    {

        [Fact]
        public void PassStructure()
        {

            var data = new FortranInteropStruct();
            data.Name = new char[16];
            data.Values = new double[10];

            // Fill the values with the series n_i=i^2 divided by 3.
            for (int i = 0; i < 10; i++)
                data.Values[i] = Math.Pow(i, 2) / 3D;

            double extra = 0;

            var result = Interop.PassStruct(ref data, ref extra);
            Assert.Equal(data.Values.Sum(), result);
        }

        [Fact]
        public void PassStructureIsoC()
        {

            var data = new FortranInteropStruct();
            data.Name = new char[16];
            data.Values = new double[10];

            // Fill the values with the series n_i=i^2 divided by 3.
            for (int i = 0; i < 10; i++)
                data.Values[i] = Math.Pow(i, 2) / 3D;

            double extra = 0;

            var result = Interop.PassStructIsoC(ref data, ref extra);
            Assert.Equal(data.Values.Sum(), result);
        }

        [Fact]
        public void ModifyStructure()
        {

            var data = new FortranInteropStruct();
            data.Name = new char[16];
            data.Values = new double[10];

            // Fill the values with the series n_i=i^2 divided by 3.
            for (int i = 0; i < 10; i++)
                data.Values[i] = Math.Pow(i, 2) / 3D;

            double change = 100D / 7D;

            Interop.ModifyStruct(ref data, ref change);

            for(int i = 0; i < 10; i++)
                Assert.Equal(Math.Pow(i, 2) / 3D + change, data.Values[i]);
        }

        [Fact]
        public void PassClass()
        {

            var data = new FortranInteropClass();

            // Fill the values with the series n_i=i^2 divided by 3.
            for (int i = 0; i < 10; i++)
                data.Values[i] = Math.Pow(i, 2) / 3D;

            double extra = 0;

            var result = Interop.PassClass(data, ref extra);
            Assert.Equal(data.Values.Sum(), result);
        }

        [Fact]
        public void ReturnStruct()
        {

            var data = Interop.ReturnStruct();
            //var data = System.Runtime.InteropServices.Marshal.PtrToStructure<FortranInteropStruct>(dataPtr);

            Assert.Equal(123, data.Id);
        }
    }
}
