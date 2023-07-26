using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungfuDesignPattern
{
    /// <summary>
    /// 负载均衡器
    /// </summary>
    internal class LoadBalancer
    {
        private static readonly object syncLocker = new object();

        // 私有静态变量，存储唯一实例
        private static LoadBalancer? instance = null;

        // 服务器集合
        private IList<CustomServer> serverList;

        // 私有构造函数
        private LoadBalancer()
        {
            serverList = new List<CustomServer>();
        }

        #region 双重锁检测
        /// <summary>
        /// 双重锁检测
        /// </summary>
        /// <returns></returns>
        public static LoadBalancer GetLoadBalancer()
        {
            if (instance == null)
            {
                lock (syncLocker)
                {
                    if (instance == null)
                    {
                        instance = new LoadBalancer();
                    }
                }
            }
            return instance;
        }
        #endregion

        #region 静态内部类
        /// <summary>
        /// 饿汉式单例不能延迟加载，懒汉式单例安全控制繁琐，而且性能受影响。静态内部类单例则将这两者有点合二为一。
        /// </summary>
        /// <returns></returns>
        // 公共静态成员方法，返回唯一实例
        public static LoadBalancer GetLoadBalancerForInternalClass()
        {
            return Nested.instance;
        }

        // 使用内部类+静态构造函数实现延迟初始化
        class Nested
        {
            static Nested() { }
            internal static readonly LoadBalancer instance = new LoadBalancer();
        }
        #endregion


        // 添加一台Server
        public void AddServer(CustomServer server)
        {
            serverList.Add(server);
        }

        // 移除一台Server
        public void RemoveServer(string serverName)
        {
            foreach (var server in serverList)
            {
                if (server.Name.Equals(serverName))
                {
                    serverList.Remove(server);
                    break;
                }
            }
        }

        // 获得一台Server - 使用随机数获取
        private Random rand = new Random();
        public CustomServer GetServer()
        {
            int index = rand.Next(serverList.Count);

            return serverList[index];
        }
    }


    /// <summary>
    /// 假装自己是一台服务器
    /// </summary>
    internal class CustomServer
    {
        public string Name { get; set; }
        public int Size { get; set; }
    }
}
