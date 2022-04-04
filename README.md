# 如何使用

- 初始化
```c#
Malody malody = new Malody( "查分账号使用的账号" ,  "查分账号使用的密码" );
//Malody malody = new Malody( 查分账号使用的账号 , 查分账号使用的密码 , Malody网站的基本Url（可选）, 请求时Header中UserAgent（可选）);
```
- 查询名字包含 siscon 字样的用户 list
```c#
var queryList = await ma.MalodyUserQuery("siscon");
```

- 查询 malody id 为 308032 的最近游玩歌曲信息
```c#
var userRecentScore = await ma.SearchRecent(308032);
```

- 查询 malody id 为 308032 的用户信息
```c#
var user = await ma.GetUserInfo(308032);
```

- 获取查分账号的 xCsrfToken
```c#
var xCsrfToken = ma.GetXCsrfToken();
```

- 获取查分账号的 sessionId
```c#
var sessionId = ma.GetSessionId();
```

- 获取查分账号的 xCsrfToken 过期时间
```c#
var xCsrfTokenExpireDate = ma.GetXCsrfTokenExpireDate();
```

- 获取查分账号的 sessionId 过期时间
```c#
var sessionIdExpireDate = ma.GetSessionIdExpireDate();
```

- MalodyUserInfoModel ( 用户信息类 )
```c#
public string UserName { get; set; }
public DateTime JoinedTime { get; set; }
public DateTime LastPlayedTime { get; set; }
public string TotalPlayedTime { get; set; }
public string Sex { get; set; }
public string Age { get; set; }
public string LiveIn { get; set; }
public long CoinCount { get; set; }
public long Income { get; set; }
public string ChartBeingPlayedTime { get; set; }
public int OnSaleChartsCount { get; set; }
public int NoneOnSaleChartsCount { get; set; }
public List<string> UserRole { get; set; }
public List<MalodyUserRankModel> MalodyUserRanks { get; set; }
public List<MalodyUserActivityModel> MalodyUserActivities { get; set; }
public List<string> MalodyAchievePicUrls { get; set; }
```

- MalodyUserRankModel ( Malody用户排名类 )
```c#
public MalodyPlayMode Mode { get; set; }
public int Rank { get; set; }
public int Exp { get; set; }
public int PlayCount { get; set; }
public float Acc { get; set; }
public int Combo { get; set; }
```

- MalodySongModel ( 歌曲信息类 )
```c#
public string SongName { get; set; }
public string CoverPicUrl { get; set; }
public int Score { get; set; }
public int Combo { get; set; }
public float Acc { get; set; }
public MalodyPlayJudge Judge { get; set; }
public string PlayedTime { get; set; }
public MalodyPlayMode Mode { get; set; }
```
