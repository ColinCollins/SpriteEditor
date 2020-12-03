# SpriteEditor
A Unity sprite editor

根据 Sebastian 早期教程写的二维的形状编辑器，做了一点微调整。
使用场景：
  1. 学习 Unity 编辑器拓展
  2. 简易的地图编辑器

后期希望加入：
1. 指定对象随机生成 功能

当前遗留问题：
1. Triangulator 切割三角形的方法研究，2D 的三角形切割如何处理的？
2. 3D 三角形切割


### 新加入功能
1. 右键选择 Custom 菜单栏，选择 create assetscripts 换出 assetScript 创建窗口
2. 填入 type 后点击 build 按钮创建对应的 scriptObject 类型
3. scriptObject 创建完成后点击 buildAsset 可以创建 .asset 文件在 ScriptObjectSetting 文件夹下
4. 选择对应的 .asset 文件右键后，选择 custom/build excel 会在 Assets 文件夹下创建 Excel 文件夹并导出对应格式的 .xlsx 文件