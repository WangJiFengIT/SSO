using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;


namespace client1.Controllers
{
    public class HomeController : Controller
    {
        public static List<string> Tokens = new List<string>();

        public async Task<ActionResult> Index()
        {
            var tokenId = Request.QueryString["tokenId"];
            //如果tokenId不为空，则是由Service302过来的。
            if (tokenId != null)
            {
                using (HttpClient http = new HttpClient())
                {
                    //验证token是否有效
                    var isValid = await http.GetStringAsync("http://localhost:8018/Home/TokenIdIsValid?tokenId=" + tokenId);//将get请求发送到指定url并在异步操作中以字符串的形式返回响应正文
                    
                    if (bool.Parse(isValid.ToString()))
                    {
                        if (!Tokens.Contains(tokenId))
                        {
                            //记录登录过的Client (主要是为了可以统一登出)
                            Tokens.Add(tokenId);
                        }
                        Session["token"] = tokenId;
                    }
                }
            }
            //判断是否是登录状态
            if (Session["token"] == null || !Tokens.Contains(Session["token"].ToString()))
            {
                //未登录
                return Redirect("http://localhost:8018/Home/Verification?backUrl=http://localhost:57667/Home");
            }
            else
            {
                //有登录 清空session
                if (Session["token"] != null)
                {
                    Session["token"] = null;
                }
            }
            return View();
        }

        /// <summary>
        /// 退出 token的删除
        /// </summary>
        /// <param name="tokenId"></param>
        public void ClearToken(string tokenId)
        {
            Tokens.Remove(tokenId);
        }
    }
}