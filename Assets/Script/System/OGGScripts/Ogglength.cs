using System;
using System.Collections;
using Xiph.LowLevel;
using System.Runtime.InteropServices;
using System.IO;

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
				byte[] buffer = ByteUtilities.ReadBytesOrDie(fs, 4);
				if(buffer[0] != 'O' || buffer[1] != 'g' || buffer[2] != 'g' || buffer[3] != 'S')
				{
					return;
				}

				version = ByteUtilities.ReadOrDieByte(fs, ref eof);
				if(eof) return;
				if(version != 0) return;

				headerType = ByteUtilities.ReadOrDieByte(fs, ref eof);
				if(eof) return;
				bool continuation = ByteUtilities.CheckBit(headerType, 0);
				bool beginningOfStream = ByteUtilities.CheckBit(headerType, 1);
				bool endOfStream = ByteUtilities.CheckBit(headerType, 2);

				if(endOfStream)
				{
					break;
				}

				Int64 granulePosition = ByteUtilities.ReadOrDieByteInt64(fs, ref eof);
				if(eof) return;

				Int32 bitstreamSerialNumber = ByteUtilities.ReadOrDieByteInt32(fs, ref eof);
				if(eof) return;

				if(sampleRate != 0 && bitstreamSerialNumber != savedBitstreamSerialNumber)
				{
					return;
				}
				savedBitstreamSerialNumber = bitstreamSerialNumber;

				Int32 pageSequenceNumber = ByteUtilities.ReadOrDieByteInt32(fs, ref eof);
				if(eof) return;

			}


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