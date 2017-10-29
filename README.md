MediaGetCore [![xpy MyGet Build Status](https://www.myget.org/BuildSource/Badge/xpy?identifier=79c81589-818b-4f7f-8233-a0ee42cf5264)](https://www.myget.org/)
[![Build Status](https://travis-ci.org/XuPeiYao/MediaGetCore.svg?branch=master)](https://travis-ci.org/XuPeiYao/MediaGetCore)
---
這是一個支援 .NET Core 環境的類別庫，開發者可以透過簡單的調用剖析器物件，解析網路媒體影音真實位址。

### 安裝
1. [NuGet](https://www.nuget.org/packages/MediaGetCore/)
2. [MyGet](https://www.myget.org/feed/xpy/package/nuget/MediaGetCore)
```
PM> Install-Package MediaGetCore
```

### 快速上手
使用單一影音平台的剖析器進行影音剖析的動作。
```csharp
using MediaGetCore.Extractors;
...(something)...
YoutubeExtractor yt = new YoutubeExtractor();
// Async Method
var infos = await yt.GetMediaInfosAsync("https://www.youtube.com/watch?v=<VIDEO_ID>");
// ELSE Use sync Method ( using MediaGetCore.Extensions; )
var infos = yt.GetMeidaInfos("https://www.youtube.com/watch?v=<VIDEO_ID>");

var firstRealUrl = infos.First().RealUrl;
```

### 支援
1. Youtube
2. Xuite
3. Dailymotion