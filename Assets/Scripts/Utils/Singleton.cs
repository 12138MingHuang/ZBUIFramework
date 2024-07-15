/// <summary>
/// Singleton 类提供一个通用的单例模式实现。
/// 该类确保类型 T 只有一个实例，并提供一个全局访问点。
/// </summary>
public class Singleton<T>
{

    /// <summary>
    /// 存储单例实例的私有静态字段。
    /// </summary>
    private static T instance;

    /// <summary>
    /// 获取类型 T 的单例实例。如果实例不存在，则创建一个新的实例。
    /// </summary>
    public static T Instance
    {
        get
        {
            // 如果实例未创建，则创建一个新的实例
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}
