// FortranGlue.h

#pragma once
#include "Fortran.h"
#include <Windows.h>

using namespace System;

namespace FortranInterop 
{

	public ref class FortranWrapper
	{

	public:

		/*
		 * This is managed wrapper function for the native Fortran function which simply
		 * returns the passed value.
		 */
		static Int32 ReturnInteger(Int32 value)
		{

			int result;

			// Simply forward the call to Fortran and return the value.
			result = __interop_MOD_return_integer(&value);
			
			return result;
		}

		/*
		 * This is a managed wrapper function for the native Fotran function which returns
		 * the passed value. Similar to ReturnInteger but using the ISO C binding module
		 * on the Fortran side.
		 */
		static Int32 ReturnIntegerIsoC(Int32 value)
		{

			int result;

			// Simply forward the call to Fortran and return the value.
			result = return_integer_c(&value);

			return result;
		}
	};
}
