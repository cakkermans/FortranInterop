# FORTRAN Interoperability with .NET: Exchanging Complex Data

This article in the FORTRAN Interoperability with .NET series explains how to exchange complex data between C# and FORTRAN.

## Introduction

In the first article in this series, we introduced the basic necessities to make calling Fortran code from the .NET world possible. In that example, only a simple integer was passed to Fortran and returned to the .NET code in order to show that the basic concept worked. In a real world situation, you’ll probably find yourself wanting to pass back and forward all kinds of data, perhaps even structured data. This article will explain the techniques, tricks and pitfalls of passing data between Fortran and the .NET world.

1. Introduction to FORTRAN interopability with .NET
1. Exchanging complex data
1. ISO C Binding module
1. Callbacks and strings
1. Mixed mode assemblies

So how does one accomplish this interoperability (or interop for short) between different languages? This series will explain what is involved in making Fortran and C# code running on the .NET framework interoperate. It is assumed that the main program is running on the .NET framework; the C# code will call the Fortran code. Callbacks may be involved, but the native Fortran code will never invoke any C# code by itself.

## Receiving Simple Data

In the elementary example, we used a Fortran function returning a value to demonstrate the working of calling Fortran code from .NET. In some cases, one may however want to return more than one value from a function or subroutine, just like C# has the out declaration for method parameters.

In Fortran, this kind of functionality is achieved more or less similar to returning a value from a Fortran function: you put the parameter in the parameter list and declare its intent to be `out`. On the C# side of things, you also need to declare the parameter as `out` (`ref` will also work).

Take for example, the following Fortran code:

```fortran
! A subroutine demonstrating passing in and out a native Fortran integer type.
subroutine copy_integer(a, b)
    
    integer, intent(in) :: a
    integer, intent(out) :: b

    b = a

end subroutine
```

The matching C# external method declaration becomes:

```C#
[DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, 
EntryPoint = "__interop_MOD_copy_integer")]

public static extern void CopyInteger(ref int a, out int b);
```

## Working with Arrays

If you are working with Fortran code, then there’s a fat chance you’ll be working with matrices a lot. As I mentioned earlier, Fortran is sort of the king in the land of numerics and where there is numerical work being done, there are usually matrices and vectors involved. This section will introduce you to the techniques required to pass arrays between C# and Fortran. First, I'll discuss all the issues at hand and at the end of this section, a number of examples will be presented.

### Dimension Inversion

An important functional difference between Fortran and most other programming languages is the manner in which Fortran stores multidimensional arrays. While most languages store a multidimensional array row after row, Fortran stores them column after column. From a numerical point of view, the column after column method makes more sense as it really is like you are storing a set of vectors and it makes matrix operations slightly more straightforward on the lower level. Beyond that, this difference is normally not too much of interest to the programmer using the language, except (surprise surprise) when you start to do mixed language programming.

Due to the fact that arrays are usually transferred as complete blocks of memory (or a pointer to one, we’ll go into a bit more depth in the ‘Marshaling’ section), the exact manner in which arrays elements are stored in memory does become a concern.

### Fixed Length Arrays

Passing a fixed length array to Fortran is fairly straight forward. Simply declare the variable in Fortran to be an array of known length. On the C# side, one can simply pass an array to the Fortran code. The only thing of which you’ll need to take care is that this array is of the correct length. Extra marshaling instructions on the parameter declaration in the C# external method declaration can prevent incorrect size arrays from being passed accidentally. Compare the following two external method declarations:

```C#
[DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, 
EntryPoint = "__interop_MOD_sum_array_fixed")]
public static extern double SumArrayFixed([MarshalAs(UnmanagedType.LPArray, SizeConst = 10)] 
double[] input);

[DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, 
EntryPoint = "__interop_MOD_sum_array_fixed")]
public static extern double SumArrayFixedSimple(double[] input);
```

The first contains a `MarshalAs` attribute on the input parameter, in which the property `SizeConst` is set to `10`. Passing an incorrect length array will throw an error.

The matching Fortran code for these method declarations is as follows:

```fortran
! Demonstrates passing a fixed length array to Fortran.
! Returns the sum of the values in the array.
function sum_array_fixed(data) result(sum)

    real*8, intent(in) :: data(10)
    real*8 :: sum
    integer :: i
    
    sum = 0
    do i = 1, 10
        sum = sum + data(i)
    end do
    
    sum = sum
    
end function
```

### Variable Length Arrays

Passing a variable length array from C# to Fortran is slightly more involved. By passing the length of the array as an argument to the Fortran function, we can use that argument to define the length of the array on the Fortran size. The example Fortran code which returns the sum of all the elements in the passed array looks like:

```fortran
! Demonstrates passing a variable length array to Fortran.
! Returns the sum of the values in the array.
function sum_array(data, length) result(sum)

    integer*4, intent(in) :: length
    real*8, intent(in) :: data(length)
    real*8 :: sum
    integer :: i

    sum = 0
    do i = 1, length
        sum = sum + data(i)
    end do

end function
```

We can slightly ease our life by introducing a slim wrapper function which takes care of passing the array length to Fortran. The external method declaration then looks as follows:

```C#
[DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl,
EntryPoint = "__interop_MOD_sum_array")]
public static extern double SumArray(double[] input, ref int inputLength);

public static double SumArray(double[] input)
{
    int length = input.Length;
    return SumArray(input, ref length);
}
```

Modifying Arrays

In a lot of cases, it may be necessary to modify the contents of arrays in Fortran and to transport the results back to C#. One example may be the coordinates of a set of tracked objects, which undergo a rotation. The following Fortran code accepts a variable length list of 3D coordinates, which are stored as a 3xn 2 dimensional array. These coordinates undergo a rotation along the Z-axis. The results are written back to the vector containing the coordinates.

```fortran
! Demonstrates passing an array and modifying its contents between Fortran and the calling language.
! Rotates the passed vectors by the specified angle in radians over the Z-axis.
subroutine rotate_vectors(vectors, vectorCount, angle)

    use iso_c_binding, only: c_int, c_double
    
    integer*4, intent(in) :: vectorCount
    real*8, intent(in) :: angle
    real*8, intent(inout) :: vectors(3, vectorCount)
    real*8 :: rotationMatrix(3, 3)
    integer :: i
    
    ! Setup rotation matrix for rotation over Z-axis by angle.
    data rotationMatrix / 0, 0, 0, 0, 0, 0, 0, 0, 0 /
    rotationMatrix(1,1) = cos(angle)
    rotationMatrix(1,2) = -sin(angle)
    rotationMatrix(2,1) = sin(angle)
    rotationMatrix(2,2) = cos(angle)
    rotationMatrix(3,3) = 1
    
    ! Perform the rotation on each vector. Note the switched array indices
    ! because Fortran stores arrays column oriented, while C and .NET use row oriented.
    do i = 1, vectorCount
        vectors(:,i) = matmul(vectors(:,i), rotationMatrix)
    end do
    
end subroutine
```

In order to be able to use the above code from the .NET world, we’ll need our usual external method declaration:

```C#
[DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, 
EntryPoint = "__interop_MOD_rotate_vectors")]
public static extern double RotateVectors([In, Out] double[,] vectors, 
ref int vectorCount, ref double angle);
```

The only real new things are the `[In, Out]` attributes which decorate the vectors parameter. The In and Out attributes instruct the .NET framework that the memory of the vector array not only needs to be copied to the unmanaged world, but also back to the managed world. This process of copying is referred to as marshaling. The attentive reader may now notice that this is odd; as we are passing a pointer to the vector array, so any changes should be written directly to the memory block of the vector array. Unfortunately, this does not always have to be true due to the way marshaling works (in this very case of a simple array of doubles, it is however likely that only a pointer is passed).

## Passing Data Structures

In even more complex cases, you’ll want to pass data to your Fortran program using structures or classes. Due to the object oriented nature of C# and many other .NET languages, this would be a very natural manner to pass data.

As Fortran also has a notion of complex data types, it is entirely feasible to pass data to Fortran in the form of a structure or class. As always, there are however a bunch of details which need to be taken care of. Before we dive into an example, all the issues at hand are shortly discussed here.

### .NET Class or Structure

First of all, you’ll need to decide if you want to pass your data as a structure or as a class. In the .NET world, the key difference in this context is that in a `struct` is a value type and that a `class` is a reference type. This means that a variable containing a class instance contains a reference to the memory location where the class instance is stored, while a variable containing a struct instance holds the actual data of the struct.

More information can be found at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/

This key difference needs to be kept in mind because Fortran expects arguments to be passed as references by default and you’ll thus need to take care that the argument passing is performed correctly.

### Data Sequence

A second issue is the sequence of the data in a structure of class. Unless you take special care, there is no guarantee that the member fields of a data structure are stored in the order in which they are declared. Take for example the following structure:

```C#
public struct FortranInteropStruct
{
    public int Id;
    public double[] Values;
}
```

There is no reason why the .NET runtime would not be allowed to first store the array of doubles and then the Id integer, instead of the other way around. The same applies to the Fortran complex data types. While the situation may not be so bleak that every compiler will completely scrable the sequence of your data types, the data field sequence is a thing that needs to be ensured.

In C#, this is accomplished using the `StructLayout` attribute, which allows one to specify how a data structure should be laid out. It works both on classes and structs. By passing `LayoutKind.Sequential`, you can instruct the runtime to lay out the data in memory in the sequence as it is declared:

```C#
[StructLayout(LayoutKind.Sequential)]
public struct FortranInteropStruct
{
    public int Id;
    public double[] Values;
}
```

In Fortran, one can make use of the `sequential` keyword, which works like the `LayoutKind.Sequential` argument for the `StructLayoutAttribute`:

```fortran
type :: struct_name
    sequential
    [members]
end type
```

More information can be found at: https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.structlayoutattribute

## Data Alignment

A final thing to look out for when passing structures back and forward is the data alignment. For various reasons, it is usually beneficial to align fields on 4 or 8 byte boundaries. So while for example, a short (16 bit) integer field only takes up two bytes, the next field will only be stored a 6 bytes further. While we will not go into details on the reasons for doing so, this is a think to look out for. I think you can imagine that if the data alignment on the .NET side and Fortran side are different, you will not find your values back when passing data between the two languages.

For Fortran compilers, it is sometimes possible to specify the data alignment. For the GNU Fortran compiler, I have not been able to find the option, so I have have assumed that it is 8 bytes. This is the usually used alignment step on modern processors. The Intel Fortran compiler allows you to explicitly specify the desired alignment.

On the .NET side, we can simply specify the data alignment using the `StructLayoutAttribute` using the `Pack` property. Setting data alignment to 8 bytes for our example `struct` then looks as follows:

```C#
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct FortranInteropStruct
{
    public int Id;
    public double[] Values;
}
```

## Marshaling

This topic was already briefly touched upon in the section on arrays. Marshaling refers to the process of managing the passing of data between managed and unmanaged code. So far, we have only dealt with data types which have equal representations in both managed and unmanaged code. An integer in .NET is for example 32 bits long and this is no different in the Fortran world. For more complex types such as classes, these assumptions may however no longer apply. Strings are another example; in the .NET world all strings are arrays of 16 bit character elements. A lot of platforms however work with 8 bit character elements.

Another issue is the nature of managed memory. The garbage collector from the .NET framework may at any moment move memory blocks around, potentially causing problems if pointers to these memory blocks have been passed to native code. Again the .NET marshaling functionality can take care of this by copying data to a fixed memory block or by pinning the managed memory block itself.

I won’t be going to deep into the aspects of marshaling in this article, but when you see attributes like `In`, `Out`, or `MarshalAs`, then you should know that these are used to control how data is marshaled between the .NET world and the Fortran world.

More information can be found at: https://learn.microsoft.com/en-us/dotnet/framework/interop/interop-marshalling

## Embedded Arrays

Fortran by default makes use of embedded arrays in its data structure. This means that instead of containing a pointer to the array, the values of the array are laid out in sequence inside the structure. This is the opposite of how it works in the .NET world. Luckily, we can use marshaling to manage this difference for us by decorating the arrays in C# with the `MarshalAs` attribute and specifying `UnmanagedType.ByValArray`:

```C#
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct FortranInteropStruct
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public double[] Values;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public char[] Name;
}
```

## Passing a Structure

Putting all the above treated matters to use allows one to seamlessly use the following Fortran code from .NET. Suppose we have a simple Fortran function accepting a structure and returning the sum of the elements in the fixed length array embedded in the structure:

```fortran
type :: interop_struct

    ! Fix the layout to sequential by using the sequence keyword.
    sequence

    integer :: id
    real*8 :: values(10)
    character*16 :: name

end type

! Demonstrates passing a structure / derived type to Fortran.
! Returns the sum of the values in the structure.
function pass_structure(data, extra_value) result(sum)

    type(interop_struct), intent(in) :: data
    real*8, intent(in) :: extra_value
    real*8 :: sum
    integer :: i

    write(*,*), "Passed an extra value of: ",extra_value

    sum = 0
    do i = 1, 10
        write (*,*), "Adding ",data%values(i)," 
        to ",sum," equals",(sum_intermediate + data%values(i))
        sum = sum + data%values(i)
    end do

    write(*,*), "Final sum ",sum

    return

end function
```

In C#, we define a matching structure and matching external method declaration:

```C#
/// <summary>
/// Structure used to demonstrate marshalling complex data types between .NET and Fortran.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct FortranInteropStruct
{
    public int Id;

    /// <summary>
    /// Fixed length array holding double values.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public double[] Values;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public char[] Name;
}
[DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, 
EntryPoint = "__interop_MOD_pass_structure", CharSet = CharSet.Ansi)]
public static extern double PassStruct(ref FortranInteropStruct data, ref double extra_value);
```

Perhaps the only thing to really look out for in this example is the fixed size arrays which needs to be taken care of. Just as when directly passing fixed size arrays, we need to instruct the .NET framework to correctly marshal these arrays. This is accomplished by decorating the arrays with the `MarshalAs` attribute. In addition, don’t forget about the embedded arrays.

## Passing a Class

Passing a class from .NET to Fortran is now pretty straight forward, with the only key difference being the omission of the `ref` declaration. Note that we are using the exact same Fortran function for passing the class as we did for demonstrating passing a structure.

```C#
/// <summary>
/// Class used to demonstrate marshalling complex data types between .NET and Fortran.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public class FortranInteropClass
{

    internal const int ValuesSize = 10;
    internal const int NameSize = 16;

    public int Id;

    /// <summary>
    /// Fixed length array holding double values.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ValuesSize)]
    public double[] Values;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NameSize)]
    public char[] Name;

    public FortranInteropClass()
    {

        Values = new double[ValuesSize];
        Name = new char[NameSize];
    }
}

[DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, 
EntryPoint = "__interop_MOD_pass_structure", CharSet = CharSet.Ansi)]
public static extern double PassClass(FortranInteropClass data, ref double extra_value);
```

### Receiving Data Structures

Naturally, you’ll also want to be able to receive data structure from Fortran. By now, you should be able to guess that the key difference between the previous two examples will be the marshaling instructions. By decorating the parameter passing the structure with the `In` and `Out` attributes, we ensure that any changes to the structure made in Fortran are copied back to the .NET world. Take, for example, the following Fortran code which modifies the values in the structure by a given amount:

```fortran
! Demonstrates passing a structure / derived type back and forward between Fortran and the calling language.
! Increments all values in the struct by the specified amount.
subroutine modify_structure(data, change)

    type(interop_struct), intent(inout) :: data
    real*8, intent(in) :: change

    data%id = 123
    data%values(:) = data%values(:) + change

end subroutine
```

The matching C# external method declaration becomes:

```C#
[DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, 
EntryPoint = "__interop_MOD_modify_structure", CharSet = CharSet.Ansi)]
public static extern void ModifyStruct([In, Out] ref FortranInteropStruct data, ref double change);
```

## Using the Code

So, does this all really work? Yes, it actually works pretty well. Attached to this article is example code showing all of the presented techniques in action. Each of the pieces of code is a structure as a unit test to verify the correct functioning.

The process of compiling and running the Fortran code was covered fairly extensively in the first article in this series. If you are lost on what to do with the example, or are running into issues, I would kindly like to ask you to read that first article.

## Conclusion

In this article, it was demonstrated how to pass and receive simple values, arrays and structures. Doing so allows one to effectively make use of Fortran code from the .NET world. Many of the aspects involved in making C# and Fortran code interoperate also apply when making other languages interoperate.

In the next articles in this series, we'll look at handling strings and passing callback between Fortran and C#.