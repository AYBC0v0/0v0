using System.Threading.Tasks;

namespace Newbe.Mahua.Plugins.RepeaterBreaker.Services
{
    /// <summary>
    /// B站直播姬
    /// </summary>
    public interface ILivegirl
    {
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        Task StopAsnyc();
    }
}
