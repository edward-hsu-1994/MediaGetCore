MediaGetCore 1.0.1
---
這是一個支援 .net Core 環境的 .net 類別庫，開發者可以透
過簡單的調用剖析器物件，解析網路媒體影音真實位址。

本專案延伸自 MediaGet ，因原本專案僅支援 Windows 之 .net
環境下，如要應用於最新的ASP.net 5與Linux環境中會有
困難，所以我將架構重整，今後 MediaGet 的更新都將於這個
專案進行發布。

在本專案中，使用到許多指令必須要調用系統中的 NodeJs
支援，故您在使用本類別庫之前必須要檢查系統中是否有安
裝 NodeJs 。

本套件之Js執行方式可經由 Factories.JsFactory.ScriptHandler 
屬性進行變更，預設為 NodeJsScriptHandler ，可依環境調整。

### [Nuget](https://www.nuget.org/packages/MediaGetCore/)
```
PM> Install-Package MediaGetCore
```

### 快速上手
```csharp
using MediaGetCore;
...(something)...
ExtractorAdapter adp = new ExtractorAdapter();
adp.IncludeAllExtractors();//引入所有的支援項目

var infos = adp.GetMediaInfos("https://www.youtube.com/watch?v=b1P-SQASlMs");
var firstRealUrl = infos.First().RealUrl;
```
當然您可以使用單一影音平台的剖析器進行影音剖析的動作。
```csharp
using MediaGetCore.Extractors;
...(something)...
YoutubeExtractor yt = new YoutubeExtractor();
var infos = yt.GetMediaInfos("https://www.youtube.com/watch?v=b1P-SQASlMs");
var firstRealUrl = infos.First().RealUrl;
```

### 支援
1. Youtube
2. Xuite
3. Dailymotion

(持續更新中)
