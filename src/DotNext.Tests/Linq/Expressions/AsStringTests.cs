﻿using System;
using System.Text;
using Xunit;

namespace DotNext.Linq.Expressions
{
    public sealed class AsStringTests : Assert
    {
        [Fact]
        public static void IntToString()
        {
            var str = 20.Const().AsString();
            Equal(typeof(int), str.Object.Type);
            Equal(typeof(int), str.Method.DeclaringType);
        }

        [Fact]
        public static void DecimalToString()
        {
            var str = 20M.Const().AsString();
            Equal(typeof(decimal), str.Object.Type);
            Equal(typeof(decimal), str.Method.DeclaringType);
        }

        [Fact]
        public static void ObjectToString()
        {
            var str = new object().Const().AsString();
            Equal(typeof(object), str.Object.Type);
            Equal(typeof(object), str.Method.DeclaringType);
        }

        [Fact]
        public static void StringBuilderToString()
        {
            var str = new StringBuilder("abc").Const().AsString();
            Equal(typeof(StringBuilder), str.Object.Type);
            Equal(typeof(StringBuilder), str.Method.DeclaringType);
        }

        [Fact]
        public static void RandomToString()
        {
            var str = new Random().Const().AsString();
            Equal(typeof(Random), str.Object.Type);
            Equal(typeof(object), str.Method.DeclaringType);
        }
    }
}
