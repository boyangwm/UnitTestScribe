To run UnitTestScribe from commandline:

Type
WM.UnitTestScribe summary --loc [Location of project under analysis] --srcmlPath [SrcML location] --outputLoc [Generated summary location]

For example,

WM.UnitTestScribe summary --loc D:\Research\Subjects\SrcML.NET --srcmlPath D:\Research\SrcML\ --outputLoc c:\temp\test.html


All subject projects for testing is under
[UnitTestScribe reporsitory]/Subjects/

SrcML is under
[UnitTestScribe reporsitory]/SrcML/


=================================================

For debugging,  

Uncomment https://github.com/boyangwm/UnitTestScribe/blob/master/WM.UnitTestScribe/Program.cs#L39

Then, change parameters' values. 
