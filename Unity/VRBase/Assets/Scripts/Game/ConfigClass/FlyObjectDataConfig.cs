using HDJ.Framework.Modules;
using UnityEngine;
// 自动生成请勿更改
	/// <summary>
 	/// 配置描述									
	/// </summary>
public class FlyObjectDataConfig : TableConfigBase
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
 	/// 飞行物飞行轨迹
	/// </summary>
	 public string skillConsumption;
	/// <summary>
 	/// 技能发动属性效果(魔力生命等)
	/// </summary>
	 public string skillPropertyChange;
	/// <summary>
 	/// 命中时飞行物是否消失
	/// </summary>
	 public bool isDisappearWhenHit;
	/// <summary>
 	/// 命中范围及参数(Json)
	/// </summary>
	 public string hitRange;
	/// <summary>
 	/// 命中时特效名字
	/// </summary>
	 public string hitFX;
	/// <summary>
 	/// 飞行时声音(碰撞)
	/// </summary>
	 public string flySFX;
	/// <summary>
 	/// 飞行物预制名
	/// </summary>
	 public string flyprefabName;
}