﻿using System;
using System.IO;
using System.Text;

namespace SimpleCL.SilkroadSecurityApi
{
	public class Packet
	{
		private ushort m_opcode;
		private PacketWriter m_writer;
		private PacketReader m_reader;
		private bool m_encrypted;
		private bool m_massive;
		private bool m_locked;
		byte[] m_reader_bytes;
		object m_lock;

		public ushort Opcode
		{
			get { return m_opcode; }
		}
		public bool Encrypted
		{
			get { return m_encrypted; }
		}
		public bool Massive
		{
			get { return m_massive; }
		}

		public Packet(Packet rhs)
		{
			lock (rhs.m_lock)
			{
				m_lock = new object();

				m_opcode = rhs.m_opcode;
				m_encrypted = rhs.m_encrypted;
				m_massive = rhs.m_massive;

				m_locked = rhs.m_locked;
				if (!m_locked)
				{
					m_writer = new PacketWriter();
					m_reader = null;
					m_reader_bytes = null;
					m_writer.Write(rhs.m_writer.GetBytes());
				}
				else
				{
					m_writer = null;
					m_reader_bytes = rhs.m_reader_bytes;
					m_reader = new PacketReader(m_reader_bytes);
				}
			}
		}
		public Packet(ushort opcode)
		{
			m_lock = new object();
			m_opcode = opcode;
			m_encrypted = false;
			m_massive = false;
			m_writer = new PacketWriter();
			m_reader = null;
			m_reader_bytes = null;
		}
		public Packet(ushort opcode, bool encrypted)
		{
			m_lock = new object();
			m_opcode = opcode;
			m_encrypted = encrypted;
			m_massive = false;
			m_writer = new PacketWriter();
			m_reader = null;
			m_reader_bytes = null;
		}
		public Packet(ushort opcode, bool encrypted, bool massive)
		{
			if (encrypted && massive)
			{
				throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
			}
			m_lock = new object();
			m_opcode = opcode;
			m_encrypted = encrypted;
			m_massive = massive;
			m_writer = new PacketWriter();
			m_reader = null;
			m_reader_bytes = null;
		}
		public Packet(ushort opcode, bool encrypted, bool massive, byte[] bytes)
		{
			if (encrypted && massive)
			{
				throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
			}
			m_lock = new object();
			m_opcode = opcode;
			m_encrypted = encrypted;
			m_massive = massive;
			m_writer = new PacketWriter();
			m_writer.Write(bytes);
			m_reader = null;
			m_reader_bytes = null;
		}
		public Packet(ushort opcode, bool encrypted, bool massive, byte[] bytes, int offset, int length)
		{
			if (encrypted && massive)
			{
				throw new Exception("[Packet::Packet] Packets cannot both be massive and encrypted!");
			}
			m_lock = new object();
			m_opcode = opcode;
			m_encrypted = encrypted;
			m_massive = massive;
			m_writer = new PacketWriter();
			m_writer.Write(bytes, offset, length);
			m_reader = null;
			m_reader_bytes = null;
		}

		public byte[] GetBytes()
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					return m_reader_bytes;
				}
				return m_writer.GetBytes();
			}
		}

		public void Lock()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					m_reader_bytes = m_writer.GetBytes();
					m_reader = new PacketReader(m_reader_bytes);
					m_writer.Close();
					m_writer = null;
					m_locked = true;
				}
			}
		}

		public long SeekRead(long offset, SeekOrigin orgin)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot SeekRead on an unlocked Packet.");
				}
				return m_reader.BaseStream.Seek(offset, orgin);
			}
		}

		public int RemainingRead()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot SeekRead on an unlocked Packet.");
				}
				return (int)(m_reader.BaseStream.Length - m_reader.BaseStream.Position);
			}
		}
		public bool ReadBool()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadByte() == 1;
			}
		}

		public byte ReadByte()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadByte();
			}
		}
		public sbyte ReadSByte()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadSByte();
			}
		}
		public ushort ReadUShort()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadUInt16();
			}
		}
		public short ReadShort()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadInt16();
			}
		}
		public uint ReadUInt()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadUInt32();
			}
		}
		public int ReadInt()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadInt32();
			}
		}
		public ulong ReadULong()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadUInt64();
			}
		}
		public long ReadLong()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadInt64();
			}
		}
		public float ReadFloat()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadSingle();
			}
		}
		public double ReadDouble()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				return m_reader.ReadDouble();
			}
		}
		public string ReadString(int length, int codepage = 1252)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}

				var bytes = m_reader.ReadBytes(length);
				return Encoding.GetEncoding(codepage).GetString(bytes);
			}
		}
		public string ReadAscii(int codepage = 1252)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}

				var length = m_reader.ReadUInt16();
				var bytes = m_reader.ReadBytes(length);

				return Encoding.GetEncoding(codepage).GetString(bytes);
			}
		}
		public string ReadUnicode()
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}

				var length = m_reader.ReadUInt16();
				var bytes = m_reader.ReadBytes(length * 2);

				return Encoding.Unicode.GetString(bytes);
			}
		}
		public byte[] ReadByteArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new byte[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadByte();
				}
				return values;
			}
		}
		public sbyte[] ReadSByteArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new sbyte[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadSByte();
				}
				return values;
			}
		}
		public ushort[] ReadUShortArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new ushort[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadUInt16();
				}
				return values;
			}
		}
		public short[] ReadShortArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new short[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadInt16();
				}
				return values;
			}
		}
		public uint[] ReadUIntArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new uint[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadUInt32();
				}
				return values;
			}
		}
		public int[] ReadIntArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new int[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadInt32();
				}
				return values;
			}
		}
		public ulong[] ReadULongArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new ulong[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadUInt64();
				}
				return values;
			}
		}
		public long[] ReadLongArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new long[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadInt64();
				}
				return values;
			}
		}
		public float[] ReadFloatArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new float[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadSingle();
				}
				return values;
			}
		}
		public double[] ReadDoubleArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new double[count];
				for (var x = 0; x < count; ++x)
				{
					values[x] = m_reader.ReadDouble();
				}
				return values;
			}
		}
		public string[] ReadAsciiArray(int count)
		{
			return ReadAsciiArray(1252);
		}
		public string[] ReadAsciiArray(int codepage, int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new string[count];
				for (var x = 0; x < count; ++x)
				{
					var length = m_reader.ReadUInt16();
					var bytes = m_reader.ReadBytes(length);
					values[x] = Encoding.UTF7.GetString(bytes);
				}
				return values;
			}
		}
		public string[] ReadUnicodeArray(int count)
		{
			lock (m_lock)
			{
				if (!m_locked)
				{
					throw new Exception("Cannot Read from an unlocked Packet.");
				}
				var values = new string[count];
				for (var x = 0; x < count; ++x)
				{
					var length = m_reader.ReadUInt16();
					var bytes = m_reader.ReadBytes(length * 2);
					values[x] = Encoding.Unicode.GetString(bytes);
				}
				return values;
			}
		}
		public long SeekWrite(long offset, SeekOrigin orgin)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot SeekWrite on a locked Packet.");
				}
				return m_writer.BaseStream.Seek(offset, orgin);
			}
		}
		public void WriteBool(bool value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value ? 1 : 0);
			}
		}
		public void WriteByte(byte value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteSByte(sbyte value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteUShort(ushort value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteShort(short value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteUInt(uint value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteInt(int value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteULong(ulong value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteLong(long value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteSingle(float value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteFloat(float value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteDouble(double value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(value);
			}
		}
		public void WriteAscii(string value, int codepage = 1252)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}

				var codepage_bytes = Encoding.GetEncoding(codepage).GetBytes(value);
				var utf7_value = Encoding.UTF7.GetString(codepage_bytes);
				var bytes = Encoding.Default.GetBytes(utf7_value);

				m_writer.Write((ushort)bytes.Length);
				m_writer.Write(bytes);
			}
		}
		public void WriteUnicode(string value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}

				var bytes = Encoding.Unicode.GetBytes(value);

				m_writer.Write((ushort)value.ToString().Length);
				m_writer.Write(bytes);
			}
		}
		public void WriteByte(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write((byte)(Convert.ToUInt64(value) & 0xFF));
			}
		}
		public void WriteSByte(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write((sbyte)(Convert.ToInt64(value) & 0xFF));
			}
		}
		public void WriteUShort(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write((ushort)(Convert.ToUInt64(value) & 0xFFFF));
			}
		}
		public void WriteShort(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write((short)(Convert.ToInt64(value) & 0xFFFF));
			}
		}
		public void WriteUInt(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write((uint)(Convert.ToUInt64(value) & 0xFFFFFFFF));
			}
		}
		public void WriteInt(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write((int)(Convert.ToInt64(value) & 0xFFFFFFFF));
			}
		}
		public void WriteULong(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(Convert.ToUInt64(value));
			}
		}
		public void WriteLong(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(Convert.ToInt64(value));
			}
		}
		public void WriteSingle(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(Convert.ToSingle(value));
			}
		}
		public void WriteDouble(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				m_writer.Write(Convert.ToDouble(value));
			}
		}
		public void WriteAscii(object value)
		{
			WriteAscii(value, 1252);
		}
		public void WriteAscii(object value, int code_page)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}

				var codepage_bytes = Encoding.GetEncoding(code_page).GetBytes(value.ToString());
				var utf7_value = Encoding.UTF7.GetString(codepage_bytes);
				var bytes = Encoding.Default.GetBytes(utf7_value);

				m_writer.Write((ushort)bytes.Length);
				m_writer.Write(bytes);
			}
		}
		public void WriteUnicode(object value)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}

				var bytes = Encoding.Unicode.GetBytes(value.ToString());

				m_writer.Write((ushort)value.ToString().Length);
				m_writer.Write(bytes);
			}
		}

		public void WriteByteArray(byte[] values)
		{
			if (m_locked)
			{
				throw new Exception("Cannot Write to a locked Packet.");
			}
			m_writer.Write(values);
		}
		public void WriteUInt8Array(byte[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteUInt16Array(UInt16[] values)
		{
			WriteUInt16Array(values, 0, values.Length);
		}
		public void WriteUInt16Array(UInt16[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteInt16Array(Int16[] values)
		{
			WriteInt16Array(values, 0, values.Length);
		}
		public void WriteInt16Array(Int16[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteUInt32Array(UInt32[] values)
		{
			WriteUInt32Array(values, 0, values.Length);
		}
		public void WriteUInt32Array(UInt32[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteInt32Array(Int32[] values)
		{
			WriteInt32Array(values, 0, values.Length);
		}
		public void WriteInt32Array(Int32[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteUInt64Array(UInt64[] values)
		{
			WriteUInt64Array(values, 0, values.Length);
		}
		public void WriteUInt64Array(UInt64[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteInt64Array(long[] values)
		{
			WriteInt64Array(values, 0, values.Length);
		}
		public void WriteInt64Array(long[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteSingleArray(float[] values)
		{
			WriteSingleArray(values, 0, values.Length);
		}
		public void WriteSingleArray(float[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteDoubleArray(double[] values)
		{
			WriteDoubleArray(values, 0, values.Length);
		}
		public void WriteDoubleArray(double[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					m_writer.Write(values[x]);
				}
			}
		}
		public void WriteAsciiArray(String[] values, int codepage)
		{
			WriteAsciiArray(values, 0, values.Length, codepage);
		}
		public void WriteAsciiArray(String[] values, int index, int count, int codepage)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteAscii(values[x], codepage);
				}
			}
		}
		public void WriteAsciiArray(String[] values)
		{
			WriteAsciiArray(values, 0, values.Length, 1252);
		}
		public void WriteAsciiArray(String[] values, int index, int count)
		{
			WriteAsciiArray(values, index, count, 1252);
		}
		public void WriteUnicodeArray(String[] values)
		{
			WriteUnicodeArray(values, 0, values.Length);
		}
		public void WriteUnicodeArray(String[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteUnicode(values[x]);
				}
			}
		}

		public void WriteUInt8Array(object[] values)
		{
			WriteUInt8Array(values, 0, values.Length);
		}
		public void WriteUInt8Array(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteByte(values[x]);
				}
			}
		}
		public void WriteInt8Array(object[] values)
		{
			WriteInt8Array(values, 0, values.Length);
		}
		public void WriteInt8Array(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteSByte(values[x]);
				}
			}
		}
		public void WriteUInt16Array(object[] values)
		{
			WriteUInt16Array(values, 0, values.Length);
		}
		public void WriteUInt16Array(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteUShort(values[x]);
				}
			}
		}
		public void WriteInt16Array(object[] values)
		{
			WriteInt16Array(values, 0, values.Length);
		}
		public void WriteInt16Array(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteShort(values[x]);
				}
			}
		}
		public void WriteUInt32Array(object[] values)
		{
			WriteUInt32Array(values, 0, values.Length);
		}
		public void WriteUInt32Array(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteUInt(values[x]);
				}
			}
		}
		public void WriteInt32Array(object[] values)
		{
			WriteInt32Array(values, 0, values.Length);
		}
		public void WriteInt32Array(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteInt(values[x]);
				}
			}
		}
		public void WriteUInt64Array(object[] values)
		{
			WriteUInt64Array(values, 0, values.Length);
		}
		public void WriteUInt64Array(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteULong(values[x]);
				}
			}
		}
		public void WriteInt64Array(object[] values)
		{
			WriteInt64Array(values, 0, values.Length);
		}
		public void WriteInt64Array(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteLong(values[x]);
				}
			}
		}
		public void WriteSingleArray(object[] values)
		{
			WriteSingleArray(values, 0, values.Length);
		}
		public void WriteSingleArray(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteSingle(values[x]);
				}
			}
		}
		public void WriteDoubleArray(object[] values)
		{
			WriteDoubleArray(values, 0, values.Length);
		}
		public void WriteDoubleArray(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteDouble(values[x]);
				}
			}
		}
		public void WriteAsciiArray(object[] values, int codepage)
		{
			WriteAsciiArray(values, 0, values.Length, codepage);
		}
		public void WriteAsciiArray(object[] values, int index, int count, int codepage)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteAscii(values[x].ToString(), codepage);
				}
			}
		}
		public void WriteAsciiArray(object[] values)
		{
			WriteAsciiArray(values, 0, values.Length, 1252);
		}
		public void WriteAsciiArray(object[] values, int index, int count)
		{
			WriteAsciiArray(values, index, count, 1252);
		}
		public void WriteUnicodeArray(object[] values)
		{
			WriteUnicodeArray(values, 0, values.Length);
		}
		public void WriteUnicodeArray(object[] values, int index, int count)
		{
			lock (m_lock)
			{
				if (m_locked)
				{
					throw new Exception("Cannot Write to a locked Packet.");
				}
				for (var x = index; x < index + count; ++x)
				{
					WriteUnicode(values[x].ToString());
				}
			}
		}
		public static string ToStringHexadecimal(byte[] bytes)
		{
			if (bytes.Length == 0)
				return "";
			var hexData = new StringBuilder();
			foreach (var b in bytes)
				hexData.Append(b.ToString("X2") + " ");
			// remove the last empty space
			return hexData.Remove(hexData.Length - 1, 1).ToString();
		}
		public override string ToString()
		{
			return "[" + Opcode.ToString("X4") + "][" + ToStringHexadecimal(GetBytes()) + "]";
    }
	}
}