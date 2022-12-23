using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LanMonitor
{
    public partial class MainWindow
    {
        public class FC_Control
        {
            private const int sheetGrid = 32;
            private const double speed = 1;
            private const double limit = 200;

            private static bool IsInitialized;
            private static Image image;
            private static BitmapSource source;
            private static bool isRight = true;
            private static List<Point> IdleList;
            private static List<Point> SleepList;
            private static List<Point> SleepInList;
            private static List<Point> SleepOutList;
            private static List<Point> MoveList;
            private static List<Point> MoveInList;
            private static List<Point> MoveOutList;
            private static List<Point> PlayList;
            private static ActionState state = ActionState.None;
            private static int index;
            private static double offset = 10;
            private static bool mousein;
            private static bool click;

            public enum ActionState
            {
                None,
                Idle,
                Sleep,
                SleepIn,
                SleepOut,
                Move,
                MoveIn,
                MoveOut,
                Play
            }
            public static void Initialize(Canvas canvas)
            {
                if (IsInitialized)
                {
                    return;
                }
                image = new()
                {
                    Width = 16,
                    Height = 16,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };
                image.MouseEnter += (object sender, MouseEventArgs e) =>
                {
                    mousein = true;
                };
                image.MouseLeave += (object sender, MouseEventArgs e) =>
                {
                    mousein = false;
                };
                image.MouseDown += (object sender, MouseButtonEventArgs e) =>
                {
                    click = true;
                };
                canvas.Children.Add(image);
                Canvas.SetTop(image, 4);
                Canvas.SetLeft(image, 6);

                source = new BitmapImage(new Uri("pack://application:,,,/fc.spr")) as BitmapSource;

                IdleList = ParsePath("3,0 3,1 3,2 3,3 3,2 3,1");
                SleepList = ParsePath("2,0 2,0 2,0 2,0 2,0 2,0");
                SleepInList = ParsePath("0,0 0,1 0,2 0,3 1,0 1,1 1,2 1,3");
                SleepOutList = ParsePath("1,3 1,2 1,1 1,0 0,3 0,2 0,1 0,0");
                MoveList = ParsePath("13,0 13,1 13,2 13,3 13,0 13,1 13,2 13,3");
                MoveInList = ParsePath("5,1 5,0 4,3 4,2 4,1 4,0");
                MoveOutList = ParsePath("4,0 4,1 4,2 4,3 5,0 5,1");
                PlayList = ParsePath("16,0 16,1 16,2 16,3 17,0 17,1 17,2 17,3 18,0 18,1 18,2 18,3 19,0 19,1 19,2 19,3 20,0 20,1 20,2 20,3");

                new Task(() =>
                {
                    while (true)
                    {
                        int v = -1;
                        Point p;
                        switch (state)
                        {
                            case ActionState.Idle:
                                p = IdleList[index];
                                v = IdleList.Count - index;
                                break;
                            case ActionState.Sleep:
                                p = SleepList[index];
                                v = SleepList.Count - index;
                                break;
                            case ActionState.SleepIn:
                                p = SleepInList[index];
                                v = SleepInList.Count - index;
                                break;
                            case ActionState.SleepOut:
                                p = SleepOutList[index];
                                v = SleepOutList.Count - index;
                                break;
                            case ActionState.Move:
                                p = MoveList[index];
                                v = MoveList.Count - index;
                                offset += isRight ? speed : speed * -1;
                                if (offset <= 0)
                                {
                                    isRight = true;
                                }
                                else if (offset >= limit)
                                {
                                    isRight = false;
                                }
                                break;
                            case ActionState.MoveIn:
                                p = MoveInList[index];
                                v = MoveInList.Count - index;
                                break;
                            case ActionState.MoveOut:
                                p = MoveOutList[index];
                                v = MoveOutList.Count - index;
                                break;
                            case ActionState.Play:
                                p = PlayList[index];
                                v = PlayList.Count - index;
                                break;
                            default:
                                break;
                        }

                        if (v != -1)
                        {
                            DrawVisual((int)p.Y, (int)p.X);
                            index++;
                            if (v == 1)
                            {
                                ChangeState();
                                index = 0;
                            }
                        }

                        Thread.Sleep(160);
                    }
                }).Start();

                IsInitialized = true;
            }

            private static void Image_MouseEnter(object sender, MouseEventArgs e)
            {
                throw new NotImplementedException();
            }

            private static List<Point> ParsePath(string path)
            {
                return path.Split(' ').Select(point => Point.Parse(point)).ToList();
            }

            private static void ChangeState()
            {
                Random random = new();
                int next = random.Next(100);
                switch (state)
                {
                    case ActionState.Idle:
                        if (next < 10)
                        {
                            state = ActionState.Idle;
                        }
                        else if (next < 50)
                        {
                            state = ActionState.MoveIn;
                        }
                        else if (next < 70)
                        {
                            state = ActionState.SleepIn;
                        }
                        else
                        {
                            state = mousein ? ActionState.Play : ActionState.Idle;
                        }
                        break;
                    case ActionState.Sleep:
                        if (next < 10)
                        {
                            state = ActionState.SleepOut;
                        }
                        else if (next < 80)
                        {
                            state = ActionState.Sleep;
                        }
                        else
                        {
                            state = click ? ActionState.SleepOut : ActionState.Sleep;
                        }
                        break;
                    case ActionState.SleepIn:
                        state = ActionState.Sleep;
                        break;
                    case ActionState.SleepOut:
                        state = ActionState.Idle;
                        break;
                    case ActionState.Move:
                        if (next < 20)
                        {
                            state = ActionState.MoveOut;
                        }
                        else if (next < 70)
                        {
                            state = ActionState.Move;
                        }
                        else
                        {
                            state = mousein ? ActionState.MoveOut : ActionState.Move;
                        }
                        break;
                    case ActionState.MoveIn:
                        state = ActionState.Move;
                        break;
                    case ActionState.MoveOut:
                        state = ActionState.Idle;
                        break;
                    case ActionState.Play:
                        state = ActionState.Idle;
                        break;
                    default:
                        state = ActionState.Idle;
                        break;
                }
                if (state == ActionState.Move && next % 10 > 7)
                {
                    isRight = !isRight;
                }
                click = false;
            }

            private static void DrawVisual(int x, int y)
            {
                Application.Current.Dispatcher?.Invoke(() =>
                {
                    CroppedBitmap bitmap = new(source, new Int32Rect(x * sheetGrid, y * sheetGrid, 32, 32));
                    bitmap.Freeze();
                    image.Source = bitmap;
                    Transform translate = new TranslateTransform(isRight ? offset * -1 : offset, 0);
                    Transform scale = new ScaleTransform(isRight ? -1 : 1, 1);
                    TransformGroup myTransformGroup = new();
                    myTransformGroup.Children.Add(translate);
                    myTransformGroup.Children.Add(scale);
                    myTransformGroup.Freeze();
                    image.RenderTransform = myTransformGroup;
                });
            }
            public static void Start()
            {
                state = ActionState.Idle;
            }
        }
    }
}
