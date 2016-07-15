﻿using System.Collections.Generic;
using Glimpse.Core.Framework;
using Glimpse.Core.SerializationConverter;
using Xunit;

namespace Glimpse.Test.Core.SerializationConverter
{
    public class GlimpseMetadataConverterShould
    {
        [Fact]
        public void ConvertAGlimpseMetadataObject()
        {
            var metadata = new GlimpseMetadata();

            var converter = new GlimpseMetadataConverter();

            var obj = converter.Convert(metadata);

            var result = obj as IDictionary<string, object>;

            Assert.NotNull(result);
            Assert.True(result.ContainsKey("version"));
            Assert.True(result.ContainsKey("plugins"));
            Assert.True(result.ContainsKey("resources"));
        }
    }
}