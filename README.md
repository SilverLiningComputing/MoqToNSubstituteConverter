# MoqToNSubstituteConverter
## A console application to convert Moq in unit tests to NSubstitute

### This project only has replacement code for the following:
``` csharp
Mock<ClassToMock> = ClassToMock
new Mock<ClassToMock> = Substitute.For
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

Times.Once = .Received(1)
Times.Exactly(3) = .Received(3)
Times.Never = .DidNotReceive()
```

## Limitations:
* This will work best on well formatted code
* There is no replacement for `new Mock<ClassToMock>() { callbase = true; }`
* Replacement Would be `Substitute.ForPartOf<ClassToMock>`
* `{ callbase = true; }` should be removed
* There is no replacment for `.VerifyAll` or `.Protected`
* Some of the replaced statements no longer have the correct indentation, they are left justified, this could be fixed in Visual Studio by reformatting the document
