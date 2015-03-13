﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class ByteUtilities {

	public static byte[] ReadBytes(FileStream file, int numBytes)
	{
		if (file == null) {
			return null;
		}
		
		byte[] buffer = new byte[numBytes];
		
		for (int i=0; i<buffer.Length; i++) {
			buffer[i] = 0;
		}
		
		int bytesRead = file.Read (buffer, 1, numBytes);
		
		if(bytesRead < numBytes)
		{
			byte[] tempBuffer = new byte[bytesRead];
			for(int i=0; i<tempBuffer.Length; i++)
			{
				tempBuffer[i] = buffer[i];
			}
			buffer = tempBuffer;
		}
		
		return buffer;
	}
	
	public static byte[] ReadBytesOrDie(FileStream file, int numBytes)
	{
		byte[] bytes = ReadBytes (file, numBytes);
		if (bytes != null && bytes.Length < numBytes) {
			return null;
		} else {
			return bytes;
		}
	}
	
	public static FileStream OpenOrDie(string filePath, FileMode mode)
	{
		return File.Open(filePath, mode, FileAccess.ReadWrite);
	}
	
	public static void SeekOrDie(FileStream file, long offset, SeekOrigin origin)
	{
		file.Seek (offset, origin);
	}
	
	public static long TellOrDie(FileStream file)
	{
		long seekPosition = file.Position;
		return seekPosition;
	}


//Generic method
	public byte Read(FileStream fs, byte obj, ref bool eofOut)
	{
		byte[] bytes = ReadBytes (fs, sizeof(byte));
		if (bytes.Length == 0) {
			eofOut = true;
			return obj;
		} else if (bytes.Length < sizeof(byte)) {
			eofOut = true;
			return obj;
		} else {
			eofOut = false;
			return (byte) bytes[0];
		}
	}

	public Int32 Read(FileStream fs, Int32 obj, ref bool eofOut)
	{
		byte[] bytes = ReadBytes (fs, sizeof(Int32));
		if (bytes.Length == 0) {
			eofOut = true;
			return obj;
		} else if (bytes.Length < sizeof(Int32)) {
			eofOut = true;
			return obj;
		} else {
			eofOut = false;
			return (Int32) BitConverter.ToInt32(bytes, 0);
		}
	}

	public UInt32 Read(FileStream fs, UInt32 obj, ref bool eofOut)
	{
		byte[] bytes = ReadBytes (fs, sizeof(UInt32));
		if (bytes.Length == 0) {
			eofOut = true;
			return obj;
		} else if (bytes.Length < sizeof(UInt32)) {
			eofOut = true;
			return obj;
		} else {
			eofOut = false;
			return (UInt32)  BitConverter.ToUInt32(bytes, 0);
		}
	}

	public Int64 Read(FileStream fs, Int64 obj, ref bool eofOut)
	{
		byte[] bytes = ReadBytes (fs, sizeof(Int64));
		if (bytes.Length == 0) {
			eofOut = true;
			return obj;
		} else if (bytes.Length < sizeof(Int64)) {
			eofOut = true;
			return obj;
		} else {
			eofOut = false;
			return (Int64) BitConverter.ToInt64(bytes, 0);
		}
	}

	public UInt64 Read(FileStream fs, UInt64 obj, ref bool eofOut)
	{
		byte[] bytes = ReadBytes (fs, sizeof(UInt64));
		if (bytes.Length == 0) {
			eofOut = true;
			return obj;
		} else if (bytes.Length < sizeof(UInt64)) {
			eofOut = true;
			return obj;
		} else {
			eofOut = false;
			return (UInt64)  BitConverter.ToUInt64(bytes, 0);
		}
	}

	public void WriteOrDie(FileStream fs, Int32 data)
	{
		byte[] arrayData = BitConverter.GetBytes (data);
		if (BitConverter.IsLittleEndian) {
			Array.Reverse(arrayData);
		}
		fs.Write (arrayData, 0, 1);
	}

	public void WriteOrDie(FileStream fs, Int64 data)
	{
		byte[] arrayData = BitConverter.GetBytes (data);
		if (BitConverter.IsLittleEndian) {
			Array.Reverse(arrayData);
		}
		fs.Write (arrayData, 0, 1);
	}


	public void AppendBytes(ref byte[] vec, Int32 data)
	{
		byte[] arrayData = BitConverter.GetBytes (data);
		if (BitConverter.IsLittleEndian) {
			Array.Reverse(arrayData);
		}
		List<byte> byteList = new List<byte> ();
		byteList.AddRange (vec);
		for (int i=0; i<arrayData.Length; i++) {
			byteList.Add(arrayData[i]);
		}
		vec = byteList.ToArray ();
	}

	public void AppendBytes(ref byte[] vec, Int64 data)
	{
		byte[] arrayData = BitConverter.GetBytes (data);
		if (BitConverter.IsLittleEndian) {
			Array.Reverse(arrayData);
		}
		List<byte> byteList = new List<byte> ();
		byteList.AddRange (vec);
		for (int i=0; i<arrayData.Length; i++) {
			byteList.Add(arrayData[i]);
		}
		vec = byteList.ToArray ();
	}

	public bool CheckBit(byte number, int bitIndex)
	{
		return (number & (1 << bitIndex)) != 0;
	}
}

/*
 	Thanks to Greg Najda for C++ equivalent
 	https://github.com/LHCGreg/itgoggpatch
*/
