﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt1
{
    enum Tool
    {
        NONE,
        CURVE,
        CIRCLE,
        LINE,
        EDITLINE,
        TRIANGLEFILLED,
        TRIANGLE,
        RECTANGLE,
        RECTANGLEFILLED,
        DOT
    }

    public partial class MainWindow : Window
    {
        private Tool tool = Tool.NONE;
        private string dcolor = "#FF000000";
        Point currentPoint = new Point();
        Point leftClickPoint = new Point();
        MyPointWithLine selectedLine;
        List<Line> lines = new List<Line>();
        List<Ellipse> ellipses = new List<Ellipse>();
        List<MyPointWithLine> points = new List<MyPointWithLine>();
        bool editMode = false;


        class NewPoint
        {
            public double locationX;
            public double locationY;
            public bool startPoint;

            public NewPoint(bool startPoint, double locationX, double locationY)
            {
                this.locationX = locationX;
                this.locationY = locationY;
                this.startPoint = startPoint;
            }
        }

        class MyPointWithLine : NewPoint
        {
            public Line line;
            public MyPointWithLine(Line line, NewPoint myPoint) : base(myPoint.startPoint, myPoint.locationX, myPoint.locationY)
            {
                this.line = line;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (editMode)
            {
                foreach (MyPointWithLine mpwl in points)
                {
                    if (Math.Abs(e.GetPosition(this).X - mpwl.locationX) < 5 && Math.Abs(e.GetPosition(this).Y - mpwl.locationY) < 5)
                    {
                        selectedLine = mpwl;
                    }
                }
            }

            if (this.tool == Tool.CURVE)
            {
                //isChecked = true;

                //if (isChecked == true)
                //{
                //    curveBTN.Background = curveBTN.Background = Brushes.LightSteelBlue;
                //}
                //else
                //{
                //    curveBTN.Background = curveBTN.Background = Brushes.Transparent;
                //}
                if (e.ButtonState == MouseButtonState.Pressed)
                    currentPoint = e.GetPosition(this);

            }

            if (this.tool == Tool.DOT)
            {
                Ellipse ellipse = new Ellipse();

                ellipse.Width = 10;
                ellipse.Height = 10;

                var bc = new BrushConverter();               
                ellipse.Fill = (Brush)bc.ConvertFrom(dcolor);

                double left = e.GetPosition(this).X;
                double top = e.GetPosition(this).Y;

                ellipse.Margin = new Thickness(left - 5, top - 5, 0, 0);

                canvasSurface.Children.Add(ellipse);
            }

            if (this.tool == Tool.CIRCLE)
            {
                Ellipse ellipse = new Ellipse();

                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.Fill = Brushes.Transparent;

                var bc = new BrushConverter();
                ellipse.Stroke = (Brush)bc.ConvertFrom(dcolor);

                double left = e.GetPosition(this).X;
                double top = e.GetPosition(this).Y;

                ellipse.Margin = new Thickness(left - 5, top - 5, 0, 0);

                canvasSurface.Children.Add(ellipse);
            }

            if (this.tool == Tool.LINE)
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    leftClickPoint = e.GetPosition(this);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.tool == Tool.LINE)
            {
                var bc = new BrushConverter();

                Line line = new Line();
                line.Stroke = (Brush)bc.ConvertFrom(dcolor);

                line.X1 = leftClickPoint.X;
                line.Y1 = leftClickPoint.Y;
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;
                lines.Add(line);
                points.AddRange(new MyPointWithLine[2] {
                new MyPointWithLine(line, new NewPoint(true, line.X1, line.Y1)),
                new MyPointWithLine(line, new NewPoint(false, line.X2, line.Y2)) });
                canvasSurface.Children.Add(line);
            }

            
            if (editMode && selectedLine != null)
            {
                lines.Remove(selectedLine.line);
                points.RemoveAll(elem => elem.line == selectedLine.line);
                canvasSurface.Children.Remove(selectedLine.line);

                if (selectedLine.startPoint)
                {
                    selectedLine.line.X1 = e.GetPosition(this).X;
                    selectedLine.line.Y1 = e.GetPosition(this).Y;
                    canvasSurface.Children.Add(selectedLine.line);
                }

                else
                {
                    selectedLine.line.X2 = e.GetPosition(this).X;
                    selectedLine.line.Y2 = e.GetPosition(this).Y;
                    canvasSurface.Children.Add(selectedLine.line);
                }

                points.AddRange(new MyPointWithLine[2] {
                new MyPointWithLine(selectedLine.line,
                new NewPoint(true, selectedLine.line.X1, selectedLine.line.Y1 )),
                new MyPointWithLine(selectedLine.line,
                new NewPoint(false, selectedLine.line.X2, selectedLine.line.Y2 ))});

                lines.Add(selectedLine.line);
                editMode = false;
                clearEditDots();
                selectedLine = null;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.tool == Tool.CURVE)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var bc = new BrushConverter();

                    Line line = new Line();
                    line.Stroke = (Brush)bc.ConvertFrom(dcolor);
                    line.X1 = currentPoint.X;
                    line.Y1 = currentPoint.Y;
                    line.X2 = e.GetPosition(this).X;
                    line.Y2 = e.GetPosition(this).Y;
                    currentPoint = e.GetPosition(this);
                    canvasSurface.Children.Add(line);
                }
            }
        }

        private void clearEditDots()
        {
            foreach (Ellipse ellipse in ellipses)
            {
                canvasSurface.Children.Remove(ellipse);
            }
        }

        private void clearCanvas()
        {
            canvasSurface.Children.Clear();
            ellipses.Clear();
            lines.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "curveBTN":
                    this.tool = Tool.CURVE;
                    editMode = false;
                    clearEditDots();
                    break;

                case "dotBTN":
                    this.tool = Tool.DOT;
                    editMode = false;
                    clearEditDots();
                    break;

                case "circleBTN":
                    this.tool = Tool.CIRCLE;
                    editMode = false;
                    clearEditDots();
                    break;

                case "lineBTN":
                    this.tool = Tool.LINE;
                    editMode = false;
                    clearEditDots();
                    break;

                case "lineEditBTN":
                    this.tool = Tool.EDITLINE; 

                    if (editMode)
                    {
                        editMode = false;
                    }

                    else
                    {
                        editMode = true;
                    }

                    if (editMode)
                    {
                        if (lines.Count > 0)
                        {
                            foreach (Line line in lines)
                            {
                                Ellipse ellipse = new Ellipse();
                                ellipse.Width = 10;
                                ellipse.Height = 10;
                                ellipse.Fill = new SolidColorBrush(Colors.Red);
                                ellipse.Margin = new Thickness(line.X1 - 5, line.Y1 - 5, 0, 0);
                                canvasSurface.Children.Add(ellipse);
                                ellipses.Add(ellipse);

                                ellipse = new Ellipse();
                                ellipse.Width = 10;
                                ellipse.Height = 10;
                                ellipse.Fill = new SolidColorBrush(Colors.Red);
                                ellipse.Margin = new Thickness(line.X2 - 5, line.Y2 - 5, 0, 0);
                                canvasSurface.Children.Add(ellipse);
                                ellipses.Add(ellipse);
                            }
                        }
                    }

                    else
                    {
                        clearEditDots();
                    }

                    break;

                //RED
                case "FFF13131":
                    this.dcolor = "#FFF13131";
                    editMode = false;
                    clearEditDots();
                    break;

                //GREEN
                case "FF41F35A":
                    this.dcolor = "#FF41F35A";
                    editMode = false;
                    clearEditDots();
                    break;

                //BLACK
                case "FF000000":
                    this.dcolor = "#FF000000";
                    editMode = false;
                    clearEditDots();
                    break;

                //BLUE
                case "FF484FE6":
                    this.dcolor = "#FF484FE6";
                    editMode = false;
                    clearEditDots();
                    break;

                //YELLOW
                case "FFF7F71F":
                    this.dcolor = "#FFF7F71F";
                    editMode = false;
                    clearEditDots();
                    break;
                //PINK
                case "FFF71FB2":
                    this.dcolor = "#FFF71FB2";
                    editMode = false;
                    clearEditDots();
                    break;

                case "clearBTN":
                    clearCanvas();
                    break;
            }
        }
    }
}