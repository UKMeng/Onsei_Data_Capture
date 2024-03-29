# Onsei_Data_Capture

## 软件功能

将[DLsite](https://www.dlsite.com/home/works/type/=/work_type_category/audio)购入的音声按作品进行信息刮削，得到适用于[Jellyfin](https://github.com/jellyfin/jellyfin)的NFO文件，从而更好地分类、体验的半自动化软件

PS: 本软件只是作者的初学C#的练习作品，同时也只写了一个粗略的使用说明，如果在使用中遇到了问题，欢迎提出issue进行交流

## 效果

![example](https://github.com/UKMeng/Onsei_Data_Capture/blob/main/img/example.png)
![example2](https://github.com/UKMeng/Onsei_Data_Capture/blob/main/img/example2.png)

## 使用说明

### 使用前准备

1. 准备一个文件夹用于存放要处理的文件
2. 以一个文件夹为一个作品的单位，文件夹名为该作品的RJ编号
3. 作品文件夹中存放音频文件以及图片文件（名为```cover```的封面图片以及名为```fanart```的背景图片，jpg、png格式均可，可以没有图片，为了美观和识别度最好至少有cover图片）

文件结构

```
example
|
├── RJ363741
|   ├──cover.jpg
|   ├──fanart.jpg
|   ├──1.mp3
|   ├──2.mp3
|   └──3.mp3
|
├── RJ317278
|   ├──cover.png
|   ├──fanart.jpg
|   ├──1.mp3
|   ├──2.flac
|   └──3.wav
```

### 使用软件

1. 修改```Config.ini```, ```sourceFolder```后的值修改为之前创建好的存放所有待处理文件的文件夹得绝对路径；如果网络访问DLsite有困难，可尝试使用代理，在```proxy```一栏中修改相应的设置
2. 打开```Onsei_Data_Capture.exe```，按任意键继续，等待全部文件处理完之后，再按任意键退出
3. 处理好的文件将会在```sourceFolder```的```output```文件夹中

### 使用Jellyfin

1. 将处理好的文件放入对应的Jellyfin媒体库的文件夹中，刷新元数据