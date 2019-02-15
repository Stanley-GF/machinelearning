﻿using System.Linq;
using Microsoft.Data.DataView;
using Microsoft.ML.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ML.Auto.Test
{
    [TestClass]
    public class PurposeInferenceTests
    {
        [TestMethod]
        public void PurposeInferenceHiddenColumnsTest()
        {
            var context = new MLContext();

            // build basic data view
            var schemaBuilder = new SchemaBuilder();
            schemaBuilder.AddColumn(DefaultColumnNames.Label, BoolType.Instance);
            schemaBuilder.AddColumn(DefaultColumnNames.Features, NumberType.R4);
            var schema = schemaBuilder.GetSchema();
            IDataView data = new EmptyDataView(context, schema);

            // normalize 'Features' column. this has the effect of creating 2 columns named
            // 'Features' in the data view, the first of which gets marked as 'Hidden'
            var normalizer = context.Transforms.Normalize(DefaultColumnNames.Features);
            data = normalizer.Fit(data).Transform(data);

            // infer purposes
            var purposes = PurposeInference.InferPurposes(context, data, DefaultColumnNames.Label);

            Assert.AreEqual(3, purposes.Count());
            Assert.AreEqual(ColumnPurpose.Label, purposes[0].Purpose);
            // assert first 'Features' purpose (hidden column) is Ignore
            Assert.AreEqual(ColumnPurpose.Ignore, purposes[1].Purpose);
            // assert second 'Features' purpose is NumericFeature
            Assert.AreEqual(ColumnPurpose.NumericFeature, purposes[2].Purpose);
        }
    }
}
