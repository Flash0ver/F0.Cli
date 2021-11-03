using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class NumericCommand : CommandBase
	{
		public NumericCommand()
		{
		}

		public sbyte SByte { get; set; }
		public byte Byte { get; set; }
		public short Int16 { get; set; }
		public ushort UInt16 { get; set; }
		public int Int32 { get; set; }
		public uint UInt32 { get; set; }
		public long Int64 { get; set; }
		public ulong UInt64 { get; set; }
		public nint IntPtr { get; set; }
		public nuint UIntPtr { get; set; }
		public float Single { get; set; }
		public double Double { get; set; }
		public decimal Decimal { get; set; }
		public BigInteger BigInt { get; set; }

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
