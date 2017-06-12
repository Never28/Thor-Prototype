using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics{

	#region Input

	public static string Horizontal = "Horizontal";
	public static string Vertical = "Vertical";
	public static string DashHorizontal = "DashHorizontal";
	public static string DashVertical = "DashVertical";
	public static string Jump = "Jump";

	#endregion

	#region Animator

	public static string Moving = "Moving";
	public static string Jumping = "Jumping";
	public static string JumpTrigger = "JumpTrigger";
	public static string VelocityX = "Velocity X";
	public static string VelocityZ = "Velocity Z";

	#endregion

	public static int GetJumpType(JumpType type){
		int r = 0;

		switch (type) {
		case JumpType.land:
			r = 0;
			break;
		case JumpType.jump:
			r = 1;
			break;
		case JumpType.fall:
			r = 2;
			break;
		case JumpType.doubleJump:
			r = 3;
			break;
		default:
			break;			
		}

		return r;
	}
}

public enum JumpType
{
	land, jump, fall, doubleJump
}


