using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.System;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Input;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Sudokuuw.Generator;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Sudokuuw.Views
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePad : Page
    {
        public GamePad()
        {
            this.InitializeComponent();
            
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown; ;
        }

        

        private SudoGenerator sudoGenerator = new SudoGenerator();
        private Sudoku sudoku;

        private CanvasRenderTarget renderTarget;
        private Point pressedPoint = new Point(0,0);
        private int xSelectedI = -1;
        private int ySelectedI = -1;
        private float cellLength = 0f;
        private List<float> xList = new List<float>();
        private List<float> yList = new List<float>();

        private void BackgroundCanvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            renderTarget = new CanvasRenderTarget(sender, (float)sender.ActualWidth, (float)sender.ActualHeight);
        }

     

        private void BackgroundCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {            
            var session = args.DrawingSession;
            xList.Clear();
            yList.Clear();
            float ratioPading = Convert.ToSingle(0.1);
            float ratio = Convert.ToSingle(0.8);
            float length = Convert.ToSingle(System.Math.Min(sender.RenderSize.Height, sender.RenderSize.Width)) ;
            float length3 = Convert.ToSingle(System.Math.Min(sender.RenderSize.Height, sender.RenderSize.Width) * ratio / 3 );

            float length9 = Convert.ToSingle(System.Math.Min(sender.RenderSize.Height, sender.RenderSize.Width) * ratio / 9);
            cellLength = length9;

            float xStart = Convert.ToSingle((sender.RenderSize.Width - length) / 2 + ratioPading * length);
            float yStart = Convert.ToSingle((sender.RenderSize.Height - length) / 2 + ratioPading * length);
            float xEnd = xStart + length * ratio;
            float xMid01 = xStart + length9;
            float xMid02 = xStart + length9*2;

            float xMid1 = xStart + length3;
            float xMid11 = xMid1 + length9;
            float xMid12 = xMid1 + length9 * 2;

            float xMid2 = xStart + length3 * 2;
            float xMid21 = xMid2 + length9;
            float xMid22 = xMid2 + length9 * 2;

            float yMid01 = yStart + length9;
            float yMid02 = yStart + length9 * 2;

            float yMid1 = yStart + length3;
            float yMid11 = yMid1 + length9;
            float yMid12 = yMid1 + length9 * 2;


            float yMid2 = yStart + length3 * 2;
            float yMid21 = yMid2 + length9;
            float yMid22 = yMid2 + length9 * 2;

            float yEnd = yStart + length * ratio;

            xList.Add(xStart);
            yList.Add(yStart);
            xList.Add(xMid01);
            yList.Add(yMid01);
            xList.Add(xMid02);
            yList.Add(yMid02);

            xList.Add(xMid1);
            yList.Add(yMid1);
            xList.Add(xMid11);
            yList.Add(yMid11);
            xList.Add(xMid12);
            yList.Add(yMid12);

            xList.Add(xMid2);
            yList.Add(yMid2);
            xList.Add(xMid21);
            yList.Add(yMid21);
            xList.Add(xMid22);
            yList.Add(yMid22);


            xList.Add(xEnd);
            yList.Add(yEnd);

          

            Color lineBoderColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);
            float widthHighlightBoder = Convert.ToSingle(4);
            float widthThinBoder = Convert.ToSingle(1);
            float selectedBoder = Convert.ToSingle(2);

            //session.FillRectangle(new Rect(new Point(), sender.RenderSize), this.backgroundBrush);
            session.DrawLine(new System.Numerics.Vector2(xStart,yStart) , new System.Numerics.Vector2(xEnd, yStart), lineBoderColor, widthHighlightBoder);
            session.DrawLine(new System.Numerics.Vector2(xStart, yStart), new System.Numerics.Vector2(xStart, yEnd), lineBoderColor, widthHighlightBoder);
            session.DrawLine(new System.Numerics.Vector2(xEnd, yEnd), new System.Numerics.Vector2(xEnd, yStart), lineBoderColor, widthHighlightBoder);
            session.DrawLine(new System.Numerics.Vector2(xEnd, yEnd), new System.Numerics.Vector2(xStart, yEnd), lineBoderColor, widthHighlightBoder);

            session.DrawLine(new System.Numerics.Vector2(xMid01, yStart), new System.Numerics.Vector2(xMid01, yEnd), lineBoderColor, widthThinBoder);
            session.DrawLine(new System.Numerics.Vector2(xMid02, yStart), new System.Numerics.Vector2(xMid02, yEnd), lineBoderColor, widthThinBoder);

            session.DrawLine(new System.Numerics.Vector2(xMid11, yStart), new System.Numerics.Vector2(xMid11, yEnd), lineBoderColor, widthThinBoder);
            session.DrawLine(new System.Numerics.Vector2(xMid12, yStart), new System.Numerics.Vector2(xMid12, yEnd), lineBoderColor, widthThinBoder);

            session.DrawLine(new System.Numerics.Vector2(xMid21, yStart), new System.Numerics.Vector2(xMid21, yEnd), lineBoderColor, widthThinBoder);
            session.DrawLine(new System.Numerics.Vector2(xMid22, yStart), new System.Numerics.Vector2(xMid22, yEnd), lineBoderColor, widthThinBoder);

            session.DrawLine(new System.Numerics.Vector2(xMid1, yStart), new System.Numerics.Vector2(xMid1, yEnd), lineBoderColor, widthHighlightBoder);
            session.DrawLine(new System.Numerics.Vector2(xMid2, yStart), new System.Numerics.Vector2(xMid2, yEnd), lineBoderColor, widthHighlightBoder);

            session.DrawLine(new System.Numerics.Vector2(xStart, yMid1), new System.Numerics.Vector2(xEnd, yMid1), lineBoderColor, widthHighlightBoder);
            session.DrawLine(new System.Numerics.Vector2(xStart, yMid2), new System.Numerics.Vector2(xEnd, yMid2), lineBoderColor, widthHighlightBoder);

            session.DrawLine(new System.Numerics.Vector2(xStart, yMid01), new System.Numerics.Vector2(xEnd, yMid01), lineBoderColor, widthThinBoder);
            session.DrawLine(new System.Numerics.Vector2(xStart, yMid02), new System.Numerics.Vector2(xEnd, yMid02), lineBoderColor, widthThinBoder);

            session.DrawLine(new System.Numerics.Vector2(xStart, yMid11), new System.Numerics.Vector2(xEnd, yMid11), lineBoderColor, widthThinBoder);
            session.DrawLine(new System.Numerics.Vector2(xStart, yMid12), new System.Numerics.Vector2(xEnd, yMid12), lineBoderColor, widthThinBoder);

            session.DrawLine(new System.Numerics.Vector2(xStart, yMid21), new System.Numerics.Vector2(xEnd, yMid21), lineBoderColor, widthThinBoder);
            session.DrawLine(new System.Numerics.Vector2(xStart, yMid22), new System.Numerics.Vector2(xEnd, yMid22), lineBoderColor, widthThinBoder);
            if (xSelectedI == -1)
            {
                if ((pressedPoint.X > xStart) && (pressedPoint.Y > yStart) && (pressedPoint.X < xEnd) && (pressedPoint.Y < yEnd))
                {
                    float xSelected = Convert.ToSingle(pressedPoint.X - xStart);
                    float ySelected = Convert.ToSingle(pressedPoint.Y - yStart);

                    xSelectedI = Convert.ToInt32(Math.Floor(xSelected / length9));
                    ySelectedI = Convert.ToInt32(Math.Floor(ySelected / length9));
                }
            }

            if (xSelectedI != -1)
            {
                float selectedStartX = xList[xSelectedI];
                float selectedEndX = xList[xSelectedI + 1];

                float selectedStartY = xList[ySelectedI];
                float selectedEndY = xList[ySelectedI + 1];

                if (xSelectedI % 3 == 0)
                {
                    selectedStartX = xList[xSelectedI] + 3;
                    selectedEndX = xList[xSelectedI + 1] - 2;
                }
                else if (xSelectedI % 3 == 1)
                {
                    selectedStartX = xList[xSelectedI] + 2;
                    selectedEndX = xList[xSelectedI + 1] - 2;
                }
                else
                {
                    selectedStartX = xList[xSelectedI] + 2;
                    selectedEndX = xList[xSelectedI + 1] - 3;
                }



                if (ySelectedI % 3 == 0)
                {
                    selectedStartY = yList[ySelectedI] + 3;
                    selectedEndY = yList[ySelectedI + 1] - 2;
                }
                else if (ySelectedI % 3 == 1)
                {
                    selectedStartY = yList[ySelectedI] + 2;
                    selectedEndY = yList[ySelectedI + 1] - 2;
                }
                else
                {
                    selectedStartY = yList[ySelectedI] + 2;
                    selectedEndY = yList[ySelectedI + 1] - 3;
                }


                //session.DrawCircle(new System.Numerics.Vector2((float)pressedPoint.X, (float)pressedPoint.Y), 20, Colors.Black, 4);
                session.DrawLine(new System.Numerics.Vector2(selectedStartX, selectedStartY), new System.Numerics.Vector2(selectedEndX, selectedStartY), Colors.IndianRed, selectedBoder);
                session.DrawLine(new System.Numerics.Vector2(selectedStartX, selectedStartY), new System.Numerics.Vector2(selectedStartX, selectedEndY), Colors.IndianRed, selectedBoder);

                session.DrawLine(new System.Numerics.Vector2(selectedEndX, selectedEndY), new System.Numerics.Vector2(selectedEndX, selectedStartY), Colors.IndianRed, selectedBoder);
                session.DrawLine(new System.Numerics.Vector2(selectedEndX, selectedEndY), new System.Numerics.Vector2(selectedStartX, selectedEndY), Colors.IndianRed, selectedBoder);

                
            }

            if (sudoku != null)
            {
                int selectedValue = 0;
                if (xSelectedI != -1)
                {
                    selectedValue = sudoku.GetValue(xSelectedI, ySelectedI);
                }
                    

                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        SudoCell cell = sudoku.GetSudoCell(x, y);
                        if (cell.value != 0)
                        {                          
                            if (cell.isReadOnly)
                            {
                                if (selectedValue == cell.value)
                                {
                                    DrawGlowText(session, cell.value.ToString(), x, y, Colors.White, Colors.DarkBlue);
                                }
                                else
                                {
                                    DrawText(session, cell.value.ToString(), x, y);
                                }
                                
                            }
                            else
                            {
                                if (selectedValue == cell.value)
                                {
                                    DrawGlowText(session, cell.value.ToString(), x, y,Colors.White,  Colors.BlueViolet );
                                }
                                else
                                {
                                    DrawText(session, cell.value.ToString(), x, y, Colors.BlueViolet);
                                }
                                   
                            }
                        }
                        else
                        {
                            int index = 1;                            
                            foreach (var testValue in cell.testValues.Reverse())
                            {
                                DrawTestText(session, testValue, x, y,index);
                                index++;
                            }                           
                        }


                    }
                }
            }

        }

        private void BackgroundCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(this.BackgroundCanvas);
            pressedPoint = pt.Position;
            xSelectedI = -1;
            ySelectedI = -1;
            BackgroundCanvas.Invalidate();
        }

        private CanvasTextFormat CreateTextFormat()
        {
            return new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                FontSize = cellLength * 4 / 5 / 1.3f,
                FontFamily = "Arial Rounded MT"
            };
        }

        private CanvasTextLayout CreateTextLayout(ICanvasResourceCreator resourceCreator, Size size, String text)
        {
            var format = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = cellLength * 4 / 5 / 1.3f,
                FontFamily = "Arial Rounded MT"
            };

            return new CanvasTextLayout(
                resourceCreator,
                text,
                format,
                (float)size.Width,
                (float)size.Height);
        }

        private void DrawGlowText(CanvasDrawingSession session, string text, int lineX, int lineY, Color? textColor = null, Color? glowColor = null)
        {
            Size size = new Size(cellLength , cellLength );
            var offset = cellLength / 10 ;
            size.Width = size.Width - offset * 2;
            size.Height = size.Height - offset * 2;

            float xStart = xList[lineX];
            float yStart = yList[lineY];

            Rect rect = new Rect()
            {
                X = xStart,
                Y = yStart,
                Width = cellLength ,
                Height = cellLength
            };

            Color textFontColor = textColor ?? Colors.DarkBlue;
            Color textGlowColor = glowColor ?? Colors.LightSeaGreen;

            using (var textLayout = CreateTextLayout(session, size, text))
            using (var textCommandList = new CanvasCommandList(session))
            {
                using (var textDs = textCommandList.CreateDrawingSession())
                {
                    textDs.DrawTextLayout(textLayout, 0, 0, textGlowColor);
                }
                var glowEffectGraph = new GlowEffectGraph();
                glowEffectGraph.Setup(textCommandList, offset);
                session.DrawImage(glowEffectGraph.Output, Convert.ToSingle(rect.X)+ offset, Convert.ToSingle(rect.Y)+ offset);

                session.DrawTextLayout(textLayout, Convert.ToSingle(rect.X) + offset, Convert.ToSingle(rect.Y) + offset, textFontColor);
            }
        }

        private void DrawText(CanvasDrawingSession session, String text, int lineX, int lineY, Color? color = null)
        {
            var leftJustifiedTextFormat = CreateTextFormat();

            Size textSize = MeasureTextSize(text, leftJustifiedTextFormat, cellLength);

            float space = (cellLength - Convert.ToSingle(textSize.Width)) / 2 ;
            float xStart = xList[lineX];
            float yStart = yList[lineY];
            Rect rect = new Rect()
            {
                X = xStart + space,
                Y = yStart + cellLength / 10f,
                Width = cellLength - 2f * cellLength / 10f,
                Height = cellLength - 2f * cellLength / 10f
            };
            Color textFontColor = color ?? Colors.DarkBlue;
            session.DrawText(text, rect, textFontColor, leftJustifiedTextFormat);
        }


        private void DrawTestText(CanvasDrawingSession session, int testValue, int lineX, int lineY, int order )
        {
            CanvasTextFormat leftJustifiedTextFormat = new CanvasTextFormat()
            {
                VerticalAlignment = CanvasVerticalAlignment.Top,
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                FontSize = cellLength / 3 / 1.1f,
                FontFamily = "Arial"
            };

            Size textSize = MeasureTextSize("8", leftJustifiedTextFormat, cellLength);
            
            float xStart = xList[lineX];
            float yStart = yList[lineY];
            int orderLength = order % 4;
            if (orderLength == 0) { orderLength = 4; }
            float orderHeight = Convert.ToSingle( Math.Ceiling(order / 4.0));
            Rect rect = new Rect()
            {
                X = xStart + cellLength - (textSize.Width * 3 / 2  ) * orderLength,
                Y = yStart + cellLength - cellLength /10f - (textSize.Height * 6 / 5 ) * orderHeight,
                Width = textSize.Width,
                Height = textSize.Height
            };

            Color textFontColor = Colors.SkyBlue;
            session.DrawText(testValue.ToString(), rect, textFontColor, leftJustifiedTextFormat);
        }

        private Size MeasureTextSize(string text, CanvasTextFormat textFormat, float limitedToWidth = 0.0f, float limitedToHeight = 0.0f)
        {
            var device = CanvasDevice.GetSharedDevice();

            var layout = new CanvasTextLayout(device, text, textFormat, limitedToWidth, limitedToHeight);

            var width = layout.DrawBounds.Width;
            var height = layout.DrawBounds.Height;

            return new Size(width, height);
        }

     
        private void UpdateSudokuValue(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Number1:
                    sudoku.SetValue(xSelectedI, ySelectedI, 1);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number2:
                    sudoku.SetValue(xSelectedI, ySelectedI, 2);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number3:
                    sudoku.SetValue(xSelectedI, ySelectedI, 3);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number4:
                    sudoku.SetValue(xSelectedI, ySelectedI, 4);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number5:
                    sudoku.SetValue(xSelectedI, ySelectedI, 5);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number6:
                    sudoku.SetValue(xSelectedI, ySelectedI, 6);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number7:
                    sudoku.SetValue(xSelectedI, ySelectedI, 7);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number8:
                    sudoku.SetValue(xSelectedI, ySelectedI, 8);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number9:
                    sudoku.SetValue(xSelectedI, ySelectedI, 9);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number0:
                    sudoku.SetValue(xSelectedI, ySelectedI, 0);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Delete:
                    sudoku.SetValue(xSelectedI, ySelectedI, 0);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.N:
                    sudoku = sudoGenerator.Generate(3, Level.VeryHard);
                    BackgroundCanvas.Invalidate();
                    break;
                default:
                    break;
            }
        }

        private void UpdateSudokuTestValue(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Number1:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 1);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number2:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 2);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number3:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 3);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number4:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 4);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number5:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 5);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number6:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 6);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number7:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 7);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number8:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 8);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.Number9:
                    sudoku.SetTestValue(xSelectedI, ySelectedI, 9);
                    BackgroundCanvas.Invalidate();
                    break;
                case VirtualKey.N:
                    sudoku = sudoGenerator.Generate(3, Level.VeryHard);
                    BackgroundCanvas.Invalidate();
                    break;
                default:
                    break;
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if ((xSelectedI != -1) && (ySelectedI != -1))
            {
                if (sudoku != null)
                {
                    if (sudoku.GetValue(xSelectedI,ySelectedI) == 0)
                    {
                        //UpdateSudokuTestValue(e.VirtualKey);
                        UpdateSudokuValue(e.VirtualKey);
                    } 
                }
                else
                {
                    UpdateSudokuValue(VirtualKey.N);
                }
                    
            }
        }

    }

    class GlowEffectGraph
    {
        public ICanvasImage Output
        {
            get
            {
                return blur;
            }
        }

        MorphologyEffect morphology = new MorphologyEffect()
        {
            Mode = MorphologyEffectMode.Dilate,
            Width = 1,
            Height = 1
        };

        GaussianBlurEffect blur = new GaussianBlurEffect()
        {
            BlurAmount = 0,
            BorderMode = EffectBorderMode.Soft
        };

        public GlowEffectGraph()
        {
            blur.Source = morphology;
        }

        public void Setup(ICanvasImage source, float amount)
        {
            morphology.Source = source;

            var halfAmount = Math.Min(amount / 2, 100);
            morphology.Width = (int)Math.Ceiling(halfAmount);
            morphology.Height = (int)Math.Ceiling(halfAmount);
            blur.BlurAmount = halfAmount;
        }
    }


}
