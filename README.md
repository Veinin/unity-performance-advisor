<h1 align="center">Unity Performance Advisor</h1>

一个简单的 Unity 静态资源检测、特效性能优化工具。

目录

- [安装](#安装)
  - [通过 Git URL 安装](#通过-Git-URL-安装)
  - [通过 unitypackage 安装](#通过-unitypackage-安装)
- [使用](#使用)
  - [静态资源检测](#静态资源检测)
  - [特效资源检测](#特效资源检测)


## 安装

### 通过 Git URL 安装

使用文本编辑器打开 *Packages/manifest.json* 文件，将下面一行地址添加到 `dependencies`中。

    {
        "dependencies": {
            "com.veinin.upa": "https://github.com/Veinin/unity-performance-advisor.git"
        }
    }

或者可以使用 Package Manager 中的 `Add package from git URL` 选项，输入 URL 后进行安装。

![image](https://user-images.githubusercontent.com/5871485/149872106-b417e627-3b51-48a9-b350-f3f662a7831d.png)

### 通过 unitypackage 安装

直接下载最新的 [unitypackage](https://github.com/Veinin/unity-performance-advisor/releases) 安装包，导入到项目编辑器内即可。

如果遇到报错，请使用 Package Manager 安装依赖：[Editor Coroutines](https://docs.unity3d.com/Packages/com.unity.editorcoroutines@1.0/manual/index.html)

## 使用

### 静态资源检测

导入插件后，可以通过菜单 `Tools -> Performance Advisor -> Assets Advisor` 打开资源检测⼯具。
![image](https://user-images.githubusercontent.com/5871485/150292774-6844dafa-e7f2-40f2-a027-df4876405f91.png)

打开后，你可以看到以下界面信息：
![image](https://user-images.githubusercontent.com/5871485/149739649-086d0017-fa8b-4e71-ba2a-1bdc1b424635.png)

设置选项包含：
- Asset Type：当前需要检测的资源类型，目前支持（Texutre、Mesh、Material、Shader、Animation、Prefab），选择 `All` 则会检测所有规则集。
- Asset Path：资源检测路径，你可以直接在 Unity Editor 中的 Assets 里面的文件夹直接拖动到改选项框。
- Asset Profile：资源检测设置选项，不同的文件夹资源可能需要不同的检测参数，默认检测参数设置放置在 `Assets Advisor/Settings` 里面，你可以通过菜单`Create -> Performance Advisor -> Profile Asset` 创建新的检测设置选项。
- Asset Filter：资源检测过滤器，输入指定关键字，如 `Bg` 则会匹配所有资源路径中带有此关键字的资源进行检测。
- Preview：开启预览所有检测过的资源。

设置检测的文件夹后，点击 `Start` 启动资源检测：
![Assets Advisor](https://user-images.githubusercontent.com/5871485/149765207-be224d88-8ad4-4d0e-8d26-524ba687b073.gif)

检测完成后，下面会输出相关检测选项，检测结果不满足需求的资源会使用红色字显示出来。可以点击操作相关按钮执行操作：
- `>` 按钮，展示出该检测规则对应的资源列表。
- `Fixed` 按钮，如果该检测规则可以直接批处理优化，则可以按此按钮一键批处理优化。
- 点击资源列表中的资源，会在 `Project` 窗口定位到此资源目录。
![image](https://user-images.githubusercontent.com/5871485/149765583-92c6cd17-c0e0-4705-a4ed-482994de2585.png)

### 特效资源检测

可以通过菜单 `Tools -> Performance Advisor -> Effects Advisor` 打开特效资源检测工具。
![image](https://user-images.githubusercontent.com/5871485/150292774-6844dafa-e7f2-40f2-a027-df4876405f91.png)

打开后，你可以看到以下界面信息：
![image](https://user-images.githubusercontent.com/5871485/149766226-3d561911-15ec-45b9-9efc-a6999ce585ff.png)

设置选项包含：
- Asset Path：特效资源检测路径。
- Asset Profile：资源检测设置选项，同上。

设置检测的文件夹后，点击 `Start` 开始特效资源检测，此时会启动游戏场景，并开始挨个执行特效播放操作，最后得到特效运行时数据：
![Effects Advisor](https://user-images.githubusercontent.com/5871485/149766779-9b6b04d6-e909-412e-93c6-4e579cc7c852.gif)
