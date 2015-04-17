using UnityEngine;
using System.Collections;

public enum Difficulty{
	BEGINNER, 
	EASY, 
	MEDIUM, 
	HARD, 
	EXPERT,
	EDIT,
	NONE
}

public enum Judge{
		BEGINNER,
		EASY,
		NORMAL,
		HARD,
		EXPERT
}

public enum Sort{
	NAME,
	STARTWITH,
	ARTIST,
	STEPARTIST,
	DIFFICULTY,
	BPM
}

public enum ScoreCount{
		FANTASTIC,
		EXCELLENT,
		GREAT,
		DECENT,
		WAYOFF,
		MISS,
		FREEZE,
		ROLL,
		JUMPS,
		HANDS,
		MINE,
		NONE
}

public enum Precision{
		FANTASTIC,
		EXCELLENT,
		GREAT,
		DECENT,
		WAYOFF,
		MISS,
		FREEZE,
		UNFREEZE,
		MINE,
		NONE
}

public enum ScoreNote
{
	QUAD,
	GOLD,
	SILVER,
	BRONZE,
	SPLUS,
	S,
	SMINUS,
	APLUS,
	A,
	AMINUS,
	BPLUS,
	B,
	BMINUS,
	CPLUS,
	C,
	CMINUS,
	BAD,
	FAILED
}

public enum ArrowType{
		NORMAL,
		FREEZE,
		ROLL,
		MINE
}

public enum ComboType{
		FULLFANTASTIC,
		FULLEXCELLENT,
		FULLCOMBO,
		NONE
}


public enum ArrowPosition{
	RIGHT,
	LEFT,
	UP,
	DOWN
}

public enum PrecParticle{
		FANTASTIC,
		FANTASTICC,
		EXCELLENT,
		EXCELLENTC,
		GREAT,
		GREATC,
		DECENT,
		WAYOFF,
		FREEZE,
		MINE
}

public enum OptionsMod
{
	NOMINES,
	NOJUMPS,
	NOHANDS,
	NOFREEZE,
	NOROLLS,
	ROLLTOFREEZE,
	NOJUDGE,
	NOBACKGROUND,
	NOTARGET,
	NOSCORE,
	NOUI

}

public enum ArrowState
{
	NONE,
	WAITINGLINKED,
	VALIDATED,
	MISSED
}

public enum Language
{
	FR,
	EN
}
