# 如何使用

- 初始化
```c#
Malody malody = new Malody(查分账号使用的账号, 查分账号使用的密码);
//Malody malody = new Malody(查分账号使用的账号,查分账号使用的密码,Malody网站的基本Url（可选）,请求时Header中UserAgent（可选）);
```
- 查询名字包含siscon字样的用户list
```c#
var queryList = await ma.MalodyUserQuery("siscon");
```

- 查询malody id为308032的最近游玩歌曲信息
```c#
var userRecentScore = await ma.SearchRecent(308032);//
```

- 查询malody id为308032的用户信息
```c#
var user = await ma.GetUserInfo(308032);//
```

- 获取查分账号的xCsrfToken
```c#
var xCsrfToken = ma.GetXCsrfToken();//
```

- 获取查分账号的sessionId
```c#
var sessionId = ma.GetSessionId();//
```

- 获取查分账号的xCsrfToken过期时间
```c#
var xCsrfTokenExpireDate = ma.GetXCsrfTokenExpireDate();//
```

- 获取查分账号的sessionId过期时间
```c#
var sessionIdExpireDate = ma.GetSessionIdExpireDate();//
```
