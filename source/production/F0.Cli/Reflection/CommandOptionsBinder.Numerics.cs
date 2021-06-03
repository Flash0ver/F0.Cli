using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace F0.Reflection
{
	internal static partial class CommandOptionsBinder
	{
		private static readonly IReadOnlyDictionary<Type, Converter<string, object>> converters = new Dictionary<Type, Converter<string, object>>
		{
			{ typeof(sbyte), new Converter<string, object>(ConvertSByte)},
			{ typeof(byte), new Converter<string, object>(ConvertByte)},
			{ typeof(short), new Converter<string, object>(ConvertInt16)},
			{ typeof(ushort), new Converter<string, object>(ConvertUInt16)},
			{ typeof(int), new Converter<string, object>(ConvertInt32)},
			{ typeof(uint), new Converter<string, object>(ConvertUInt32)},
			{ typeof(long), new Converter<string, object>(ConvertInt64)},
			{ typeof(ulong), new Converter<string, object>(ConvertUInt64)},
			{ typeof(float), new Converter<string, object>(ConvertSingle)},
			{ typeof(double), new Converter<string, object>(ConvertDouble)},
			{ typeof(decimal), new Converter<string, object>(ConvertDecimal)},
			{ typeof(BigInteger), new Converter<string, object>(ConvertBigInteger) },
		};

		private static object ConvertSByte(string value)
		{
			sbyte integral = SByte.Parse(value, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
			return integral;
		}

		private static object ConvertByte(string value)
		{
			byte integral = Byte.Parse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
			return integral;
		}

		private static object ConvertInt16(string value)
		{
			short integral = Int16.Parse(value, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
			return integral;
		}

		private static object ConvertUInt16(string value)
		{
			ushort integral = UInt16.Parse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
			return integral;
		}

		private static object ConvertInt32(string value)
		{
			int integral = Int32.Parse(value, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
			return integral;
		}

		private static object ConvertUInt32(string value)
		{
			uint integral = UInt32.Parse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
			return integral;
		}

		private static object ConvertInt64(string value)
		{
			long integral = Int64.Parse(value, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
			return integral;
		}

		private static object ConvertUInt64(string value)
		{
			ulong integral = UInt64.Parse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
			return integral;
		}

		private static object ConvertSingle(string value)
		{
			float real = Single.Parse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo);
			return real;
		}

		private static object ConvertDouble(string value)
		{
			double real = Double.Parse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo);
			return real;
		}

		private static object ConvertDecimal(string value)
		{
			decimal real = Decimal.Parse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo);
			return real;
		}

		private static object ConvertBigInteger(string value)
		{
			BigInteger integral = BigInteger.Parse(value, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
			return integral;
		}
	}
}
