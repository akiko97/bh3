 {
 	/*	创建的时候，距离地面高出多少【米】，防止碰撞壳在一开始略微陷入地面，从而导致起步会微小跳跃	*/
	"CreatePosYOffset" : 0.02,
	
	/*	HP配置方式以后肯定会重构这里简单写一下	*/
	"HP" : 200.0,

	/*	与状态机转换相关的基础配置	*/
 	"StateMachinePattern" :
 	{
 		"Parameters" :
 		[
 			/*	在没有任何Buff情况下的恒定移动速度 【米/秒】	*/
 			{
 				"Param" : "ConstMoveSpeed",
 				"Type" : "Float",
 				"Value" : 4.267
 			},
 			/*	跑动动画播放速度为1.0的情况下下的移动速度 【米/秒】	*/
 			{
 				"Param" : "AniMoveSpeed",
 				"Type" : "Float",
 				"Value" : 4.267
 			},
 			/*	跑动动画播放速度最慢是0.1	*/
 			{
 				"Param" : "AniMinSpeedRatio",
 				"Type" : "Float",
 				"Value" : 0.1
 			},
 			/*	跑动动画播放速度最块是1.0	*/
 			{
 				"Param" : "AniMaxSpeedRatio",
 				"Type" : "Float",
 				"Value" : 10
 			},
 			/*	当角色推动怪的时候，推怪速度降为正常移动速度的0.x	*/
 			{
 				"Param" : "PushMonsterSpeedRatio",
 				"Type" : "Float",
 				"Value" : 0.5
 			},
 			/*	角色转换角度Lerp时候乘的参数，越大转的越快	*/
			{
 				"Param" : "ChangeDirLerpRatioForMove",
 				"Type" : "Float",
 				"Value" : 30
 			},
 			/*	角色以等高椭圆选取攻击对象，椭圆的离心率；越大越倾向于选择镜头朝向的怪；椭圆是以角色为焦点，镜头朝向为长轴的椭圆	*/
			{
 				"Param" : "TargetSelectionEccentricity",
 				"Type" : "Float",
 				"Value" : 0.5
 			}
 		]
 	},

	/*	选择攻击对象的方式	*/
 	"AttackTargetSelectPattern" :
 	{
 		"AttackTargetSelectMethod" : "SelectEnemyByEllipse"
 	},

	/*	有几种攻击方式，几连击	*/
	"ATKNumber" : 4,
 	"ATK01" :
 	{
 		"CollisionDetectMethod" : "RectCollisionDetect",
 		"Parameters" :
 		[
 			{
 				"Param" : "IsThrow",
 				"Type" : "Bool",
 				"Value" : "False"
 			},
 			{
 				"Param" : "Damage",
 				"Type" : "Float",
 				"Value" : 5.0
 			},
			{
				"Param" : "AniDamageRatio",
				"Type" : "Float",
				"Value" : 0.5
			},
			{
 				"Param" : "OffsetZ",
 				"Type" : "Float",
 				"Value" : -0.1
 			},
 			{
 				"Param" : "CenterYOffset",
 				"Type" : "Float",
 				"Value" : 0.8
 			},
 			{
 				"Param" : "Width",
 				"Type" : "Float",
 				"Value" : 0.6
 			},
 			{
 				"Param" : "Distance",
 				"Type" : "Float",
 				"Value" : 1.7
 			},
 			{
 				"Param" : "FrameHalt",
 				"Type" : "Int",
 				"Value" : 5
 			},
 			{
 				"Param" : "RetreatVelocity",
 				"Type" : "Float",
 				"Value" : 10.0
 			},
 			{
 				"Param" : "ShakeTime",
 				"Type" : "Float",
 				"Value" : 0.10
 			},
 			{
 				"Param" : "ShakeRange",
 				"Type" : "Float",
 				"Value" : 0.06
 			}
 		]
 	},

 	"ATK02" :
 	{
 		"CollisionDetectMethod" : "FanCollisionDetect",
 		"Parameters" :
 		[
 			{
 				"Param" : "IsThrow",
 				"Type" : "Bool",
 				"Value" : "False"
 			},
 			{
 				"Param" : "Damage",
 				"Type" : "Float",
 				"Value" : 5.0
 			},
			{
				"Param" : "AniDamageRatio",
				"Type" : "Float",
				"Value" : 0.33
			},
 			{
 				"Param" : "CenterYOffset",
 				"Type" : "Float",
 				"Value" : 0.8
 			},
			{
 				"Param" : "OffsetZ",
 				"Type" : "Float",
 				"Value" : -0.1
 			},
 			{
 				"Param" : "Radius",
 				"Type" : "Float",
 				"Value" : 1.5
 			},
 			{
 				"Param" : "FanAngle",
 				"Type" : "Float",
 				"Value" : 150
 			},
			{
 				"Param" : "MeleeRadius",
 				"Type" : "Float",
 				"Value" : 0.8
 			},
 			{
 				"Param" : "MeleeFanAngle",
 				"Type" : "Float",
 				"Value" : 240
 			},
 			{
 				"Param" : "FrameHalt",
 				"Type" : "Int",
 				"Value" : 7
 			},
 			{
 				"Param" : "RetreatVelocity",
 				"Type" : "Float",
 				"Value" : 10.0
 			},
 			{
 				"Param" : "ShakeTime",
 				"Type" : "Float",
 				"Value" : 0.12
 			},
 			{
 				"Param" : "ShakeRange",
 				"Type" : "Float",
 				"Value" : 0.08
 			}
 		]
 	},

 	"ATK03" :
 	{
 		"CollisionDetectMethod" : "FanCollisionDetect",
 		"Parameters" :
 		[
 			{
 				"Param" : "IsThrow",
 				"Type" : "Bool",
 				"Value" : "True"
 			},
 			{
 				"Param" : "Damage",
 				"Type" : "Float",
 				"Value" : 10.0
 			},
			{
				"Param" : "AniDamageRatio",
				"Type" : "Float",
				"Value" : 0.7
			},
 			{
 				"Param" : "CenterYOffset",
 				"Type" : "Float",
 				"Value" : 0.8
 			},
			{
 				"Param" : "OffsetZ",
 				"Type" : "Float",
 				"Value" : -0.1
 			},
 			{
 				"Param" : "Radius",
 				"Type" : "Float",
 				"Value" : 1.2
 			},
 			{
 				"Param" : "FanAngle",
 				"Type" : "Float",
 				"Value" : 90
 			},
			{
 				"Param" : "MeleeRadius",
 				"Type" : "Float",
 				"Value" : 0.8
 			},
 			{
 				"Param" : "MeleeFanAngle",
 				"Type" : "Float",
 				"Value" : 150
 			},
 			{
 				"Param" : "FrameHalt",
 				"Type" : "Int",
 				"Value" : 5
 			},
 			{
 				"Param" : "RetreatVelocity",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "ShakeTime",
 				"Type" : "Float",
 				"Value" : 0.15
 			},
 			{
 				"Param" : "ShakeRange",
 				"Type" : "Float",
 				"Value" : 0.1
 			}
 		]
 	},

 	"ATK04" :
 	{
 		"CollisionDetectMethod" : "CylinderCollisionDetectTargetLocked",
 		"Parameters" :
 		[
 			{
 				"Param" : "IsThrow",
 				"Type" : "Bool",
 				"Value" : "False"
 			},
 			{
 				"Param" : "Damage",
 				"Type" : "Float",
 				"Value" : 15.0
 			},
			{
				"Param" : "AniDamageRatio",
				"Type" : "Float",
				"Value" : 1.0
			},
 			{
 				"Param" : "Radius",
 				"Type" : "Float",
 				"Value" : 1.5
 			},
 			{
 				"Param" : "Height",
 				"Type" : "Float",
 				"Value" : 3.0
 			},
 			{
 				"Param" : "FrameHalt",
 				"Type" : "Int",
 				"Value" : 7
 			},
 			{
 				"Param" : "RetreatVelocity",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "ShakeTime",
 				"Type" : "Float",
 				"Value" : 0.18
 			},
 			{
 				"Param" : "ShakeRange",
 				"Type" : "Float",
 				"Value" : 0.12
 			}
 		]
 	},


	"SKLNumber" : 2,
 	"SKL01" :
 	{
 		"Parameters" :
 		[
 			{
 				"Param" : "JumpPullZAvatarRootHeightThresholdRatio",
 				"Type" : "Float",
 				"Value" : 1.0
 			}
 		],
 		"SubATKs" :
 		[
 			"SKL01_SubATK01",
 			"SKL01_SubATK02"
 		]
 	},
 	"SKL01_SubATK01" :
 	{
 		"CollisionDetectMethod" : "RectCollisionDetect",
 		"Parameters" :
 		[
 			{
 				"Param" : "IsThrow",
 				"Type" : "Bool",
 				"Value" : "False"
 			},
 			{
 				"Param" : "Damage",
 				"Type" : "Float",
 				"Value" : 5.0
 			},
			{
				"Param" : "AniDamageRatio",
				"Type" : "Float",
				"Value" : 0.33
			},
			{
 				"Param" : "OffsetZ",
 				"Type" : "Float",
 				"Value" : -0.1
 			},
 			{
 				"Param" : "CenterYOffset",
 				"Type" : "Float",
 				"Value" : 0.4
 			},
 			{
 				"Param" : "Width",
 				"Type" : "Float",
 				"Value" : 0.5
 			},
 			{
 				"Param" : "Distance",
 				"Type" : "Float",
 				"Value" : 1.0
 			},
 			{
 				"Param" : "FrameHalt",
 				"Type" : "Int",
 				"Value" : 2
 			},
 			{
 				"Param" : "RetreatVelocity",
 				"Type" : "Float",
 				"Value" : 2.0
 			},
 			{
 				"Param" : "ShakeTime",
 				"Type" : "Float",
 				"Value" : 0.05
 			},
 			{
 				"Param" : "ShakeRange",
 				"Type" : "Float",
 				"Value" : 0.05
 			}
 		]
 	},
 	"SKL01_SubATK02" :
 	{
 		"CollisionDetectMethod" : "RectCollisionDetect",
 		"Parameters" :
 		[
 			{
 				"Param" : "IsThrow",
 				"Type" : "Bool",
 				"Value" : "False"
 			},
 			{
 				"Param" : "Damage",
 				"Type" : "Float",
 				"Value" : 15.0
 			},
			{
				"Param" : "AniDamageRatio",
				"Type" : "Float",
				"Value" : 1.0
			},
			{
 				"Param" : "OffsetZ",
 				"Type" : "Float",
 				"Value" : -0.1
 			},
 			{
 				"Param" : "CenterYOffset",
 				"Type" : "Float",
 				"Value" : 0.8
 			},
 			{
 				"Param" : "Width",
 				"Type" : "Float",
 				"Value" : 1.0
 			},
 			{
 				"Param" : "Distance",
 				"Type" : "Float",
 				"Value" : 4.0
 			},
 			{
 				"Param" : "FrameHalt",
 				"Type" : "Int",
 				"Value" : 10
 			},
 			{
 				"Param" : "RetreatVelocity",
 				"Type" : "Float",
 				"Value" : 15.0
 			},
 			{
 				"Param" : "ShakeTime",
 				"Type" : "Float",
 				"Value" : 0.18
 			},
 			{
 				"Param" : "ShakeRange",
 				"Type" : "Float",
 				"Value" : 0.12
 			}
 		]
 	},

 	"SKL02" :
 	{
 		"Parameters" :
 		[
 		],
 		"SubATKs" :
 		[
 			"SKL02_SubATK01",
 			"SKL02_SubATK02"
 		]
 	},
 	"SKL02_SubATK01" :
 	{
 		"CollisionDetectMethod" : "FanCollisionDetect",
 		"Parameters" :
 		[
 			{
 				"Param" : "IsThrow",
 				"Type" : "Bool",
 				"Value" : "False"
 			},
 			{
 				"Param" : "Damage",
 				"Type" : "Float",
 				"Value" : 3.0
 			},
			{
				"Param" : "AniDamageRatio",
				"Type" : "Float",
				"Value" : 0.33
			},
 			{
 				"Param" : "CenterYOffset",
 				"Type" : "Float",
 				"Value" : 0.8
 			},
			{
 				"Param" : "OffsetZ",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "Radius",
 				"Type" : "Float",
 				"Value" : 2.0
 			},
 			{
 				"Param" : "FanAngle",
 				"Type" : "Float",
 				"Value" : 360
 			},
			{
 				"Param" : "MeleeRadius",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "MeleeFanAngle",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "FrameHalt",
 				"Type" : "Int",
 				"Value" : 0
 			},
 			{
 				"Param" : "RetreatVelocity",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "ShakeTime",
 				"Type" : "Float",
 				"Value" : 0.03
 			},
 			{
 				"Param" : "ShakeRange",
 				"Type" : "Float",
 				"Value" : 0.03
 			}
 		]
 	},
 	"SKL02_SubATK02" :
 	{
 		"CollisionDetectMethod" : "FanCollisionDetect",
 		"Parameters" :
 		[
 			{
 				"Param" : "IsThrow",
 				"Type" : "Bool",
 				"Value" : "False"
 			},
 			{
 				"Param" : "Damage",
 				"Type" : "Float",
 				"Value" : 6.0
 			},
			{
				"Param" : "AniDamageRatio",
				"Type" : "Float",
				"Value" : 0.7
			},
 			{
 				"Param" : "CenterYOffset",
 				"Type" : "Float",
 				"Value" : 0.8
 			},
			{
 				"Param" : "OffsetZ",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "Radius",
 				"Type" : "Float",
 				"Value" : 1.0
 			},
 			{
 				"Param" : "FanAngle",
 				"Type" : "Float",
 				"Value" : 360
 			},
			{
 				"Param" : "MeleeRadius",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "MeleeFanAngle",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "FrameHalt",
 				"Type" : "Int",
 				"Value" : 3
 			},
 			{
 				"Param" : "RetreatVelocity",
 				"Type" : "Float",
 				"Value" : 0.0
 			},
 			{
 				"Param" : "ShakeTime",
 				"Type" : "Float",
 				"Value" : 0.06
 			},
 			{
 				"Param" : "ShakeRange",
 				"Type" : "Float",
 				"Value" : 0.06
 			}
 		]
 	},
}