using System;
using System.Collections;
using Xiph.LowLevel;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

public class Ogglength {

	public OggVorbis_File openOggVorbisFile(string filePath)
	{
		OggVorbis_File oggfile = new OggVorbis_File ();
		NativeMethods.ov_fopen (Marshal.StringToHGlobalAuto(filePath), ref oggfile);
		return oggfile;
	}

	public double getReportedTime(string filePath)
	{
		OggVorbis_File oggfile = openOggVorbisFile (filePath);
		double reportedTime = NativeMethods.ov_time_total (ref oggfile, -1);
		if (reportedTime != NativeConstants.OV_EINVAL) {
			return reportedTime;
		}
		return -1;
	}

	public double getRealTime(string filePath)
	{
		OggVorbis_File oggfile = openOggVorbisFile (filePath);
		double totalTimeRead = 0;

		byte[] buffer = new byte[4096];
		IntPtr bufferIntPtr = Marshal.AllocHGlobal(buffer.Length);
		Marshal.Copy (buffer, 0, bufferIntPtr, buffer.Length);

		int logicalBitstreamRead = -555;
		int logicalBitstreamReading = -1;

		long bytesRead = 0;
		int pcmWordSize = 2;


		while ((bytesRead = NativeMethods.ov_read(ref oggfile, bufferIntPtr, 4096, 0, pcmWordSize, 1, ref logicalBitstreamRead)) > 0) {

			if(logicalBitstreamReading == -1)
			{
				logicalBitstreamReading = logicalBitstreamRead;
			}else if(logicalBitstreamReading != logicalBitstreamRead)
			{
				return -1;
			}

			long samplesRead = bytesRead / pcmWordSize;
			vorbis_info info = ((vorbis_info) Marshal.PtrToStructure(NativeMethods.ov_info(ref oggfile, logicalBitstreamRead), typeof(vorbis_info)));
			int numChannels = info.channels;

			if(numChannels <= 0)
			{
				return -1;
			}
			long samplesPerChannel = samplesRead / numChannels;
			double timeRead = (double)samplesPerChannel / info.rate;

			totalTimeRead += timeRead;
		}

		if (bytesRead < 0) {
			return -1;
		}

		return totalTimeRead;
	}

	public void ChangeSongLength(string filePath, double numSeconds)
	{
		FileStream fs = null;
		try{

			fs = ByteUtilities.OpenOrDie(filePath, FileMode.Open);

			byte version;
			byte headerType;
			UInt32 sampleRate = 0;
			Int32 savedBitstreamSerialNumber = 0;
			bool eof = false;

			while(true)
			{
				byte[] buffer = ByteUtilities.ReadBytesOrDie(ref fs, 4);
				if(buffer[0] != 'O' || buffer[1] != 'g' || buffer[2] != 'g' || buffer[3] != 'S')
				{
					return;
				}

				version = ByteUtilities.ReadOrDieByte(ref fs, ref eof);
				if(eof) return;
				if(version != 0) return;

				headerType = ByteUtilities.ReadOrDieByte(ref fs, ref eof);
				if(eof) return;
				bool continuation = ByteUtilities.CheckBit(headerType, 0);
				bool beginningOfStream = ByteUtilities.CheckBit(headerType, 1);
				bool endOfStream = ByteUtilities.CheckBit(headerType, 2);

				if(endOfStream)
				{
					break;
				}

				Int64 granulePositionWhile = ByteUtilities.ReadOrDieByteInt64(ref fs, ref eof);
				if(eof) return;

				Int32 bitstreamSerialNumberWhile = ByteUtilities.ReadOrDieByteInt32(ref fs, ref eof);
				if(eof) return;

				if(sampleRate != 0 && bitstreamSerialNumberWhile != savedBitstreamSerialNumber)
				{
					return;
				}
				savedBitstreamSerialNumber = bitstreamSerialNumberWhile;

				Int32 pageSequenceNumberWhile = ByteUtilities.ReadOrDieByteInt32(ref fs, ref eof);
				if(eof) return;
				Int32 checksumWhile = ByteUtilities.ReadOrDieByteInt32(ref fs, ref eof);
				if(eof) return;
				byte numSegmentsWhile = ByteUtilities.ReadOrDieByte(ref fs, ref eof);
				if(eof) return;

				if(sampleRate == 0)
				{
					byte[] segmentSizesWhile = ByteUtilities.ReadBytesOrDie(ref fs, numSegmentsWhile);
					UInt32 vorbisHeaderPacketSize = 0;
					for(int segIndex = 0; segIndex < numSegmentsWhile; segIndex++)
					{
						vorbisHeaderPacketSize += segmentSizesWhile[segIndex];
						if(segmentSizesWhile[segIndex] < 255)
						{
							break;
						}
					}

					if(vorbisHeaderPacketSize < 16)
					{
						return;
					}

					byte packetType = ByteUtilities.ReadOrDieByte(ref fs, ref eof);
					if(eof) return;

					if(packetType != 1)
					{
						return;
					}

					byte[] vorbisString = ByteUtilities.ReadBytesOrDie(ref fs, 6);
					if(vorbisString[0] != 'v' || vorbisString[1] != 'o' || vorbisString[2] != 'r'
					   || vorbisString[3] != 'b' || vorbisString[4] != 'i' || vorbisString[5] != 's')
					{
						return;
					}

					UInt32 vorbisVersion = ByteUtilities.ReadOrDieByteUInt32(ref fs, ref eof);
					if(eof) return;

					if(vorbisVersion != 0)
					{
						return;
					}

					byte numChannels = ByteUtilities.ReadOrDieByte(ref fs, ref eof);
					if(eof) return;
					sampleRate = ByteUtilities.ReadOrDieByteUInt32(ref fs, ref eof);
					if(eof) return;

					if(sampleRate == 0)
					{
						return;
					}

					Int32 pageDataSize = 0;
					for(byte segmentIndex = 0; segmentIndex < numSegmentsWhile; segmentIndex++)
					{
						byte segmentSize = segmentSizesWhile[segmentIndex];
						pageDataSize += segmentSize;
					}

					Int32 unreadDataBytes = pageDataSize - 16;
					ByteUtilities.SeekOrDie(ref fs, unreadDataBytes, SeekOrigin.Current);
				}else{
					Int32 pageDataSize = 0;

					for(byte segmentIndex = 0; segmentIndex < numSegmentsWhile; segmentIndex++)
					{
						byte segmentSize = ByteUtilities.ReadOrDieByte(ref fs, ref eof);
						if(eof) return;
						pageDataSize += segmentSize;
					}

					ByteUtilities.SeekOrDie(ref fs, pageDataSize, SeekOrigin.Current);
				}
			}

			if(sampleRate == 0)
			{
				return;
			}

			Int64 numSamples = (Int64)(numSeconds * sampleRate);

			long granulePositionPosition = ByteUtilities.TellOrDie(ref fs);

			Int64 granulePosition = ByteUtilities.ReadOrDieByteInt64(ref fs, ref eof);
			if(eof) return;
			granulePosition = numSamples;

			Int32 bitstreamSerialNumber = ByteUtilities.ReadOrDieByteInt32(ref fs, ref eof);
			if(eof) return;
			Int32 pageSequenceNumber = ByteUtilities.ReadOrDieByteInt32(ref fs, ref eof);
			if(eof) return;

			Int32 checksum = ByteUtilities.ReadOrDieByteInt32(ref fs, ref eof);
			if(eof) return;
			checksum = 0;

			byte numSegments = ByteUtilities.ReadOrDieByte(ref fs, ref eof);
			if(eof) return;

			byte[] segmentSizes = ByteUtilities.ReadBytesOrDie(ref fs, numSegments);

			List<byte> headerBytes = new List<byte>();
			headerBytes.Add(Convert.ToByte ('O'));
			headerBytes.Add(Convert.ToByte ('g'));
			headerBytes.Add(Convert.ToByte ('g'));
			headerBytes.Add(Convert.ToByte ('S'));
			headerBytes.Add(version);
			headerBytes.Add(headerType);

			//Resoudre le problème du toArray
			ByteUtilities.AppendBytes(ref headerBytes, granulePosition);
			ByteUtilities.AppendBytes(ref headerBytes, bitstreamSerialNumber);
			ByteUtilities.AppendBytes(ref headerBytes, pageSequenceNumber);
			ByteUtilities.AppendBytes(ref headerBytes), checksum);
			headerBytes.Add(numSegments);

		}catch(Exception e)
		{
			Console.WriteLine(e.Message);
			if(fs != null)
			{
				fs.Close();
			}
		}
	}
}
/*
 	Thanks to Greg Najda for C++ equivalent
 	https://github.com/LHCGreg/itgoggpatch
*/