<h1 align="center">
    <img src="https://raw.githubusercontent.com/JimBobSquarePants/ImageProcessor/develop/build/icons/imageprocessor-logo-256.png" alt="ImageProcessor" width="175"/>
    <br>
    ImageProcessor.Core
</h1>
    

**ImageProcessor.Core** 基于[ImageProcessor](https://github.com/JimBobSquarePants/ImageProcessor) 采用 .Net Standard 2.0 构建，是一款轻量级的图像处理框架,使用Fluent Api方式构建更加易于使用。想阅读详细文档可访问 [http://imageprocessor.org/](http://imageprocessor.org/)


新增马赛克效果
```
ImageFactory factory = new ImageFactory();
factory.Load(@"C:\Users\chenr\Documents\Assets\test2.gif");
factory.Mosaic(new SS.Drawing.Imaging.MosaicLayer(450, 280, 140, 50, new System.Drawing.Size(10, 10)));
factory.Save(@"C:\Users\chenr\Documents\\Assets\test2_mosaic.gif");
```
