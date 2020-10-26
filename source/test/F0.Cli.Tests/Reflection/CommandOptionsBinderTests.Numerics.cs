using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using F0.Cli;
using F0.Reflection;
using F0.Tests.Commands;
using Xunit;

namespace F0.Tests.Reflection
{
	public partial class CommandOptionsBinderTests
	{
		[Fact]
		public void OptionsWithValuesMayBeBoundToNumericPropertiesWherePropertyNameMatchesOptionKey_Positive()
		{
			var command = new NumericCommand();
			CommandLineArguments args = CreateArgs("sbyte", "111", "byte", "222", "int16", "333", "uint16", "444", "int32", "555", "uint32", "666", "int64", "777", "uint64", "888", "single", "1.1", "double", "2.2", "decimal", "3.3", "bigint", "999");

			Assert.Equal(default, command.SByte);
			Assert.Equal(default, command.Byte);
			Assert.Equal(default, command.Int16);
			Assert.Equal(default, command.UInt16);
			Assert.Equal(default, command.Int32);
			Assert.Equal(default, command.UInt32);
			Assert.Equal(default, command.Int64);
			Assert.Equal(default, command.UInt64);
			Assert.Equal(default, command.Single);
			Assert.Equal(default, command.Double);
			Assert.Equal(default, command.Decimal);
			Assert.Equal(default, command.BigInt);

			CommandOptionsBinder.BindOptions(command, args);

			Assert.Equal((sbyte)111, command.SByte);
			Assert.Equal((byte)222, command.Byte);
			Assert.Equal((short)333, command.Int16);
			Assert.Equal((ushort)444, command.UInt16);
			Assert.Equal(555, command.Int32);
			Assert.Equal(666u, command.UInt32);
			Assert.Equal(777L, command.Int64);
			Assert.Equal(888ul, command.UInt64);
			Assert.Equal(1.1f, command.Single);
			Assert.Equal(2.2, command.Double);
			Assert.Equal(3.3m, command.Decimal);
			Assert.Equal(new BigInteger(999), command.BigInt);
		}

		[Fact]
		public void OptionsWithValuesMayBeBoundToNumericPropertiesWherePropertyNameMatchesOptionKey_Negative()
		{
			var command = new NumericCommand();
			CommandLineArguments args = CreateArgs("sbyte", "-111", "int16", "-333", "int32", "-555", "int64", "-777", "single", "-1.1", "double", "-2.2", "decimal", "-3.3", "bigint", "-999");

			Assert.Equal(default, command.SByte);
			Assert.Equal(default, command.Int16);
			Assert.Equal(default, command.Int32);
			Assert.Equal(default, command.Int64);
			Assert.Equal(default, command.Single);
			Assert.Equal(default, command.Double);
			Assert.Equal(default, command.Decimal);
			Assert.Equal(default, command.BigInt);

			CommandOptionsBinder.BindOptions(command, args);

			Assert.Equal((sbyte)-111, command.SByte);
			Assert.Equal((short)-333, command.Int16);
			Assert.Equal(-555, command.Int32);
			Assert.Equal(-777L, command.Int64);
			Assert.Equal(-1.1f, command.Single);
			Assert.Equal(-2.2, command.Double);
			Assert.Equal(-3.3m, command.Decimal);
			Assert.Equal(new BigInteger(-999), command.BigInt);
		}

		[Theory]
		[MemberData(nameof(GetNegativeUnsignedIntegerTestData))]
		public void CannotBindNegativeIntegerValueToUnsignedPropertyType(string option, string value)
		{
			var command = new NumericCommand();
			CommandLineArguments args = CreateArgs(option, value);

			Exception exception = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, args));
			Assert.IsType<FormatException>(exception.InnerException);
		}

		[Theory]
		[MemberData(nameof(GetOverflowMaximumTestData))]
		public void Overflow_Maximum(string option, string value)
		{
			var command = new NumericCommand();
			CommandLineArguments args = CreateArgs(option, value);

			Exception exception = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, args));
			Assert.IsType<OverflowException>(exception.InnerException);
		}

		[Theory]
		[MemberData(nameof(GetOverflowMinimumTestData))]
		public void Overflow_Minimum(string option, string value)
		{
			var command = new NumericCommand();
			CommandLineArguments args = CreateArgs(option, value);

			Exception exception = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, args));
			Assert.IsType<OverflowException>(exception.InnerException);
		}

		[Fact]
		public void Overflow_Maximum_FloatingPoint()
		{
			string maxSingle = Single.MaxValue.ToString("F", NumberFormatInfo.InvariantInfo);
			string maxDouble = Double.MaxValue.ToString("F", NumberFormatInfo.InvariantInfo);

			var command = new NumericCommand();

#if IS_IEEE_754_2008_COMPLIANT
			CommandLineArguments args = CreateArgs("single", $"1{maxSingle}", "double", $"1{maxDouble}");

			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal(Single.PositiveInfinity, command.Single);
			Assert.Equal(Double.PositiveInfinity, command.Double);
#else
			Exception binary32 = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, CreateArgs("single", $"1{maxSingle}")));
			Exception binary64 = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, CreateArgs("double", $"1{maxDouble}")));
			Assert.IsType<OverflowException>(binary32.InnerException);
			Assert.IsType<OverflowException>(binary64.InnerException);
#endif
		}

		[Fact]
		public void Overflow_Minimum_FloatingPoint()
		{
			string minSingle = Single.MinValue.ToString("F", NumberFormatInfo.InvariantInfo);
			string minDouble = Double.MinValue.ToString("F", NumberFormatInfo.InvariantInfo);

			var command = new NumericCommand();

#if IS_IEEE_754_2008_COMPLIANT
			CommandLineArguments args = CreateArgs("single", minSingle.Insert(1, "1"), "double", minDouble.Insert(1, "1"));

			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal(Single.NegativeInfinity, command.Single);
			Assert.Equal(Double.NegativeInfinity, command.Double);
#else
			Exception binary32 = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, CreateArgs("single", minSingle.Insert(1, "1"))));
			Exception binary64 = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, CreateArgs("double", minDouble.Insert(1, "1"))));
			Assert.IsType<OverflowException>(binary32.InnerException);
			Assert.IsType<OverflowException>(binary64.InnerException);
#endif
		}

		[Theory]
		[MemberData(nameof(GetThousandsSeparatorTestData))]
		public void Disallow_ThousandsSeparators(string option, string value)
		{
			var command = new NumericCommand();
			CommandLineArguments args = CreateArgs(option, value);

			Exception exception = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, args));
			Assert.IsType<FormatException>(exception.InnerException);
		}

		public static IEnumerable<object[]> GetNegativeUnsignedIntegerTestData()
		{
			return new object[][]
			{
				new object[] { "byte", "-222" },
				new object[] { "uint16", "-444" },
				new object[] { "uint32", "-666" },
				new object[] { "uint64", "-888" }
			};
		}

		public static IEnumerable<object[]> GetOverflowMaximumTestData()
		{
			return new object[][]
			{
				new object[] { "sbyte", $"{SByte.MaxValue}0" },
				new object[] { "byte", $"{Byte.MaxValue}0" },
				new object[] { "int16", $"{Int16.MaxValue}0" },
				new object[] { "uint16", $"{UInt16.MaxValue}0" },
				new object[] { "int32", $"{Int32.MaxValue}0" },
				new object[] { "uint32", $"{UInt32.MaxValue}0" },
				new object[] { "int64", $"{Int64.MaxValue}0" },
				new object[] { "uint64", $"{UInt64.MaxValue}0" },
				new object[] { "decimal", $"1{Decimal.MaxValue.ToString(NumberFormatInfo.InvariantInfo)}" },
			};
		}

		public static IEnumerable<object[]> GetOverflowMinimumTestData()
		{
			return new object[][]
			{
				new object[] { "sbyte", $"{SByte.MinValue}0" },
				new object[] { "int16", $"{Int16.MinValue}0" },
				new object[] { "int32", $"{Int32.MinValue}0" },
				new object[] { "int64", $"{Int64.MinValue}0" },
				new object[] { "decimal", $"{Decimal.MinValue.ToString(NumberFormatInfo.InvariantInfo).Insert(1, "1")}" },
			};
		}

		public static IEnumerable<object[]> GetThousandsSeparatorTestData()
		{
			return new object[][]
			{
				new object[] { "int16", "-32,768" },
				new object[] { "int16", "32,767" },
				new object[] { "uint16", "65,535" },
				new object[] { "int32", "-2,147,483,648" },
				new object[] { "int32", "2,147,483,647" },
				new object[] { "uint32", "4,294,967,295" },
				new object[] { "int64", "-9,223,372,036,854,775,808" },
				new object[] { "int64", "9,223,372,036,854,775,807" },
				new object[] { "uint64", "18,446,744,073,709,551,615" },
				new object[] { "single", "-1,000.0" },
				new object[] { "single", "1,000.0" },
				new object[] { "double", "-1,000.0" },
				new object[] { "double", "1,000.0" },
				new object[] { "decimal", "-1,000.0" },
				new object[] { "decimal", "1,000.0" },
				new object[] { "bigint", "-1,000" },
				new object[] { "bigint", "1,000" },
			};
		}
	}
}
