# Unity 静态资源、特效性能优化工具

## 静态资源检测

导入插件后，可以通过菜单 `Tools -> Performance Advisor -> Asset Advisor` 打开资源检测⼯具
![image](https://user-images.githubusercontent.com/5871485/149739510-8a233b3d-9d3c-492a-814b-eb91dcb8350e.png)

打开后，你可以看到以下界面信息：
![image](https://user-images.githubusercontent.com/5871485/149739649-086d0017-fa8b-4e71-ba2a-1bdc1b424635.png)

设置选项包含：
- Asset Type：当前需要检测的资源类型，目前支持（Texutre、Mesh、Material、Shader、Animation、Prefab），选择 `All` 则会检测所有规则集。
- Asset Path：资源检测路径，你可以直接在 Unity Editor 中的 Assets 里面的文件夹直接拖动到改选项框。
- Asset Profile：资源检测设置选项，不同的文件夹资源可能需要不同的检测参数，默认检测参数设置放置在 `Assets Advisor/Settings` 里面，你可以通过菜单`Create->Performance Advisor/Profile Asset` 创建新的检测设置选项。
- Asset Filter：资源检测过滤器，输入指定关键字，如 `Bg` 则会匹配所有资源路径中带有此关键字的资源进行检测。
- Preview：开启预览所有检测过的资源。

设置检测的文件夹后，点击 `Start` 启动资源检测：
