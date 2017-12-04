using HDJ.Framework.Modules;
using UnityEngine;
// 自动生成请勿更改
	/// <summary>
 	/// 配置描述																																					
	/// </summary>
public class SkillDataConfig : TableConfigBase
{ 
	/// <summary>
 	/// id
	/// </summary>
	 public int id;
	/// <summary>
 	/// 名字
	/// </summary>
	 public string name;
	/// <summary>
 	/// 技能备注
	/// </summary>
	 public string describe;
	/// <summary>
 	/// 技能等级
	/// </summary>
	 public int level;
	/// <summary>
 	/// 技能图标
	/// </summary>
	 public string iconName;
	/// <summary>
 	/// 技能冷却时间(毫秒)
	/// </summary>
	 public int coolingTime;
	/// <summary>
 	/// 技能发动半径(0不限制,超出范围不能启动)
	/// </summary>
	 public float skillLaunchRadius;
	/// <summary>
 	/// 技能发动消耗(魔力生命等)
	/// </summary>
	 public string skillConsumption;
	/// <summary>
 	/// 技能发动属性效果(魔力生命等)
	/// </summary>
	 public string skillPropertyChange;
	/// <summary>
 	/// 技能命中方式(0直接命中,1飞行物命中)
	/// </summary>
	 public int hitType;
	/// <summary>
 	/// 命中最大人数(0-100,0无限)
	/// </summary>
	 public int hitMaxNum;
	/// <summary>
 	/// 命中范围及参数(Json)
	/// </summary>
	 public string hitRange;
	/// <summary>
 	/// 命中时动画(勾住敌人)
	/// </summary>
	 public string hitAnimName;
	/// <summary>
 	/// 命中时特效名字
	/// </summary>
	 public string hitFX;
	/// <summary>
 	/// 命中时声音(碰撞)
	/// </summary>
	 public string hitSFX;
	/// <summary>
 	/// 技能作用对象类型(类型，关系，种族等,json)
	/// </summary>
	 public string skillHitObjectType;
	/// <summary>
 	/// 飞行物名称,延时,生成位置物体
	/// </summary>
	 public string flyObject;
	/// <summary>
 	/// 技能操作类型(0主动,1被动)
	/// </summary>
	 public int skillHandleType;
	/// <summary>
 	/// 可激活的
	/// </summary>
	 public bool canActive;
	/// <summary>
 	/// 抗打断级别
	/// </summary>
	 public int resistanceInterruptLevel;
	/// <summary>
 	/// 打断级别
	/// </summary>
	 public int interruptLevel;
	/// <summary>
 	/// 关联buffer(buff表ID)
	/// </summary>
	 public int[] useBuffers;
	/// <summary>
 	/// 是否吟唱
	/// </summary>
	 public bool isSing;
	/// <summary>
 	/// 吟唱时是否可移动
	/// </summary>
	 public bool singCanMove;
	/// <summary>
 	/// 吟唱时间(毫秒)
	/// </summary>
	 public int singTime;
	/// <summary>
 	/// 吟唱动画(循环)
	/// </summary>
	 public string singAnimName;
	/// <summary>
 	/// 吟唱特效，延时,持续时间,产生部位名称(GamebOject)
	/// </summary>
	 public string[] singFX;
	/// <summary>
 	/// 吟唱音效及发动延时(毫秒)
	/// </summary>
	 public string[] singSFX;
	/// <summary>
 	/// 发动时动画(可有多个，一个一个播放逗号分开)
	/// </summary>
	 public string launchAnimName;
	/// <summary>
 	/// 发动特效，延时,持续时间,产生部位名称(GamebOject)
	/// </summary>
	 public string[] launchFX;
	/// <summary>
 	/// 技能发动音效及发动延时(毫秒)
	/// </summary>
	 public string[] launchSFX;
	/// <summary>
 	/// 是否引导技能
	/// </summary>
	 public bool isGuidingSkill;
	/// <summary>
 	/// 引导时是否可移动
	/// </summary>
	 public bool guidingSkillCanMove;
	/// <summary>
 	/// 引导时间(毫秒)
	/// </summary>
	 public int guidingSkillTime;
	/// <summary>
 	/// 引导间隔时间(每个间隔发出一组技能效果,毫秒)
	/// </summary>
	 public int guidingSkillIntervalTime ;
	/// <summary>
 	/// 引导动画(循环)
	/// </summary>
	 public string guidingSkillAnimName;
	/// <summary>
 	/// 引导特效，延时,持续时间,产生部位名称(GamebOject)
	/// </summary>
	 public string[] guidingSkillFX;
	/// <summary>
 	/// 引导音效及发动延时(毫秒)
	/// </summary>
	 public string[] guidingSkillSFX;
}