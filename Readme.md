Fortran and C# .NET Interop explained
====================================

This repository contains documentation and example code on how to make Fortran interoperate with the .NET framework.

An introduction to the strange world of mixed language programming with FORTRAN and C# .NET code

1. [Introduction to FORTRAN interopability with .NET](docs/introduction.md)
1. [Exchanging complex data](docs/complex-data.md)
1. [ISO C Binding module](docs/iso-c-binding.md)
1. Callbacks and strings (unfinished)
1. Mixed mode assemblies (unfinished)

Example code
------------

The example code is complete and covers all topics, including callbacks, strings and mixed mode assemblies. The example code is comprised of:

1. Interop.f90 contains the Fortran code.
1. The FortranInterop folder containing the Visual Studio solution.
   1. FortranInterop project demonstrating PInvoke
   1. FortranGlue project demonstrating a Mixed mode assembly
   1. FortranInteropTests proving both integrations using unit tests

References
----------

[Fortran Wiki on interoperability](http://fortranwiki.org/fortran/show/Interoperability)

The articles and example code were initially published at Codeproject.com.

* [Introduction to FORTRAN Interoperability with .NET](http://www.codeproject.com/Articles/1099942/FORTRAN-Interoperability-with-NET-Exchanging-Compl)
* [FORTRAN Interoperability with .NET: Exchanging Complex Data](http://www.codeproject.com/Articles/1065197/Introduction-to-FORTRAN-Interoperability-with-NET)
* [FORTRAN Interoperability with .NET: ISO C Binding module](http://www.codeproject.com/Articles/1096974/FORTRAN-Interoperability-with-NET-Part-II-ISO-C-Bi)


License
-------

[MIT X11](http://en.wikipedia.org/wiki/MIT_License)