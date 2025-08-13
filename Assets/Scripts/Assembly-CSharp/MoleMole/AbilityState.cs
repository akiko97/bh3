using System;

namespace MoleMole
{
	[Flags]
	public enum AbilityState
	{
		None = 0,
		Invincible = 1,
		Limbo = 2,
		WitchTimeSlowed = 4,
		Bleed = 8,
		Stun = 0x10,
		Paralyze = 0x20,
		Burn = 0x40,
		Poisoned = 0x80,
		Frozen = 0x100,
		MoveSpeedDown = 0x200,
		AttackSpeedDown = 0x400,
		Weak = 0x800,
		Fragile = 0x1000,
		Endure = 0x2000,
		MoveSpeedUp = 0x4000,
		AttackSpeedUp = 0x8000,
		PowerUp = 0x10000,
		Shielded = 0x20000,
		CritUp = 0x40000,
		Immune = 0x80000,
		MaxMoveSpeed = 0x100000,
		TargetLocked = 0x200000,
		Tied = 0x400000,
		BlockAnimEventAttack = 0x800000,
		Undamagable = 0x1000000,
		ReflectBullet = 0x2000000,
		SlowWhenFrozenOrParalyze = 0x4000000,
		Count = 0x1B
	}
}
