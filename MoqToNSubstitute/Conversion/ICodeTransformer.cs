﻿namespace MoqToNSubstitute.Conversion;

internal interface ICodeTransformer
{
    void Transform(string sourceFilePath, bool transform = false);
}