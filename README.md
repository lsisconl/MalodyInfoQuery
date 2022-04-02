# 如何使用
```c#
static async Task Main(string[] args)
{
   Malody malody = new Malody(查分账号使用的账号, 查分账号使用的密码);//Malody malody = new Malody(查分账号使用的账号, 查分账号使用的密码,Malody网站的基本Url（可选）,请求时Header中UserAgent（可选）);
   var queryList = await ma.MalodyUserQuery("siscon");//查询名字包含siscon字样的用户list
   var userRecentScore = await ma.SearchRecent(308032);//查询malody id为308032的最近游玩歌曲信息 
   var user = await ma.GetUserInfo(308032);//查询malody id为308032的用户信息
   var xCsrfToken = ma.GetXCsrfToken();//获取查分账号的xCsrfToken
   var sessionId = ma.GetSessionId();//获取查分账号的sessionId
   var xCsrfTokenExpireDate = ma.GetXCsrfTokenExpireDate();//获取查分账号的xCsrfToken过期时间
   var sessionIdExpireDate = ma.GetSessionIdExpireDate();//获取查分账号的sessionId过期时间
}
```
