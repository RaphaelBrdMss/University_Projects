# CMAKE generated file: DO NOT EDIT!
# Generated by "Unix Makefiles" Generator, CMake Version 3.17

# Delete rule output on recipe failure.
.DELETE_ON_ERROR:


#=============================================================================
# Special targets provided by cmake.

# Disable implicit rules so canonical targets will work.
.SUFFIXES:


# Disable VCS-based implicit rules.
% : %,v


# Disable VCS-based implicit rules.
% : RCS/%


# Disable VCS-based implicit rules.
% : RCS/%,v


# Disable VCS-based implicit rules.
% : SCCS/s.%


# Disable VCS-based implicit rules.
% : s.%


.SUFFIXES: .hpux_make_needs_suffix_list


# Command-line flag to silence nested $(MAKE).
$(VERBOSE)MAKESILENT = -s

# Suppress display of executed commands.
$(VERBOSE).SILENT:


# A target that is always out of date.
cmake_force:

.PHONY : cmake_force

#=============================================================================
# Set environment variables for the build.

# The shell in which to execute make rules.
SHELL = /bin/sh

# The CMake executable.
CMAKE_COMMAND = /Applications/CLion.app/Contents/bin/cmake/mac/bin/cmake

# The command to remove a file.
RM = /Applications/CLion.app/Contents/bin/cmake/mac/bin/cmake -E rm -f

# Escaping for special characters.
EQUALS = =

# The top-level source directory on which CMake was run.
CMAKE_SOURCE_DIR = /Users/raphaelbraud-mussi/Desktop/raytracer-template

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = /Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug

# Include any dependencies generated for this target.
include CMakeFiles/raytracer.dir/depend.make

# Include the progress variables for this target.
include CMakeFiles/raytracer.dir/progress.make

# Include the compile flags for this target's objects.
include CMakeFiles/raytracer.dir/flags.make

CMakeFiles/raytracer.dir/source/main.cpp.o: CMakeFiles/raytracer.dir/flags.make
CMakeFiles/raytracer.dir/source/main.cpp.o: ../source/main.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug/CMakeFiles --progress-num=$(CMAKE_PROGRESS_1) "Building CXX object CMakeFiles/raytracer.dir/source/main.cpp.o"
	/Library/Developer/CommandLineTools/usr/bin/c++  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles/raytracer.dir/source/main.cpp.o -c /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/main.cpp

CMakeFiles/raytracer.dir/source/main.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/raytracer.dir/source/main.cpp.i"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/main.cpp > CMakeFiles/raytracer.dir/source/main.cpp.i

CMakeFiles/raytracer.dir/source/main.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/raytracer.dir/source/main.cpp.s"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/main.cpp -o CMakeFiles/raytracer.dir/source/main.cpp.s

CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.o: CMakeFiles/raytracer.dir/flags.make
CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.o: ../source/common/ObjMesh.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug/CMakeFiles --progress-num=$(CMAKE_PROGRESS_2) "Building CXX object CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.o"
	/Library/Developer/CommandLineTools/usr/bin/c++  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.o -c /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/ObjMesh.cpp

CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.i"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/ObjMesh.cpp > CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.i

CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.s"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/ObjMesh.cpp -o CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.s

CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.o: CMakeFiles/raytracer.dir/flags.make
CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.o: ../source/common/SourcePath.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug/CMakeFiles --progress-num=$(CMAKE_PROGRESS_3) "Building CXX object CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.o"
	/Library/Developer/CommandLineTools/usr/bin/c++  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.o -c /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/SourcePath.cpp

CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.i"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/SourcePath.cpp > CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.i

CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.s"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/SourcePath.cpp -o CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.s

CMakeFiles/raytracer.dir/source/common/Trackball.cpp.o: CMakeFiles/raytracer.dir/flags.make
CMakeFiles/raytracer.dir/source/common/Trackball.cpp.o: ../source/common/Trackball.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug/CMakeFiles --progress-num=$(CMAKE_PROGRESS_4) "Building CXX object CMakeFiles/raytracer.dir/source/common/Trackball.cpp.o"
	/Library/Developer/CommandLineTools/usr/bin/c++  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles/raytracer.dir/source/common/Trackball.cpp.o -c /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/Trackball.cpp

CMakeFiles/raytracer.dir/source/common/Trackball.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/raytracer.dir/source/common/Trackball.cpp.i"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/Trackball.cpp > CMakeFiles/raytracer.dir/source/common/Trackball.cpp.i

CMakeFiles/raytracer.dir/source/common/Trackball.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/raytracer.dir/source/common/Trackball.cpp.s"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/Trackball.cpp -o CMakeFiles/raytracer.dir/source/common/Trackball.cpp.s

CMakeFiles/raytracer.dir/source/common/Object.cpp.o: CMakeFiles/raytracer.dir/flags.make
CMakeFiles/raytracer.dir/source/common/Object.cpp.o: ../source/common/Object.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug/CMakeFiles --progress-num=$(CMAKE_PROGRESS_5) "Building CXX object CMakeFiles/raytracer.dir/source/common/Object.cpp.o"
	/Library/Developer/CommandLineTools/usr/bin/c++  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles/raytracer.dir/source/common/Object.cpp.o -c /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/Object.cpp

CMakeFiles/raytracer.dir/source/common/Object.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/raytracer.dir/source/common/Object.cpp.i"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/Object.cpp > CMakeFiles/raytracer.dir/source/common/Object.cpp.i

CMakeFiles/raytracer.dir/source/common/Object.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/raytracer.dir/source/common/Object.cpp.s"
	/Library/Developer/CommandLineTools/usr/bin/c++ $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S /Users/raphaelbraud-mussi/Desktop/raytracer-template/source/common/Object.cpp -o CMakeFiles/raytracer.dir/source/common/Object.cpp.s

# Object files for target raytracer
raytracer_OBJECTS = \
"CMakeFiles/raytracer.dir/source/main.cpp.o" \
"CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.o" \
"CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.o" \
"CMakeFiles/raytracer.dir/source/common/Trackball.cpp.o" \
"CMakeFiles/raytracer.dir/source/common/Object.cpp.o"

# External object files for target raytracer
raytracer_EXTERNAL_OBJECTS =

raytracer.app/Contents/MacOS/raytracer: CMakeFiles/raytracer.dir/source/main.cpp.o
raytracer.app/Contents/MacOS/raytracer: CMakeFiles/raytracer.dir/source/common/ObjMesh.cpp.o
raytracer.app/Contents/MacOS/raytracer: CMakeFiles/raytracer.dir/source/common/SourcePath.cpp.o
raytracer.app/Contents/MacOS/raytracer: CMakeFiles/raytracer.dir/source/common/Trackball.cpp.o
raytracer.app/Contents/MacOS/raytracer: CMakeFiles/raytracer.dir/source/common/Object.cpp.o
raytracer.app/Contents/MacOS/raytracer: CMakeFiles/raytracer.dir/build.make
raytracer.app/Contents/MacOS/raytracer: glfw-3.2/src/libglfw3.a
raytracer.app/Contents/MacOS/raytracer: libglad.a
raytracer.app/Contents/MacOS/raytracer: pngdecode/libpngdecode.a
raytracer.app/Contents/MacOS/raytracer: libglad.a
raytracer.app/Contents/MacOS/raytracer: glfw-3.2/src/libglfw3.a
raytracer.app/Contents/MacOS/raytracer: CMakeFiles/raytracer.dir/link.txt
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --bold --progress-dir=/Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug/CMakeFiles --progress-num=$(CMAKE_PROGRESS_6) "Linking CXX executable raytracer.app/Contents/MacOS/raytracer"
	$(CMAKE_COMMAND) -E cmake_link_script CMakeFiles/raytracer.dir/link.txt --verbose=$(VERBOSE)

# Rule to build all files generated by this target.
CMakeFiles/raytracer.dir/build: raytracer.app/Contents/MacOS/raytracer

.PHONY : CMakeFiles/raytracer.dir/build

CMakeFiles/raytracer.dir/clean:
	$(CMAKE_COMMAND) -P CMakeFiles/raytracer.dir/cmake_clean.cmake
.PHONY : CMakeFiles/raytracer.dir/clean

CMakeFiles/raytracer.dir/depend:
	cd /Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug && $(CMAKE_COMMAND) -E cmake_depends "Unix Makefiles" /Users/raphaelbraud-mussi/Desktop/raytracer-template /Users/raphaelbraud-mussi/Desktop/raytracer-template /Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug /Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug /Users/raphaelbraud-mussi/Desktop/raytracer-template/cmake-build-debug/CMakeFiles/raytracer.dir/DependInfo.cmake --color=$(COLOR)
.PHONY : CMakeFiles/raytracer.dir/depend

