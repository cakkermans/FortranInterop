#pragma once

/* External function declaration of the regular Fortran return integer function.
Name mangling is prevented using the extern "C" attribute.
*/
extern "C" __declspec(dllimport) int __interop_MOD_return_integer(int *value);

/* External function declaration of the ISO C binding based Fortran return integer function.
Name mangling is prevented using the extern "C" attribute.
*/
extern "C" __declspec(dllimport) int return_integer_c(int *value);

extern "C" __declspec(dllimport) void __interop_MOD_copy_integer(int *a, int *b);
extern "C" __declspec(dllimport) void copy_integer_c(int *a, int *b);
extern "C" __declspec(dllimport) double __interop_MOD_return_double(double *value);
extern "C" __declspec(dllimport) double return_double_c(double *value);




