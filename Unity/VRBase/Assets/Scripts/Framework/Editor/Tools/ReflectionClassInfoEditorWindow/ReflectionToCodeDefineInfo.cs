using System;
using System.Reflection;

/// <summary>
/// 将反射信息转换为代码定义信息
/// </summary>
public class ReflectionToCodeDefineInfo  {
	public static string GetTypeAllInfo(Type type, ConstructorInfo[] cs, FieldInfo[] fs, PropertyInfo[] ps, MethodInfo[] ms)
    {
        if (type == null) return "";
        string content = "";
        content += "程序集: " + type.Assembly.FullName + "\n";
        content+="Module: " + type.Module.FullyQualifiedName + "\n";
        content += "\n\n";
        bool isHaveNameSpace = false;
        if (!string.IsNullOrEmpty(type.Namespace))
        {
            isHaveNameSpace = true;
            content += " namespace " + type.Namespace + "\n" + "{";
        }

        if (type.IsSerializable)
            content += "\t  "+"[System.Serializable]\n";
        string attributs = type.Attributes.ToString();
       
            content += "\t" + "  " + GetAccessPermissionKeyWords(type) + "  ";

     
        if (attributs.Contains("Abstract")&& attributs.Contains("Sealed"))
        {
                content += "  static  ";
        }
        else
        {
            if (attributs.Contains("Sealed"))
                content += "  sealed  ";
            if (attributs.Contains("Abstract"))
                content += "  abstract  ";
        }

        if (type.IsClass)
            content += " class";
        else if (type.IsEnum)
            content += " enum";
        else if (type.IsInterface)
            content += " interface";
        else if (type.IsValueType)
            content += " struct";

        content +=" "+ TypeNameChange( type);
        content = KeyWordChangeColor(content);
        content += "\n\t{";
        if (fs.Length > 0)
            content += "\n";
       
        for (int i = 0; i < cs.Length; i++)
        {
            content += "\t    " + GetConstructorInfoToDeclaring(type, cs[i]) + "\n";
        }
        if(fs.Length>0)
            content += "\n\n";
        for (int i = 0; i < fs.Length; i++)
        {
            content +="\t    "+ GetFieldInfoToDeclaring(type,fs[i]) + "\n";
        }
        if (ps.Length > 0)
            content += "\n\n";
        for (int i = 0; i < ps.Length; i++)
        {
            content += "\t    " + GetPropertyInfoToDeclaring(type,ps[i]) + "\n";
        }
        if(ms.Length>0)
        content += "\n\n";
        for (int i = 0; i < ms.Length; i++)
        {
            content += "\t    " + GetMethodInfoToDeclaring(type, ms[i]) + "\n";
        }
        content += "\n\t}";
        if (isHaveNameSpace)
            content += "\n}";
        return content;
       
    }
    public static string GetGenericArgumentString(Type[] GenericArgTypes)
    {
        string ss = "";
        for (int i = 0; i < GenericArgTypes.Length; i++)
        {
            ss += GenericArgTypes[i].ToString();
            if (i < (GenericArgTypes.Length - 1))
                ss += ",";
        }
        return "<" + ss + ">";
    }
    private static string GetSealedKeyWord(object o)
    {
        string s = "";
        bool IsStatic = GetPropertyValue<bool>(o, "IsStatic");
        if (IsStatic)
            return "  static  ";
        bool IsAbstract = GetPropertyValue<bool>(o, "IsAbstract");
        if (IsAbstract)
            return "  abstract  ";
        bool isOverride = false;
        if(o is MethodInfo)
        {
            MethodInfo me = (MethodInfo)o;
            MethodInfo m = me.GetBaseDefinition();
            if (!me.Equals(m))
                isOverride = true;

        }else if(o is PropertyInfo)
        {
            PropertyInfo f = (PropertyInfo)o;
            MethodInfo mget = null;
            MethodInfo mset = null;
            MethodInfo mm1 = null;
            mget = f.GetGetMethod();

            mset = f.GetSetMethod();
            if (mget != null)
                mm1 = mget;
            else if (mset != null)
                mm1 = mset;

            if (mm1 != null)
            {
                MethodInfo m = mm1.GetBaseDefinition();
                if (!mm1.Equals(m))
                    isOverride = true;
            }
        }
        bool IsFinal = GetPropertyValue<bool>(o, "IsFinal");
        bool IsVirtual = GetPropertyValue<bool>(o, "IsVirtual");

        if (IsFinal)
        {
            s += "  sealed  ";
            if (isOverride)
                s += "override  ";
        }
        else
        {
            if (isOverride)
                s += "  override  ";
            else if (IsVirtual)
                s += "  virtual  ";
        }


        return s;
    }
    private static string GetAccessPermissionKeyWords(object o)
    {
        if(o is Type)
        {
            Type t = (Type)o;
            if (t.IsClass || t.IsEnum||t.IsValueType|| t.IsInterface)
            {
                if(t.IsPublic)
                    return "public";
                else
                    return "internal";
            }
        }
        
        if (GetPropertyValue<bool>(o, "IsAssembly"))
            return "internal";
        if (GetPropertyValue<bool>(o, "IsFamily"))
            return "protected";
        if (GetPropertyValue<bool>(o, "IsPublic"))
            return "public";
        if (GetPropertyValue<bool>(o, "IsPrivate"))
            return "private";
        return "";
    }

    private static T GetPropertyValue<T>(object o, string propertyName)
    {
        PropertyInfo ps = o.GetType().GetProperty(propertyName);
        if (ps != null)
            return (T)ps.GetValue(o, null);
        else
            return default(T);
    }

    public static string GetFieldInfoToDeclaring(Type type, FieldInfo f)
    {
        string content = "";

            content += "  "+ GetAccessPermissionKeyWords(f) + "  ";    

        if (f.IsStatic)
        {
            content += "  static  ";
        }
        if (f.IsInitOnly)
        {
            content += "  readonly  ";
        }
        if (f.IsLiteral)
        {
            content += "  const  ";
        }
        content += " "+ TypeNameChange(f.FieldType)+" ";

        content += AddColor(f.Name, f.Name, "#00ff00ff") ;
        if (f.IsLiteral)
        {
            content += " = " + f.GetValue(null);
        }
        content += ";";
        content = KeyWordChangeColor(content);
        return content;
    }

  
    public static string GetPropertyInfoToDeclaring(Type type, PropertyInfo f)
    {
        string content = "";
        content += " public ";
        MethodInfo mget = null;
        MethodInfo mset = null;
        MethodInfo m = null;
        mget = f.GetGetMethod();
       
        mset = f.GetSetMethod();
        if (mget != null)
            m = mget;
       else if (mset!=null)
         m = mset;
        
        if (m != null)
        {
            content += GetSealedKeyWord(m);
        }
        content +=" "+ TypeNameChange(f.PropertyType) + " ";

        content += AddColor(f.Name, f.Name, "#00ff00ff") + " ";
        content += "{ ";
        if (mget!=null)
        {
            content += "  " + GetAccessPermissionKeyWords(mget) + "  ";
            content += " get ;";
        }
        if (mset!=null)
        {
            content += "  " + GetAccessPermissionKeyWords(mset) + "  ";
            content += " set ;";
        }
        content += "};";
        content = KeyWordChangeColor(content);
        return content;
    }
    public static string GetConstructorInfoToDeclaring(Type type, ConstructorInfo f)
    {
        string content = "";
       // string attrs = f.Attributes.ToString();
        content += "  " + GetAccessPermissionKeyWords(f) + "  ";

        content += GetSealedKeyWord(f);

        content += AddColor(f.Name, f.Name, "#00ff00ff");
        if (f.IsGenericMethod)
        {
            Type[] pis = f.GetGenericArguments();
            content += GetGenericArgumentString(pis);
        }

        {
            content += "(";
            ParameterInfo[] pis = f.GetParameters();
            string ss = "";
            for (int i = 0; i < pis.Length; i++)
            {
                ParameterInfo p = pis[i];
                if (p.ParameterType.IsByRef)
                {
                    if (p.IsOut)
                    {
                        ss += "  out  ";
                    }
                    else
                    {
                        ss += "  ref  ";
                    }
                }
                ss += TypeNameChange(p.ParameterType) + " " + AddColor(p.Name, p.Name, "#00ff00ff");
                object temp = p.GetType().GetField("DefaultValueImpl", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(p);
                if (temp != null && !string.IsNullOrEmpty(temp.ToString()))
                    ss += " = " + temp;
                if (i < (pis.Length - 1))
                    ss += ", ";
            }
            content += ss + ");";
        }
        content = KeyWordChangeColor(content);
        return content;
    }
    public static string GetMethodInfoToDeclaring(Type type, MethodInfo f)
    {
        string content = "";
       // string attrs = f.Attributes.ToString();
        content += "  " + GetAccessPermissionKeyWords(f) + "  ";

        content += GetSealedKeyWord(f);
        if (!f.IsConstructor)
        {
            content += " "+ TypeNameChange(f.ReturnParameter.ParameterType)+ " ";
        }
        content += AddColor(f.Name, f.Name, "#00ff00ff");
        if (f.IsGenericMethod)
        {
           Type[] pis = f.GetGenericArguments();           
            content += GetGenericArgumentString(pis);
        }
            
        {
            content += "(";
            ParameterInfo[] pis = f.GetParameters();
            string ss = "";
            for (int i = 0; i < pis.Length; i++)
            {
                ParameterInfo p = pis[i];
                if (p.ParameterType.IsByRef)
                {
                    if (p.IsOut)
                    {
                        ss += "  out  ";
                    }
                    else
                    {
                        ss += "  ref  ";
                    }
                }               
                ss += TypeNameChange(p.ParameterType) + " " + AddColor(p.Name, p.Name, "#00ff00ff");
              object temp =  p.GetType().GetField("DefaultValueImpl", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(p);
                if (temp!=null && !string.IsNullOrEmpty(temp.ToString()))
                    ss += " = " + temp;
                if (i < (pis.Length - 1))
                    ss += ", ";
            }
            content += ss + ");";
        }
        content = KeyWordChangeColor(content);
        return content;
    }
    private static string TypeNameChange(Type t)
    {
        if (t == null)
            return "";
        string name = t.FullName ==null? t.ToString():t.FullName;
        bool IsByRef = false;
        if (name.Contains("&") && name.EndsWith("&"))
        {
            name = name.Replace("&", "");
            IsByRef = t.IsByRef;
        }
        if (t.IsArray)
        {
            name = TypeNameChange(t.GetElementType());
            name += "[]";
            
        }
         if (name.Contains("`"))
        {
            name = name.Split('`')[0];
            string s = "";
           Type[] tArr = t.GetGenericArguments();
            for (int i = 0; i < tArr.Length; i++)
            {
                s += TypeNameChange(tArr[i]);
                if (i < tArr.Length - 1)
                    s += ",";
            }
            name = name + "<" + s + ">";
        }
        else if (name == typeof(object).FullName)
        {
            name = "object";
        }
        else if (name == typeof(string).FullName)
        {
            name = "string";
        }
        else if (name == typeof(bool).FullName)
        {
            name = "bool";
        }
        else if (name == typeof(byte).FullName)
        {
            name = "byte";
        }
        else if (name == typeof(char).FullName)
        {
            name = "char";
        }
        else if (name == typeof(double).FullName)
        {
            name = "double";
        }
        else if (name == typeof(float).FullName)
        {
            name = "float";
        }
        else if (name == typeof(int).FullName)
        {
            name = "int";
        }
        else if (name == typeof(long).FullName)
        {
            name = "long";
        }
        else if (name == typeof(float).FullName)
        {
            name = "float";
        }
        else if (name == typeof(short).FullName)
        {
            name = "short";
        }
        else if (name == typeof(uint).FullName)
        {
            name = "uint";
        }
        else if (name == typeof(ulong).FullName)
        {
            name = "ulong";
        }
        else if (name == typeof(ushort).FullName)
        {
            name = "ushort";
        }
        else if (name == typeof(void).FullName)
        {
            name = "void";
        }
        if (name != t.FullName)
            name = " " + name + " ";
        
        return name;
    }
    public static string KeyWordChangeColor(string content)
    {
        string s = "abstract|as|base|bool| "
  + " break	byte|case|catch| "
  + "char|checked|class|const| "
  + "continue|decimal|default|delegate |"
  + "do|double|else|enum|"
  + "event|explicit  |  extern	|false| "
  + "finally|	fixed |float  | for |"
  + "foreach	|goto	|if	|implicit |"
  + "in	  | int| interface |"
  + "internal|	is|	lock|	long| "
  + "namespace |  new	|null	|object| "
  + "operator  |  out	|  override |"
  + "params|  private| protected |public| "
  + "readonly| ref| return|	sbyte| "
  + "sealed |short|	sizeof|	stackalloc |"
  + "static |string| struct |switch|"
  + "this |   throw	|true	|try |"
  + "typeof	|uint  |  ulong |unchecked |"
  + "unsafe |ushort	|using	|using |static |"
  + "void |volatile|	while |  get| set|virtual|";
        s = s.Replace("\t", "");
        string[] kws = s.Split('|');
        for (int i = 0; i < kws.Length; i++)
        {
            kws[i] = kws[i].Trim();
           
        }

        for (int i = 0; i < kws.Length; i++)
        {
            content = AddColor(content, " " + kws[i] + " ", "#00ffffff");
        }
        return content;
    }
    public static string AddColor(string content,string ChangeColorContent, string color)
    {
        if (!content.Contains(ChangeColorContent))
            return content;

        string s = "<color=" + color + ">" + ChangeColorContent + "</color>";
        content = content.Replace(ChangeColorContent, s);
        return content;
    }
}


