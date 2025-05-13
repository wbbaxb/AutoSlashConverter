# AutoSlashConverter

Windows路径格式自动转换工具。

## 功能

- 监听剪贴板，当复制了 Windows 路径时（例如 `C:\Users\YourName`），自动将其中的反斜杠 `\` 转换为正斜杠 `/` （例如 `C:/Users/YourName`）。
- 程序启动后在系统托盘区域运行，不打扰正常使用。

## 使用场景

方便在需要使用正斜杠路径格式的程序或环境中（如 MinGW/Git Bash、WSL、各类编程配置文件等）快速粘贴路径。

## 打包成单文件

在项目根目录执行以下命令：

```bash
# 还原项目依赖
dotnet restore

# 清理旧的构建文件
dotnet clean -c Release

# 打包为单文件自包含应用
dotnet publish AutoSlashConverter.Presentation/AutoSlashConverter.Presentation.csproj -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained true -o ./publish

# 运行打包后的程序
./publish/AutoSlashConverter.exe
```

命令说明：
- `-c Release`：使用Release配置
- `-r win-x64`：目标运行时为Windows 64位
- `-p:PublishSingleFile=true`：生成单文件
- `-p:PublishReadyToRun=true`：启用ReadyToRun预编译
- `-p:IncludeNativeLibrariesForSelfExtract=true`：包含并自动提取原生库
- `--self-contained true`：包含.NET运行时
- `-o ./publish`：输出到publish目录
