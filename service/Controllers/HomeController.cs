using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;


namespace service.Controllers
{
    public class HomeController : Controller
    {
        public static Dictionary<string, Guid> TokenIds = new Dictionary<string, Guid>();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 验证是否登录
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Verification(string backUrl)
        {
            if (Session["user"] == null)
            {
                return Redirect("Login?backUrl=" + backUrl);
            }

            Session["user"] = "已经登录";
            return Redirect(backUrl + "?tokenId=" + TokenIds[Session.SessionID]);
        }

        /// <summary>
        /// 验证tokenId是否有效
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public bool TokenIdIsValid(Guid tokenId)
        {
            return TokenIds.Any(t => t.Value == tokenId);
        }

        /// <summary>
        /// 返回登录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <param name="backUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public string Login(string name, string pwd, string backUrl)
        {
            if (name == "wjf" && pwd == "970512")//TODO：验证用户名密码登录
            {
                //用session标识会话是否登录
                Session["user"] = "王继峰已登录";
                //在认证中心 保存客户端Client的登录认证码
                TokenIds.Add(Session.SessionID, Guid.NewGuid());
            }
            else
            {
                //认证失败 重新登录
                return "/Home/Login";
            }
            return backUrl + "?tokenId=" + TokenIds[Session.SessionID];//生成一个token发放到客户端
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Logout()
        {
            using (HttpClient http = new HttpClient())
            {
                await http.GetAsync("http://localhost:57667/Home/ClearToken?tokenId=" + TokenIds[Session.SessionID]);
                await http.GetAsync("http://localhost:58002/Home/ClearToken?tokenId=" + TokenIds[Session.SessionID]);
            }

            TokenIds.Remove(Session.SessionID);
            Session["user"] = null;
            return Redirect("Login");
        }
    }
}