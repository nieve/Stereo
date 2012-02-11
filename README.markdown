# MonoDevelop.Stereo

An attempt to add some resharper/refactoring functionalities to MonoDevelop Add-In, 
learning on the way how to work without VS.

Currently the add-in contains: 

* Generating classes and interfaces from not-yet-existing types references. Interfaces are generated following name conventions (i.e. ISomething). Invoke from context menue.
* Renaming namespaces - usings, namespace declerations and full type names references. Invoke from context menue.
* Move to another file - move any type not matching the name of the current file to a new file, baring the name of the moved type. Invoke by using Alt|Enter.
* Duplicate text - appends selected text or the current line. Invoke by using Ctrl|Alt|D.

## Building Pre-requisits

You will need to have the Mono Libraries Package installed on your machine in order to build the solution.
You can get it here: http://monodevelop.com/files/Windows/MonoLibraries.msi

Forkings, testing, bugs & issues, ideas and requests are welcome!

(More info to come...)