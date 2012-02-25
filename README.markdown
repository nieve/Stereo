# Moved provisionally to my MonoDevelop fork

Due to breaking changes being currently added to the refactoring functionalities on the newresolver branch of MonoDevelop, I've decided to move my efforts inside the MonoDevelop source code for the time being. This version can be found on my newstereo branch here: https://github.com/nieve/monodevelop/tree/newstereo
The work on the dev branch was taken as is and moved into the new branch on MD.
Move to another file is already baked in on the newresolver branch & I had to change the Quick Fixes shortcut to Ctrl|Alt|Enter.

# MonoDevelop.Stereo

An attempt to add some resharper/refactoring functionalities to MonoDevelop Add-In, 
learning on the way how to work without VS.

Currently the add-in contains: 

* Generating classes and interfaces from not-yet-existing types references. Interfaces are generated following name conventions (i.e. ISomething). Invoke by using Alt|Enter.
* Renaming namespaces - usings, namespace declerations and full type names references. Invoke from context menue.
* Move to another file - move any type not matching the name of the current file to a new file, baring the name of the moved type. Invoke by using Alt|Enter.
* Duplicate text - appends selected text or the current line. Invoke by using Ctrl|Alt|D.

## Installing

If you wish to start using the MonoDevelop.Stereo add-in without going through the pain of building, simply download the libraries only (found on the download page) and extract the MonoDevelop.Stereo folder found inside the zip directly to your %MonoDevelop Installation Path%\AddIns directory and you're ready to go.

## A work in progress

This is a work in progress, far from being a finalised product.
The decision I've taken in the beginning of my work on it was to deliver as much in as little time as possible, worrying more about functionality than any looks and aiming towards a seamless integration in the IDE, following its very own refactoring add-ins.
If you find any issues, problems or bugs please let me know, open an issue or even send a pull request if you feel like it. I'd appreciate any kind of feedback. Forkings, testing, bugs & issues, ideas and requests are welcome!

(More info to come...)

## Legal Gobbledygook (MIT License)
(For lack of a better suited or perhaps a better knowledge of what I'm supposed to put here)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.