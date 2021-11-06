using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using F0.Cli;
using F0.Reflection;
using F0.Tests.Commands;
using Xunit;
using Xunit.Abstractions;

namespace F0.Tests.Reflection
{
	public partial class CommandOptionsBinderTests
	{
		private readonly ITestOutputHelper output;

		public CommandOptionsBinderTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void OptionsWithValuesMayBeBoundToNumericPropertiesWherePropertyNameMatchesOptionKey_Positive()
		{
			NumericCommand command = new();
#if HAS_HALF
			CommandLineArguments args = CreateArgs(("sbyte", "111"), ("byte", "222"), ("int16", "333"), ("uint16", "444"), ("int32", "555"), ("uint32", "666"), ("int64", "777"), ("uint64", "888"), ("intptr", "240"), ("uintptr", "240"), ("bigint", "999"), ("half", "1.1"), ("single", "2.2"), ("double", "3.3"), ("decimal", "4.4"));
#else
			CommandLineArguments args = CreateArgs(("sbyte", "111"), ("byte", "222"), ("int16", "333"), ("uint16", "444"), ("int32", "555"), ("uint32", "666"), ("int64", "777"), ("uint64", "888"), ("intptr", "240"), ("uintptr", "240"), ("bigint", "999"), ("single", "2.2"), ("double", "3.3"), ("decimal", "4.4"));
#endif

			Assert.Equal(default, command.SByte);
			Assert.Equal(default, command.Byte);
			Assert.Equal(default, command.Int16);
			Assert.Equal(default, command.UInt16);
			Assert.Equal(default, command.Int32);
			Assert.Equal(default, command.UInt32);
			Assert.Equal(default, command.Int64);
			Assert.Equal(default, command.UInt64);
			Assert.Equal(default, command.IntPtr);
			Assert.Equal(default, command.UIntPtr);
			Assert.Equal(default, command.BigInt);
#if HAS_HALF
			Assert.Equal(default, command.Half);
#endif
			Assert.Equal(default, command.Single);
			Assert.Equal(default, command.Double);
			Assert.Equal(default, command.Decimal);

			CommandOptionsBinder.BindOptions(command, args);

			Assert.Equal((sbyte)111, command.SByte);
			Assert.Equal((byte)222, command.Byte);
			Assert.Equal((short)333, command.Int16);
			Assert.Equal((ushort)444, command.UInt16);
			Assert.Equal(555, command.Int32);
			Assert.Equal(666u, command.UInt32);
			Assert.Equal(777L, command.Int64);
			Assert.Equal(888ul, command.UInt64);
			Assert.Equal((nint)0x_F0, command.IntPtr);
			Assert.Equal((nuint)0x_F0, command.UIntPtr);
			Assert.Equal(new BigInteger(999), command.BigInt);
#if HAS_HALF
			Assert.Equal((Half)1.1f, command.Half);
#endif
			Assert.Equal(2.2f, command.Single);
			Assert.Equal(3.3, command.Double);
			Assert.Equal(4.4m, command.Decimal);
		}

		[Fact]
		public void OptionsWithValuesMayBeBoundToNumericPropertiesWherePropertyNameMatchesOptionKey_Negative()
		{
			NumericCommand command = new();
#if HAS_HALF
			CommandLineArguments args = CreateArgs(("sbyte", "-111"), ("int16", "-333"), ("int32", "-555"), ("int64", "-777"), ("intptr", "-240"), ("bigint", "-999"), ("half", "-1.1"), ("single", "-2.2"), ("double", "-3.3"), ("decimal", "-4.4"));
#else
			CommandLineArguments args = CreateArgs(("sbyte", "-111"), ("int16", "-333"), ("int32", "-555"), ("int64", "-777"), ("intptr", "-240"), ("bigint", "-999"), ("single", "-2.2"), ("double", "-3.3"), ("decimal", "-4.4"));
#endif

			Assert.Equal(default, command.SByte);
			Assert.Equal(default, command.Int16);
			Assert.Equal(default, command.Int32);
			Assert.Equal(default, command.Int64);
			Assert.Equal(default, command.IntPtr);
			Assert.Equal(default, command.BigInt);
#if HAS_HALF
			Assert.Equal(default, command.Half);
#endif
			Assert.Equal(default, command.Single);
			Assert.Equal(default, command.Double);
			Assert.Equal(default, command.Decimal);

			CommandOptionsBinder.BindOptions(command, args);

			Assert.Equal((sbyte)-111, command.SByte);
			Assert.Equal((short)-333, command.Int16);
			Assert.Equal(-555, command.Int32);
			Assert.Equal(-777L, command.Int64);
			Assert.Equal((nint)(-0x_F0), command.IntPtr);
			Assert.Equal(new BigInteger(-999), command.BigInt);
#if HAS_HALF
			Assert.Equal((Half)(-1.1f), command.Half);
#endif
			Assert.Equal(-2.2f, command.Single);
			Assert.Equal(-3.3, command.Double);
			Assert.Equal(-4.4m, command.Decimal);
		}

		[Theory]
		[MemberData(nameof(GetNegativeUnsignedIntegerTestData))]
		public void CannotBindNegativeIntegerValueToUnsignedPropertyType(string option, string value)
		{
			NumericCommand command = new();
			CommandLineArguments args = CreateArgs((option, value));

			Exception exception = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, args));
			Assert.IsType<FormatException>(exception.InnerException);
		}

		[Theory]
		[MemberData(nameof(GetOverflowMaximumTestData))]
		public void Overflow_Maximum(string option, string value)
		{
			NumericCommand command = new();
			CommandLineArguments args = CreateArgs((option, value));

			Exception exception = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, args));
			Assert.IsType<OverflowException>(exception.InnerException);
		}

		[Theory]
		[MemberData(nameof(GetOverflowMinimumTestData))]
		public void Overflow_Minimum(string option, string value)
		{
			NumericCommand command = new();
			CommandLineArguments args = CreateArgs((option, value));

			Exception exception = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, args));
			Assert.IsType<OverflowException>(exception.InnerException);
		}

		[Fact]
		public void Overflow_Maximum_FloatingPoint()
		{
			string maxSingle = Single.MaxValue.ToString("F", NumberFormatInfo.InvariantInfo);
			string maxDouble = Double.MaxValue.ToString("F", NumberFormatInfo.InvariantInfo);

			NumericCommand command = new();

#if IS_IEEE_754_2008_COMPLIANT
			CommandLineArguments args = CreateArgs(("single", $"1{maxSingle}"), ("double", $"1{maxDouble}"));

			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal(Single.PositiveInfinity, command.Single);
			Assert.Equal(Double.PositiveInfinity, command.Double);
#else
			Exception binary32 = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, CreateArgs(("single", $"1{maxSingle}"))));
			Exception binary64 = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, CreateArgs(("double", $"1{maxDouble}"))));
			Assert.IsType<OverflowException>(binary32.InnerException);
			Assert.IsType<OverflowException>(binary64.InnerException);
#endif
		}

		[Fact]
		public void Overflow_Minimum_FloatingPoint()
		{
			string minSingle = Single.MinValue.ToString("F", NumberFormatInfo.InvariantInfo);
			string minDouble = Double.MinValue.ToString("F", NumberFormatInfo.InvariantInfo);

			NumericCommand command = new();

#if IS_IEEE_754_2008_COMPLIANT
			CommandLineArguments args = CreateArgs(("single", minSingle.Insert(1, "1")), ("double", minDouble.Insert(1, "1")));

			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal(Single.NegativeInfinity, command.Single);
			Assert.Equal(Double.NegativeInfinity, command.Double);
#else
			Exception binary32 = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, CreateArgs(("single", minSingle.Insert(1, "1")))));
			Exception binary64 = Assert.Throws<CommandOptionBindingException>(() => CommandOptionsBinder.BindOptions(command, CreateArgs(("double", minDouble.Insert(1, "1")))));
			Assert.IsType<OverflowException>(binary32.InnerException);
			Assert.IsType<OverflowException>(binary64.InnerException);
#endif
		}

		[Fact]
		public void Overflow_Half()
		{
#if HAS_HALF
			string maxHalf = Half.MaxValue.ToString("F", NumberFormatInfo.InvariantInfo);
			string minHalf = Half.MinValue.ToString("F", NumberFormatInfo.InvariantInfo);

			NumericCommand maximum = new();
			NumericCommand minimum = new();

			CommandLineArguments max = CreateArgs(("half", $"1{maxHalf}"));
			CommandLineArguments min = CreateArgs(("half", minHalf.Insert(1, "1")));

			CommandOptionsBinder.BindOptions(maximum, max);
			CommandOptionsBinder.BindOptions(minimum, min);

			Assert.Equal(Half.PositiveInfinity, maximum.Half);
			Assert.Equal(Half.NegativeInfinity, minimum.Half);
#else
			output.WriteLine("'Half' is available in .NET 5.0 or greater.");
#endif
		}

		[Theory]
		[MemberData(nameof(GetThousandsSeparatorTestData))]
		public void Disallow_ThousandsSeparators(string option, string value)
		{
			NumericCommand command = new();
			CommandLineArguments args = CreateArgs((option, value));

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
				new object[] { "uint64", "-888" },
				new object[] { "uintptr", "-240" },
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
				new object[] { "intptr", $"{Get_IntPtr_MaxValue()}0" },
				new object[] { "uintptr", $"{Get_UIntPtr_MaxValue()}0" },
				new object[] { "decimal", $"1{Decimal.MaxValue.ToString(NumberFormatInfo.InvariantInfo)}" },
			};

			static nint Get_IntPtr_MaxValue()
			{
#if HAS_NATIVE_SIZED_INTEGERS
				return IntPtr.MaxValue;
#else
				return Environment.Is64BitProcess
					? (nint)Int64.MaxValue
					: (nint)Int32.MaxValue;
#endif
			}

			static nuint Get_UIntPtr_MaxValue()
			{
#if HAS_NATIVE_SIZED_INTEGERS
				return UIntPtr.MaxValue;
#else
				return Environment.Is64BitProcess
					? (nuint)UInt64.MaxValue
					: (nuint)UInt32.MaxValue;
#endif
			}
		}

		public static IEnumerable<object[]> GetOverflowMinimumTestData()
		{
			return new object[][]
			{
				new object[] { "sbyte", $"{SByte.MinValue}0" },
				new object[] { "int16", $"{Int16.MinValue}0" },
				new object[] { "int32", $"{Int32.MinValue}0" },
				new object[] { "int64", $"{Int64.MinValue}0" },
				new object[] { "intptr", $"{Get_IntPtr_MinValue()}0" },
				new object[] { "decimal", $"{Decimal.MinValue.ToString(NumberFormatInfo.InvariantInfo).Insert(1, "1")}" },
			};

			static nint Get_IntPtr_MinValue()
			{
#if HAS_NATIVE_SIZED_INTEGERS
				return IntPtr.MinValue;
#else
				return Environment.Is64BitProcess
					? (nint)Int64.MinValue
					: (nint)Int32.MinValue;
#endif
			}
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
				new object[] { "intptr", Get_IntPtr_MinValue() },
				new object[] { "intptr", Get_IntPtr_MaxValue() },
				new object[] { "uintptr", Get_UIntPtr_MaxValue() },
				new object[] { "bigint", "-1,000" },
				new object[] { "bigint", "1,000" },
#if HAS_HALF
				new object[] { "half", "-1,000.0" },
				new object[] { "half", "1,000.0" },
#endif
				new object[] { "single", "-1,000.0" },
				new object[] { "single", "1,000.0" },
				new object[] { "double", "-1,000.0" },
				new object[] { "double", "1,000.0" },
				new object[] { "decimal", "-1,000.0" },
				new object[] { "decimal", "1,000.0" },
			};

			static string Get_IntPtr_MinValue()
			{
				return Environment.Is64BitProcess
					? "-9,223,372,036,854,775,808"
					: "-2,147,483,648";
			}

			static string Get_IntPtr_MaxValue()
			{
				return Environment.Is64BitProcess
					? "9,223,372,036,854,775,807"
					: "2,147,483,647";
			}

			static string Get_UIntPtr_MaxValue()
			{
				return Environment.Is64BitProcess
					? "18,446,744,073,709,551,615"
					: "4,294,967,295";
			}
		}
	}
}
