module INTEROP

    implicit none

    ! Fix the layout to sequential by specifying the C binding.
    type, bind(C) :: interop_struct_c
	
        integer :: id
        real*8 :: values(10)
        character*16 :: name

    end type


    type :: interop_struct

        ! Fix the layout to sequential by using the sequence keyword.
        sequence

        integer :: id
        real*8 :: values(10)
        character*16 :: name

    end type

    contains

    ! A subroutine demonstrating passing in and out a native Fortran integer type.
    subroutine copy_integer(a, b)
		
        integer, intent(in) :: a
        integer, intent(out) :: b

        b = a

    end subroutine

    ! A subroutine demonstrating passing in and out an integer using the ISO C binding
    ! to ensure compatibility and prevent name mangling.
    subroutine copy_integer_c(a, b) bind(C, name='copy_integer_c')

        use iso_c_binding, only: c_int

        integer(kind=c_int), intent(in) :: a
        integer(kind=c_int), intent(out) :: b

        b = a

    end subroutine

    ! A simple function demonstrating returning a value.
    function return_double(input) result(output)

        real*8, intent(in) :: input
        real*8 :: output

        write(*,*), "Passed value: ",input
        output = input

    end function

    ! A simple function demonstrating returning a value using the ISO C binding.
    function return_double_c(input) result(output) bind(C, name='return_double_c')

        use iso_c_binding

        real(kind=c_double), intent(in) :: input
        real(kind=c_double) :: output

        write(*,*), "Passed value: ",input
        output = input

    end function

    ! A simple function demonstrating returning a value.
    function return_integer(input) result(output)

		! Don't leave the calling convention to chance.
		!GCC$ ATTRIBUTES CDECL ::  return_integer
	
        integer*4, intent(in) :: input
        integer*4 :: output

        write(*,*), "Passed value: ",input
        output = input

    end function

    ! A simple function demonstrating returning a value using the ISO C binding.
    function return_integer_c(input) result(output) bind(C, name='return_integer_c')

        use iso_c_binding

        integer(c_int), intent(in) :: input
        integer(c_int) :: output

        write(*,*), "Passed value: ",input
        output = input

    end function

    ! A subroutine demonstrating passing strings to Fortran using the ISO C binding.
    subroutine string_input_c(text, text_len, status) bind(C, name='string_input')

        use iso_c_binding, only: c_char

        character(kind=c_char), intent(in) :: text(*)
        integer, intent(in), value :: text_len
        integer, intent(out) :: status

        !write (*,*), "Passed value: ",text

    end subroutine

    ! A subroutine demonstrating passing strings of fixed length to Fortran.
    subroutine string_input_fixlen(text, status)

        character(len=10), intent(in) :: text
        integer, intent(out) :: status

        ! Print for debugging purposes and test the passed string.
        write (*,*), "Passed value: ",text
        if (text == "Test") then
            status = 1
        else
            status = -1
        end if
    end subroutine

    ! A subroutine demonstrating passing strings of variable length to Fortran.
    subroutine string_input_varlen(text, text_len, status)

        ! Make sure the string length is passed by value as we need it to specify the
        ! length of the string here in the Fortran code.
        integer, intent(in), value :: text_len
        character(len=text_len), intent(in) :: text
        integer, intent(out) :: status

        ! Print for debugging purposes and test the passed string.
        write (*,*), "Passed value: ",text
        if (text == "Test") then
            status = 1
        else
            status = -1
        end if
    end subroutine
	
	! A subroutine demonstrating returning strings of fixed length from Fortran.
    subroutine string_output_fixlen(text)

        character(len=20), intent(out) :: text

		text = "Hello from Fortran";

    end subroutine
	
	! A subroutine demonstrating returning strings of variable length from Fortran.
    subroutine string_output_varlen(text, text_len)

		integer, intent(inout) :: text_len
        character(len=text_len), intent(out) :: text

        ! Set the string contents and set the actual string length.
		text = "Hello from Fortran";
		text_len = len_trim(text);

    end subroutine

    subroutine callback_example(total, step, callback)

        ! Need to define c_callback as value in order to prevent c_callback
        ! from being a pointer to a pointer because Fortran passes variables
        ! by reference.
        external callback
        integer, intent(in) :: total, step
        integer :: i

        do i = 0,total,step
            call callback(i)
        end do

    end subroutine

    ! A subroutine demonstating a
    subroutine callback_example_c(total, step, c_callback) bind(C, name='callback_example_c')

        use iso_c_binding, only: c_int, c_funptr, c_f_procpointer

        ! Define an interface which describes the callback.
        interface
            subroutine callback(value)
                use iso_c_binding, only: c_int
                integer(kind=c_int), intent(in) :: value
            end subroutine
        end interface

        ! Need to define c_callback as value in order to prevent c_callback
        ! from being a pointer to a pointer because Fortran passes variables
        ! by reference.
        type(c_funptr), intent(in), value :: c_callback
        integer(kind=c_int), intent(in) :: total, step
        integer(kind=c_int) :: i

        procedure(callback), pointer :: callback_ptr

        ! Convert the c pointer to a callable procedure.
        call c_f_procpointer(c_callback, callback_ptr)

        do i = 0,total,step
            call callback_ptr(i)
        end do

    end subroutine


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
            write (*,*), "Adding ",data%values(i)," to ",sum," equals",(sum + data%values(i))
            sum = sum + data%values(i)
        end do

        write(*,*), "Final sum ",sum

        return

    end function

    ! Demonstrates passing a structure / derived type to Fortran.
    ! Returns the sum of the values in the structure.
    function pass_structure_c(data, extra_value) result(sum) bind(C, name='pass_structure_c')

        type(interop_struct_c), intent(in) :: data
        real*8, intent(in) :: extra_value
        real*8 :: sum, sum_intermediate
        integer :: i

        write(*,*), "Passed an extra value of: ",extra_value

        sum_intermediate = 0
        do i = 1, 10
            write (*,*), "Adding ",data%values(i)," to ",sum_intermediate," equals",(sum_intermediate + data%values(i))
            sum_intermediate = sum_intermediate + data%values(i)
        end do

        write(*,*), "Final sum ",sum_intermediate

        sum = sum_intermediate

    end function

    ! Demonstrates passing a structure / derived type back and forward between Fortran and the calling language.
    ! Increments all values in the struct by the specified amount.
    subroutine modify_structure(data, change)

        type(interop_struct), intent(inout) :: data
        real*8, intent(in) :: change

        data%id = 123
        data%values(:) = data%values(:) + change

    end subroutine
	
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
	
	function sum_array_c(data, length) result(sum) bind(C, name='sum_array_c')
	
		use iso_c_binding, only: c_int, c_double
	
		integer(kind=c_int), intent(in) :: length
		real(kind=c_double), intent(in) :: data(length)
		real(kind=c_double) :: sum
		integer :: i
		
		sum = 0
		do i = 1, length
			sum = sum + data(i)
		end do
		
	end function
	
	function sum_array_2d_c(data, length1, length2) result(sum) bind(C, name='sum_array_2d_c')
	
		use iso_c_binding, only: c_int, c_double
	
		integer(kind=c_int), intent(in) :: length1, length2
		real(kind=c_double), intent(in) :: data(length1, length2)
		real(kind=c_double) :: sum
		integer :: i, j
		
		sum = 0
		do i = 1, length1
			do j = 1, length2
				sum = sum + data(i,j)
			end do
		end do
		
	end function
	
	! Demonstrates passing an array and modifying its contents between Fortran and the calling language.
    ! Rotates the passed vectors by the specified angle in radiants over the Z-axis.
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
		! because Fortan stores arrays column oriented, while C and .NET use row oriented.
		do i = 1, vectorCount
			vectors(:,i) = matmul(vectors(:,i), rotationMatrix)
		end do
		
	end subroutine
	
	! Demonstrates passing an array and modifying its contents between Fortran and the calling language using the ISO C binding.
    ! Rotates the passed vectors by the specified angle in radiants over the Z-axis.
	subroutine rotate_vectors_c(vectors, vectorCount, angle) bind(C, name='rotate_vectors_c')
	
		use iso_c_binding, only: c_int, c_double
		
		integer(kind=c_int), intent(in) :: vectorCount
		real(kind=c_double), intent(in) :: angle
		real(kind=c_double), intent(inout) :: vectors(3, vectorCount)
		real(kind=c_double) :: rotationMatrix(3, 3)
		integer :: i
		
		! Setup rotation matrix for rotation over Z-axis by angle.
		data rotationMatrix / 0, 0, 0, 0, 0, 0, 0, 0, 0 /
		rotationMatrix(1,1) = cos(angle)
		rotationMatrix(1,2) = -sin(angle)
		rotationMatrix(2,1) = sin(angle)
		rotationMatrix(2,2) = cos(angle)
		rotationMatrix(3,3) = 1
		
		! Perform the rotation on each vector. Note the switched array indices
		! because Fortan stores arrays column oriented, while C and .NET use row oriented.
		do i = 1, vectorCount
			vectors(:,i) = matmul(vectors(:,i), rotationMatrix)
		end do
		
	end subroutine

end module
