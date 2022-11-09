<div align="center">
    <img src="https://user-images.githubusercontent.com/89084713/200838730-abba4415-7785-417b-a0a1-69bc5c5c150c.png" alt="Icon" width="180px">
</div> 

# YieldExportReports

データベースから取得したデータをExcelファイルにマッピングし出力できるオープンソースアプリケーションです。
C#で作成しており対応OSはWindowsのみとなります。

<a href="https://zenn.dev/takuty/articles/598b38c2c27434">詳細情報はこちらに記載しております。<a>

<a href="https://github.com/Takuty-a11y/YieldExportReports/releases/tag/v1.0.0">Releases</a>よりダウンロードできます。

# Demo

<div align="center">
    <img src="https://user-images.githubusercontent.com/89084713/200838977-866469ff-2e20-40a8-8fe5-ba3e615b03e5.png" alt="demo" width="960px">
</div>

# Main Features

- **データベース**：4つのデータベース接続が可能でありSQLから自由にデータを取得可能です
- **設定情報管理**：データベース接続や出力設定をファイルに保存することができるので設定値の切替が可能です
- **出力設定**：データからセルや分割指定など様々な設定が可能です
- **ファイル形式**：レポートはExceのみ,データ出力はXML,JSON,CSV,Excelの形式で出力可能です

# Requirement

For use
- Windows 7, Windows 8, Windows 10, Windows 11
- .Net 6.0 (Microsoft Windows Desktop Runtime 6)
- SQLServer, OLE DB, MySQL, PostgreSQL

For Development
- Visual Studio Community 2022

# Lisence

This software is released under the MIT License, see LICENSE.txt.
