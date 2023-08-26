# MoqToNSubstituteConverter
## A console application to convert Moq in unit tests to NSubstitute

### This project only has replacement code for the following:
``` csharp
Mock<ClassToMock> = ClassToMock
new Mock<ClassToMock>() = Substitute.For<ClassToMock>()
new Mock<ClassToMock>() { CallBase = true } = Substitute.ForPartsOf
It.IsAny = Arg.Any
It.Is = Arg.Is
.Object will be removed
.Verifiable will be removed
.Result will be removed
.Setup(name => name will be removed
.Verify(name => name = Received()
```
``` csharp
.Returns, .ReturnsAsync = .Returns
.Throws, .ThrowsAsync = .Throws
```
``` csharp
Times.Once = .Received(1)
Times.Exactly(3) = .Received(3)
Times.Never = .DidNotReceive()
```

## Limitations:
* This will work best on well formatted code
* There is no replacment for `.VerifyAll` or `.Protected`
* `It.IsAnyType` does not currently have a replacement
* Some of the replaced statements no longer have the correct indentation, they are left justified, this could be fixed in Visual Studio by reformatting the document
* The replacement for `Mock<ClassToMock> = new()` is not technically correct, it will replace it with `ClassToMock = new()` but it should be `ClassToMock = Substitute.For<ClassToMock>()`

## How to Run the code (PowerShell)
By default the executable will run in the current directory and it will perfom analysis only so you can see the results of the analysis in the log file.
```
.\MoqToNSubstitute.exe
```
To perform analysis on a specific directory:
```
.\MoqToNSubstitute.exe C:\Users\user\Documents\Projects\Vts.Api\Vts.Api.Tests\
```
To perform transformation on a specific directory:
```
.\MoqToNSubstitute.exe C:\Users\user\Documents\Projects\Vts.Api\Vts.Api.Tests\ false
```

## How to Debug the code (Visual Studio)
If you place the folder for the code to be analyzed in the bin folder of the main project `\MoqToNSubstitute\MoqToNSubstitute\bin\Debug\net6.0\` you can set breakpoints and step through the code, it will analyze by default. The log file will be located in the logs folder in that same location `\MoqToNSubstitute\MoqToNSubstitute\bin\Debug\net6.0\logs`. 