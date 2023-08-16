using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SkiaSharp.DrawVertices.demo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            SKElement.PaintSurface += SKElement_PaintSurface;
        }


        #region Drag

        private readonly Dictionary<Thumb, SKPoint> _thumbStatusDic = new();


        private void Thumbs_OnDragStarted(object sender, DragStartedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var thumbStatus = new SKPoint()
                                  {
                                      Y = Convert.ToSingle(thumb.GetValue(Canvas.TopProperty)),
                                      X = Convert.ToSingle(thumb.GetValue(Canvas.LeftProperty)),
                                  };


                _thumbStatusDic[thumb] = thumbStatus;
            }
        }

        private void Thumbs_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var thumbStatus = _thumbStatusDic[thumb];


                var newX = e.HorizontalChange + thumbStatus.X;
                var newY = e.VerticalChange   + thumbStatus.Y;

                if (newX < 0)
                {
                    newX = 0;
                }

                if (newY < 0)
                {
                    newY = 0;
                }

                thumb.SetValue(Canvas.TopProperty,  newY);
                thumb.SetValue(Canvas.LeftProperty, newX);

                _thumbStatusDic[thumb] = new SKPoint((float)newX, (float)newY);
            }
        }

        private void Thumbs_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            SKElement.InvalidateVisual();
        }

        #endregion

        private void SKElement_PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            canvas.Clear(SKColors.White);

            //=================================

            canvas.Clear(SKColors.White);


            //三个顶点
            var vertices = new[]
                           {
                               GetSkPointFromThumb(ThumbVertex0),
                               GetSkPointFromThumb(ThumbVertex1),
                               GetSkPointFromThumb(ThumbVertex2),
                           };

            //三个顶点对应的纹理坐标
            var texs = new[]
                       {
                           GetSkPointFromThumb(ThumbTex0),
                           GetSkPointFromThumb(ThumbTex1),
                           GetSkPointFromThumb(ThumbTex2),
                       };


            //纹理
            using var imgFileStream = Application.GetResourceStream(new Uri("/baboon.png", UriKind.RelativeOrAbsolute))?.Stream;
            using var stream        = new SKManagedStream(imgFileStream);
            using var bitmap        = SKBitmap.Decode(stream);

            // 创建纹理着色器 SkShader，并使用 SkShader.CreateBitmapShader 方法绑定位图
            var shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);

            var paint = new SKPaint
                        {
                            IsAntialias = true,
                            Shader      = shader,
                        };


            canvas.DrawVertices(SKVertexMode.Triangles, //如何解释顶点数组  Triangles=三角形列表
                                vertices,               //顶点
                                texs: texs,             //每个顶点在纹理空间（而不是UV空间）中的坐标。可能为null。
                                colors: null,           //每个顶点的颜色.可以为null表示不设置顶点的颜色
                                paint
                               );
        }

        private SKPoint GetSkPointFromThumb(Thumb thumb)
        {
            var x = Convert.ToSingle((double)thumb.GetValue(Canvas.LeftProperty) + thumb.ActualWidth  / 2f);
            var y = Convert.ToSingle((double)thumb.GetValue(Canvas.TopProperty)  + thumb.ActualHeight / 2f);


            return new SKPoint(x, y);
        }
    }
}