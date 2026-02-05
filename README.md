# Taskbar Monitor

[English](#english) | [日本語](#japanese)

---

<a name="english"></a>
## English

**Taskbar Monitor** is a Windows 11 taskbar widget that overlays environment sensor data and network latency metrics directly next to your system clock. It seamlessly integrates into your desktop, providing real-time monitoring at a glance.

### Features

*   **Environment Monitoring**: Displays Temperature, Humidity, and CO2 levels from InfluxDB.
*   **Network Latency**: Monitors Ping latency to Local DNS and Gateway with sub-millisecond precision.
*   **Taskbar Integration**: Automatically positions itself next to the system tray clock on your selected monitor.
*   **Multi-Monitor Support**: Configurable to display on any specific monitor.
*   **Settings UI**: Easy-to-use configuration window accessible via the right-click context menu.

### Installation

1.  Download the latest installer (`TaskbarMonitor.msi`) from the [Releases](#) page.
2.  Run the installer and follow the instructions.
    *   *Note: This software is provided "AS IS", without warranty of any kind.*
3.  The application will start automatically. You can find it in your Start Menu as "Taskbar Monitor".

### Configuration

#### InfluxDB Setup

This application expects an **InfluxDB v2** instance with the following structure:

*   **Bucket Name**: `environment` (default, configurable)
*   **Measurement**: `environment`
*   **Fields**:
    *   `temperature` (Float)
    *   `humidity` (Float)
    *   `co2` (Integer/Float)

Ensure your InfluxDB setup matches these field names, or the widget will not display sensor data.

#### Application Settings

Right-click the widget to open **Settings**. You can configure:
*   **InfluxDB**: URL, Token, Org, Bucket.
*   **Network**: Local DNS IP, Gateway IP.
*   **Display**: Target Display Index (0, 1, ...), UI Offset.

### Development

**Requirements:**
*   Windows 10/11
*   .NET 8.0 SDK
*   WiX Toolset v4 (for installer)

**Build:**
```powershell
dotnet publish TaskbarMonitor/TaskbarMonitor.csproj -c Release -r win-x64 /p:PublishSingleFile=true /p:SelfContained=true
```

### License

This project is licensed under the **GNU General Public License v3.0 (GPLv3)** - see the [LICENSE](LICENSE) file for details.

---

<a name="japanese"></a>
## Japanese

**Taskbar Monitor** は、Windows 11 のタスクバー（時計の横）に、環境センサーデータやネットワークの応答速度（Ping）をオーバーレイ表示する常駐型ウィジェットです。

### 機能

*   **環境モニタリング**: InfluxDB から取得した気温・湿度・CO2濃度を表示します。
*   **ネットワーク監視**: ローカルDNSおよびゲートウェイへのPing応答時間を、サブミリ秒（0.xx ms）単位で表示します。
*   **タスクバー統合**: 指定したモニターの時計の横に自動的に配置され、邪魔になりません。
*   **マルチモニター対応**: 設定により、表示するディスプレイを自由に選択できます。
*   **設定画面**: 右クリックメニューから、接続先や表示設定を簡単に変更できます。

### インストール方法

1.  [Releases](#) ページから最新のインストーラー (`TaskbarMonitor.msi`) をダウンロードします。
2.  インストーラーを実行してインストールしてください。
    *   *注意: 本ソフトウェアは現状有姿で提供され、いかなる保証もありません。*
3.  インストール後、自動的に起動します。スタートメニューからも起動可能です。

### 設定・構成

#### InfluxDB の構成

本アプリは、以下の構成を持つ **InfluxDB v2** を想定しています。

*   **バケット名**: `environment` (デフォルト、変更可能)
*   **Measurement**: `environment` (デフォルト、変更可能)
*   **フィールド (Fields)**:
    *   `temperature` (数値: 温度)
    *   `humidity` (数値: 湿度)
    *   `co2` (数値: CO2濃度)

これらのフィールド名でデータが保存されている必要があります。

#### アプリ設定

ウィジェットを右クリックして「設定 (Settings)」を開きます。
*   **InfluxDB**: URL, トークン, 組織(Org), バケット名。
*   **Network**: 監視対象のローカルDNSとゲートウェイのIPアドレス。
*   **Display**: 表示するディスプレイ番号 (0, 1, ...) や位置調整。

### 開発環境

**必須要件:**
*   Windows 10/11
*   .NET 8.0 SDK
*   WiX Toolset v4 (インストーラー作成用)

**ビルド方法:**
```powershell
dotnet publish TaskbarMonitor/TaskbarMonitor.csproj -c Release -r win-x64 /p:PublishSingleFile=true /p:SelfContained=true
```

### ライセンス

本プロジェクトは **GNU General Public License v3.0 (GPLv3)** の下で公開されています。詳細は [LICENSE](LICENSE) ファイルをご確認ください。
