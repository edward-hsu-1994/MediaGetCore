MediaGetCore 1.0.0.0
---
這是一個支援.net Core環境的.net類別庫，開發者可以透過簡單的調用剖析器物件，解析網路媒體影音真實位址。

本專案延伸自MediaGet，因原本專案僅支援Windows之.net環境下，如要應用於最新的ASP.net 5與Linux環境中會有困難，所以我將架構重整，今後MediaGet的更新都將於這個專案進行發布。

在本專案中，使用到許多指令必須要調用系統中的NodeJs支援，故您在使用本類別庫之前必須要檢察系統中是否有安裝NodeJs環境，在這個類別庫中我們已經提供一個靜態屬性(MediaGetCore.Factories.NodeJsFactory.IsSupport)可以調用檢查


###支援
1. Youtube
2. Xuite
3. Dailymotion

(持續更新中)