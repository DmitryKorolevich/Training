// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.EntityFrameworkCore.ValueGeneration.Internal
{
    public class TemporaryBinaryValueGenerator : ValueGenerator<byte[]>
    {
        public override byte[] Next() => Guid.NewGuid().ToByteArray();

        public override bool GeneratesTemporaryValues => true;
    }
}
