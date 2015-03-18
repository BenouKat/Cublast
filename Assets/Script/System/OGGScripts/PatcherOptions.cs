using System.Collections;
using System.Collections.Generic;

enum PatcherLengthCondition
{
	condition_none,
	condition_equal, 
	condition_greater 
};

public class PatcherOptions {

	PatcherLengthCondition m_lengthConditionType = PatcherLengthCondition.condition_equal;
	double m_lengthCondition = 105;

	public bool LengthMeetsConditions(double reportedSongLength)
	{
		switch (m_lengthConditionType) {
		case PatcherLengthCondition.condition_none:
			return true;
		case PatcherLengthCondition.condition_equal:
			return (reportedSongLength < m_lengthCondition + 0.01) && (reportedSongLength > m_lengthCondition - 0.01);
		case PatcherLengthCondition.condition_greater:
			return reportedSongLength > m_lengthCondition;
		}
		return false;
	}

	public bool FileMeetsConditions(Ogglength oggLength, string filePath)
	{
		if (m_lengthConditionType != PatcherLengthCondition.condition_none) {

			double reportedLength = oggLength.getReportedTime (filePath);
			oggLength.closeOggFile();
			return LengthMeetsConditions (reportedLength);
		} else {
			return true;
		}
	}


}
/*
 	Thanks to Greg Najda for C++ equivalent
 	https://github.com/LHCGreg/itgoggpatch
*/