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
			{ typeof(nint), new Converter<string, object>(ConvertIntPtr)},
			{ typeof(nuint), new Converter<string, object>(ConvertUIntPtr)},
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

		private static object ConvertIntPtr(string value)
		{
#if HAS_NATIVE_SIZED_INTEGERS
			nint native = IntPtr.Parse(value, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
#else
			nint native = Environment.Is64BitProcess
				? FromInt64(value)
				: FromInt32(value);

			static nint FromInt64(string value)
			{
				long integral = Int64.Parse(value, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
				return (nint)integral;
			}
			static nint FromInt32(string value)
			{
				int integral = Int32.Parse(value, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);
				return (nint)integral;
			}
#endif
			return native;
		}

		private static object ConvertUIntPtr(string value)
		{
#if HAS_NATIVE_SIZED_INTEGERS
			nuint native = UIntPtr.Parse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
#else
			nuint native = Environment.Is64BitProcess
				? FromUInt64(value)
				: FromUInt32(value);

			static nuint FromUInt64(string value)
			{
				ulong integral = UInt64.Parse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
				return (nuint)integral;
			}
			static nuint FromUInt32(string value)
			{
				uint integral = UInt32.Parse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
				return (nuint)integral;
			}
#endif
			return native;
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
