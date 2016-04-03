﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Chess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        int[,] chessboardmap = new int[8, 8]; //an integer array of chesspieces
        bool ispathhighlighted = false; //shows whether the valid path of a chess piece is highlighted or not
        int[] selectpiececoord = new int[2]; //coordinates of selected piece
        string playerturn = "white"; 
        Rectangle[,] chesssquares = new Rectangle[8, 8]; //a 2 dimensional array of all chessboard squares
        public MainPage()
        {
            this.InitializeComponent();
            initializechessboard();
        }

        public void initializechessboard() //sets up the chessboard
        {
            for(int i=0;i<8;i++)
            {
                for(int j=0;j<8;j++)
                {
                    chessboardmap[i, j] = 0;
                }
            }

            for(int i=0;i<8;i++)
            {
                chessboardmap[1, i] = 6;
                chessboardmap[6, i] = -6;
            }

            //white pieces
            chessboardmap[0, 0] = 4; //rook
            chessboardmap[0, 7] = 4; 
            chessboardmap[0, 1] = 3; //knight
            chessboardmap[0, 6] = 3;
            chessboardmap[0, 2] = 5; //bishop
            chessboardmap[0, 5] = 5;
            chessboardmap[0, 3] = 2; //queen 
            chessboardmap[0, 4] = 1; //king

            //black pieces
            chessboardmap[7, 0] = -4; //rook
            chessboardmap[7, 7] = -4;
            chessboardmap[7, 1] = -3; //knight
            chessboardmap[7, 6] = -3;
            chessboardmap[7, 2] = -5; //bishop
            chessboardmap[7, 5] = -5;
            chessboardmap[7, 3] = -2; //queen
            chessboardmap[7, 4] = -1; //king

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    loadpiece(chessboardmap[i, j], i, j);
                }
            }
        }

        public void loadpiece(int piece, int x, int y) //loads a chess piece
        {
            Rectangle temp = new Rectangle();
            temp.Height = 100;
            temp.Width = 100;
            temp.Tapped += Rectangle_Tapped;
            String owner;
            if (piece == 0)
                owner = "none";
            else if (piece < 0)
                owner = "black";
            else
                owner = "white";


            if (owner == "white")
            {
                //white pieces
                temp.Stroke = new SolidColorBrush(Colors.LawnGreen);
                temp.StrokeThickness = 0;
                ImageBrush img = new ImageBrush();
                switch (piece)
                {
                    case 1:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/king_white.png"));
                        break;
                    case 2:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/queen_white.png"));
                        break;
                    case 3:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/knight_white.png"));
                        break;
                    case 4:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/rook_white.png"));
                        break;
                    case 5:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/bishop_white.png"));
                        break;
                    case 6:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/pawn_white.png"));
                        break;
                }
                temp.Fill = img;
            }

            else if (owner == "black")
            {
                //black pieces
                temp.Stroke = new SolidColorBrush(Colors.Red);
                temp.StrokeThickness = 0;
                ImageBrush img = new ImageBrush();
                switch (piece)
                {
                    case -1:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/king_black.png"));
                        break;
                    case -2:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/queen_black.png"));
                        break;
                    case -3:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/knight_black.png"));
                        break;
                    case -4:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/rook_black.png"));
                        break;
                    case -5:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/bishop_black.png"));
                        break;
                    case -6:
                        img.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/pawn_black.png"));
                        break;
                }
                temp.Fill = img;

            }

            else if(owner == "none")
            {
                temp.Stroke = new SolidColorBrush(Colors.Blue);
                temp.StrokeThickness = 0;
                temp.Fill = new SolidColorBrush(Colors.Transparent);
            }

            Grid.SetRow(temp, x);
            Grid.SetColumn(temp, y);
            if(chesssquares[x,y]!=null)
                chessboard.Children.Remove(chesssquares[x, y]);
            chesssquares[x, y] = temp;
            chessboard.Children.Add(chesssquares[x, y]);
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e) //triggers whenever a chess square is tapped
        {
            Rectangle rect = sender as Rectangle;
            int row = Grid.GetRow(rect);
            int column = Grid.GetColumn(rect);
            coordinatebox.Text = row.ToString() + " , " + column.ToString();
            if(ispathhighlighted == false)
            {
                if (playerturn == "white" && chessboardmap[row, column] > 0)
                {
                    showvalidsquares(row, column);
                    ispathhighlighted = true;
                    selectpiececoord[0] = row;
                    selectpiececoord[1] = column;
                }
                else if (playerturn == "black" && chessboardmap[row, column] < 0)
                {
                    showvalidsquares(row, column);
                    ispathhighlighted = true;
                    selectpiececoord[0] = row;
                    selectpiececoord[1] = column;
                }
            }
            else
            {
                if (chesssquares[row,column].StrokeThickness != 0 && (row != selectpiececoord[0] || column != selectpiececoord[1]))
                {
                    clearpath();
                    movepiece(chessboardmap[selectpiececoord[0], selectpiececoord[1]], selectpiececoord[0], selectpiececoord[1], row, column);
                    if (playerturn == "white")
                    {
                        playerturn = "black";
                    }

                    else
                    {
                        playerturn = "white";
                    }
                    ispathhighlighted = false;
                }
                else
                {
                    clearpath();
                    ispathhighlighted = false;
                }
            }
        }

        private void clearpath() //clears the highlighted path
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    chesssquares[i, j].StrokeThickness = 0;
                }
            }
        }

        private void showvalidsquares(int selectx, int selecty) //shows valid moves of a selected chess piece
        {
            switch(chessboardmap[selectx,selecty])
            {
                case 1: //white king
                    for(int i = selectx - 1; i <= selectx + 1; i++)
                    {
                        for(int j = selecty - 1; j <= selecty + 1; j++)
                        {
                            if(i>-1 && i<8 && j<8 && j>-1)
                            {
                                if(chessboardmap[i,j]<=0)
                                {
                                    highlighttile(i, j);
                                }
                            }
                        }
                    }
                    break;

                case 2: //white queen
                    /*horizontal and vertical movements*/
                    for (int i = selectx + 1; i < 8; i++)
                    {
                        if (chessboardmap[i, selecty] == 0)
                        {
                            highlighttile(i, selecty);
                        }
                        else if (chessboardmap[i, selecty] < 0)
                        {
                            highlighttile(i, selecty);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selectx - 1; i > -1; i--)
                    {
                        if (chessboardmap[i, selecty] == 0)
                        {
                            highlighttile(i, selecty);
                        }
                        else if (chessboardmap[i, selecty] < 0)
                        {
                            highlighttile(i, selecty);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selecty + 1; i < 8; i++)
                    {
                        if (chessboardmap[selectx, i] == 0)
                        {
                            highlighttile(selectx, i);
                        }
                        else if (chessboardmap[selectx, i] < 0)
                        {
                            highlighttile(selectx, i);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selecty - 1; i > -1; i--)
                    {
                        if (chessboardmap[selectx, i] == 0)
                        {
                            highlighttile(selectx, i);
                        }
                        else if (chessboardmap[selectx, i] < 0)
                        {
                            highlighttile(selectx, i);
                            break;
                        }
                        else
                            break;
                    }

                    /*Diagonal movements*/
                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx + i < 8 && selecty + i < 8)
                        {
                            if (chessboardmap[selectx + i, selecty + i] == 0)
                                highlighttile(selectx + i, selecty + i);
                            else if (chessboardmap[selectx + i, selecty + i] < 0)
                            {
                                highlighttile(selectx + i, selecty + i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx + i < 8 && selecty - i > -1)
                        {
                            if (chessboardmap[selectx + i, selecty - i] == 0)
                                highlighttile(selectx + i, selecty - i);
                            else if (chessboardmap[selectx + i, selecty - i] < 0)
                            {
                                highlighttile(selectx + i, selecty - i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx - i > -1 && selecty + i < 8)
                        {
                            if (chessboardmap[selectx - i, selecty + i] == 0)
                                highlighttile(selectx - i, selecty + i);
                            else if (chessboardmap[selectx - i, selecty + i] < 0)
                            {
                                highlighttile(selectx - i, selecty + i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx - i > -1 && selecty - i > -1)
                        {
                            if (chessboardmap[selectx - i, selecty - i] == 0)
                                highlighttile(selectx - i, selecty - i);
                            else if (chessboardmap[selectx - i, selecty - i] < 0)
                            {
                                highlighttile(selectx - i, selecty - i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    break;

                case 3: //white knight
                    if (selectx + 2 < 8 && selecty - 1 > -1)
                    {
                        if (chessboardmap[selectx + 2, selecty - 1] <= 0)
                        {
                            highlighttile(selectx + 2, selecty - 1);
                        }
                    }

                    if (selectx + 2 < 8 && selecty + 1 < 8)
                    {

                        if (chessboardmap[selectx + 2, selecty + 1] <= 0)
                        {
                            highlighttile(selectx + 2, selecty + 1);
                        }
                    }

                    if (selectx - 2 > -1 && selecty - 1 > -1)
                    {

                        if (chessboardmap[selectx - 2, selecty - 1] <= 0)
                        {
                            highlighttile(selectx - 2, selecty - 1);
                        }
                    }

                    if (selectx - 2 > -1 && selecty + 1 < 8)
                    {
                        if (chessboardmap[selectx - 2, selecty + 1] <= 0)
                        {
                            highlighttile(selectx - 2, selecty + 1);
                        }
                    }

                    /**/
                    if (selectx + 1 < 8 && selecty + 2 < 8)
                    {

                        if (chessboardmap[selectx + 1, selecty + 2] <= 0)
                        {
                            highlighttile(selectx + 1, selecty + 2);
                        }
                    }

                    if (selectx - 1 > -1 && selecty + 2 < 8)
                    {

                        if (chessboardmap[selectx - 1, selecty + 2] <= 0)
                        {
                            highlighttile(selectx - 1, selecty + 2);
                        }
                    }

                    if (selectx + 1 < 8 && selecty - 2 > -1)
                    {
                        if (chessboardmap[selectx + 1, selecty - 2] <= 0)
                        {
                            highlighttile(selectx + 1, selecty - 2);
                        }
                    }

                    if (selectx - 1 > -1 && selecty - 2 > -1)
                    { 
                        if (chessboardmap[selectx - 1, selecty - 2] <= 0)
                        {
                            highlighttile(selectx - 1, selecty - 2);
                        }
                    }
                    break;

                case 4: //white rook
                    for(int i = selectx + 1;i<8;i++)
                    {
                        if (chessboardmap[i, selecty] == 0)
                        {
                            highlighttile(i, selecty);
                        }
                        else if (chessboardmap[i, selecty] < 0)
                        {
                            highlighttile(i, selecty);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selectx - 1; i > -1; i--)
                    {
                        if (chessboardmap[i, selecty] == 0)
                        {
                            highlighttile(i, selecty);
                        }
                        else if (chessboardmap[i, selecty] < 0)
                        {
                            highlighttile(i, selecty);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selecty + 1; i < 8; i++)
                    {
                        if (chessboardmap[selectx, i] == 0)
                        {
                            highlighttile(selectx, i);
                        }
                        else if (chessboardmap[selectx,i] < 0)
                        {
                            highlighttile(selectx, i);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selecty - 1; i > -1; i--)
                    {
                        if (chessboardmap[selectx, i] == 0)
                        {
                            highlighttile(selectx, i);
                        }
                        else if (chessboardmap[selectx, i] < 0)
                        {
                            highlighttile(selectx, i);
                            break;
                        }
                        else
                            break;
                    }
                    break;

                case 5: //white bishop
                    for(int i = 1; i<8 ; i++)
                    {
                        if (selectx + i < 8 && selecty + i < 8)
                        {
                            if (chessboardmap[selectx + i, selecty + i] == 0)
                                highlighttile(selectx + i, selecty + i);
                            else if (chessboardmap[selectx + i, selecty + i] < 0)
                            {
                                highlighttile(selectx + i, selecty + i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx + i < 8 && selecty - i > -1)
                        {
                            if (chessboardmap[selectx + i, selecty - i] == 0)
                                highlighttile(selectx + i, selecty - i);
                            else if (chessboardmap[selectx + i, selecty - i] < 0)
                            {
                                highlighttile(selectx + i, selecty - i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx - i > -1 && selecty + i < 8)
                        {
                            if (chessboardmap[selectx - i, selecty + i] == 0)
                                highlighttile(selectx - i, selecty + i);
                            else if (chessboardmap[selectx - i, selecty + i] < 0)
                            {
                                highlighttile(selectx - i, selecty + i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx - i > -1 && selecty - i > -1)
                        {
                            if (chessboardmap[selectx - i, selecty - i] == 0)
                                highlighttile(selectx - i, selecty - i);
                            else if (chessboardmap[selectx - i, selecty - i] < 0)
                            {
                                highlighttile(selectx - i, selecty - i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }
                    break;

                case 6: //white pawn
                    if (selectx + 1 < 8)
                    {
                        if (chessboardmap[selectx + 1, selecty] == 0)
                        {
                            highlighttile(selectx + 1, selecty);
                            if (selectx == 1 && chessboardmap[selectx + 2, selecty] == 0)
                                highlighttile(selectx + 2, selecty);
                        }
                    }
                    if (selectx + 1 < 8 && selecty + 1 < 8)
                    {
                        if (chessboardmap[selectx + 1, selecty + 1] < 0)
                            highlighttile(selectx + 1, selecty + 1);
                    }
                    if(selectx + 1 < 8 && selecty - 1 > -1)
                    {
                        if (chessboardmap[selectx + 1, selecty - 1] < 0)
                            highlighttile(selectx + 1, selecty - 1);
                    }
                break;
                
                /* black pieces*/
                case -1: //black king
                    for (int i = selectx - 1; i <= selectx + 1; i++)
                    {
                        for (int j = selecty - 1; j <= selecty + 1; j++)
                        {
                            if (i > -1 && i < 8 && j < 8 && j > -1)
                            {
                                if (chessboardmap[i, j] >= 0)
                                {
                                    highlighttile(i, j);
                                }
                            }
                        }
                    }
                    break;
                case -2: //black queen
                    /*Horizontal and Vertical movements*/
                    for (int i = selectx + 1; i < 8; i++)
                    {
                        if (chessboardmap[i, selecty] == 0)
                        {
                            highlighttile(i, selecty);
                        }
                        else if (chessboardmap[i, selecty] > 0)
                        {
                            highlighttile(i, selecty);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selectx - 1; i > -1; i--)
                    {
                        if (chessboardmap[i, selecty] == 0)
                        {
                            highlighttile(i, selecty);
                        }
                        else if (chessboardmap[i, selecty] > 0)
                        {
                            highlighttile(i, selecty);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selecty + 1; i < 8; i++)
                    {
                        if (chessboardmap[selectx, i] == 0)
                        {
                            highlighttile(selectx, i);
                        }
                        else if (chessboardmap[selectx, i] > 0)
                        {
                            highlighttile(selectx, i);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selecty - 1; i > -1; i--)
                    {
                        if (chessboardmap[selectx, i] == 0)
                        {
                            highlighttile(selectx, i);
                        }
                        else if (chessboardmap[selectx, i] > 0)
                        {
                            highlighttile(selectx, i);
                            break;
                        }
                        else
                            break;
                    }

                    /*Diagonal movements*/

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx + i < 8 && selecty + i < 8)
                        {
                            if (chessboardmap[selectx + i, selecty + i] == 0)
                                highlighttile(selectx + i, selecty + i);
                            else if (chessboardmap[selectx + i, selecty + i] > 0)
                            {
                                highlighttile(selectx + i, selecty + i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx + i < 8 && selecty - i > -1)
                        {
                            if (chessboardmap[selectx + i, selecty - i] == 0)
                                highlighttile(selectx + i, selecty - i);
                            else if (chessboardmap[selectx + i, selecty - i] > 0)
                            {
                                highlighttile(selectx + i, selecty - i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx - i > -1 && selecty + i < 8)
                        {
                            if (chessboardmap[selectx - i, selecty + i] == 0)
                                highlighttile(selectx - i, selecty + i);
                            else if (chessboardmap[selectx - i, selecty + i] > 0)
                            {
                                highlighttile(selectx - i, selecty + i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx - i > -1 && selecty - i > -1)
                        {
                            if (chessboardmap[selectx - i, selecty - i] == 0)
                                highlighttile(selectx - i, selecty - i);
                            else if (chessboardmap[selectx - i, selecty - i] > 0)
                            {
                                highlighttile(selectx - i, selecty - i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    break;
                case -3: //black knight
                    if (selectx + 2 < 8 && selecty - 1 > -1)
                    {
                        if (chessboardmap[selectx + 2, selecty - 1] >= 0)
                        {
                            highlighttile(selectx + 2, selecty - 1);
                        }
                    }

                    if (selectx + 2 < 8 && selecty + 1 < 8)
                    {

                        if (chessboardmap[selectx + 2, selecty + 1] >= 0)
                        {
                            highlighttile(selectx + 2, selecty + 1);
                        }
                    }

                    if (selectx - 2 > -1 && selecty - 1 > -1)
                    {

                        if (chessboardmap[selectx - 2, selecty - 1] >= 0)
                        {
                            highlighttile(selectx - 2, selecty - 1);
                        }
                    }

                    if (selectx - 2 > -1 && selecty + 1 < 8)
                    {
                        if (chessboardmap[selectx - 2, selecty + 1] >= 0)
                        {
                            highlighttile(selectx - 2, selecty + 1);
                        }
                    }

                    /**/
                    if (selectx + 1 < 8 && selecty + 2 < 8)
                    {

                        if (chessboardmap[selectx + 1, selecty + 2] >= 0)
                        {
                            highlighttile(selectx + 1, selecty + 2);
                        }
                    }

                    if (selectx - 1 > -1 && selecty + 2 < 8)
                    {

                        if (chessboardmap[selectx - 1, selecty + 2] >= 0)
                        {
                            highlighttile(selectx - 1, selecty + 2);
                        }
                    }

                    if (selectx + 1 < 8 && selecty - 2 > -1)
                    {
                        if (chessboardmap[selectx + 1, selecty - 2] >= 0)
                        {
                            highlighttile(selectx + 1, selecty - 2);
                        }
                    }

                    if (selectx - 1 > -1 && selecty - 2 > -1)
                    {
                        if (chessboardmap[selectx - 1, selecty - 2] >= 0)
                        {
                            highlighttile(selectx - 1, selecty - 2);
                        }
                    }
                    break;
                case -4: //black rook
                    for (int i = selectx + 1; i < 8; i++)
                    {
                        if (chessboardmap[i, selecty] == 0)
                        {
                            highlighttile(i, selecty);
                        }
                        else if (chessboardmap[i, selecty] > 0)
                        {
                            highlighttile(i, selecty);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selectx - 1; i > -1; i--)
                    {
                        if (chessboardmap[i, selecty] == 0)
                        {
                            highlighttile(i, selecty);
                        }
                        else if (chessboardmap[i, selecty] > 0)
                        {
                            highlighttile(i, selecty);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selecty + 1; i < 8; i++)
                    {
                        if (chessboardmap[selectx, i] == 0)
                        {
                            highlighttile(selectx, i);
                        }
                        else if (chessboardmap[selectx, i] > 0)
                        {
                            highlighttile(selectx, i);
                            break;
                        }
                        else
                            break;
                    }

                    for (int i = selecty - 1; i > -1; i--)
                    {
                        if (chessboardmap[selectx, i] == 0)
                        {
                            highlighttile(selectx, i);
                        }
                        else if (chessboardmap[selectx, i] > 0)
                        {
                            highlighttile(selectx, i);
                            break;
                        }
                        else
                            break;
                    }
                    break;
                case -5: //black bishop
                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx + i < 8 && selecty + i < 8)
                        {
                            if (chessboardmap[selectx + i, selecty + i] == 0)
                                highlighttile(selectx + i, selecty + i);
                            else if (chessboardmap[selectx + i, selecty + i] > 0)
                            {
                                highlighttile(selectx + i, selecty + i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx + i < 8 && selecty - i > -1)
                        {
                            if (chessboardmap[selectx + i, selecty - i] == 0)
                                highlighttile(selectx + i, selecty - i);
                            else if (chessboardmap[selectx + i, selecty - i] > 0)
                            {
                                highlighttile(selectx + i, selecty - i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx - i > -1 && selecty + i < 8)
                        {
                            if (chessboardmap[selectx - i, selecty + i] == 0)
                                highlighttile(selectx - i, selecty + i);
                            else if (chessboardmap[selectx - i, selecty + i] > 0)
                            {
                                highlighttile(selectx - i, selecty + i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    for (int i = 1; i < 8; i++)
                    {
                        if (selectx - i > -1 && selecty - i > -1)
                        {
                            if (chessboardmap[selectx - i, selecty - i] == 0)
                                highlighttile(selectx - i, selecty - i);
                            else if (chessboardmap[selectx - i, selecty - i] > 0)
                            {
                                highlighttile(selectx - i, selecty - i);
                                break;
                            }
                            else
                                break;
                        }

                        else
                            break;
                    }

                    break;
                case -6: //black pawn
                    if (selectx - 1 > -1)
                    {
                        if (chessboardmap[selectx - 1, selecty] == 0)
                        {
                            highlighttile(selectx - 1, selecty);
                            if (selectx == 6 && chessboardmap[selectx - 2, selecty] == 0)
                                highlighttile(selectx - 2, selecty);
                        }
                    }
                    if (selectx - 1 > -1 && selecty + 1 < 8)
                    {
                        if (chessboardmap[selectx - 1, selecty + 1] > 0)
                            highlighttile(selectx - 1, selecty + 1);
                    }
                    if (selectx - 1 > -1 && selecty - 1 > -1)
                    {
                        if (chessboardmap[selectx - 1, selecty - 1] > 0)
                            highlighttile(selectx - 1, selecty - 1);
                    }
                    break;
                default:
                    break;
            }
            highlighttile(selectx, selecty);
        }

        private void highlighttile(int x, int y) //highlights a tile by drawing a boundary around it
        {
            if (chesssquares[x, y].StrokeThickness == 0)
                chesssquares[x, y].StrokeThickness = 4;
            else
                chesssquares[x, y].StrokeThickness = 0;
        }

        private void movepiece(int piece, int initx, int inity, int targetx, int targety) //moves a chesspiece
        {
            if((initx != targetx || inity !=targety) && piece!=0)
            {
                int temp;
                temp = chessboardmap[targetx, targety];
                chessboardmap[targetx, targety] = chessboardmap[initx, inity];
                chessboardmap[initx, inity] = 0;
                loadpiece(chessboardmap[initx, inity], initx, inity);
                loadpiece(chessboardmap[targetx, targety], targetx, targety);
                if (temp == 1 || temp == -1)
                    gameover(playerturn);
            }
        }
       
        private void gameover(string _playerturn) //triggers when game is over.
        {
            TextBlock winmsg = new TextBlock();
            winmsg.Height = 100;
            winmsg.Width = 500;
            winmsg.Opacity = 0;
            winmsg.HorizontalAlignment = HorizontalAlignment.Center;
            winmsg.VerticalAlignment = VerticalAlignment.Center;
            winmsg.FontSize = 66.667;
            winmsg.FontWeight = FontWeights.Bold;
            winmsg.Text = _playerturn.ToUpper() + " Wins!";
            winmsg.Foreground = new SolidColorBrush(Colors.Black);
            winmsg.TextAlignment = TextAlignment.Center;

            Button tryagain = new Button();
            tryagain.Height = 70;
            tryagain.Width = 200;
            tryagain.Opacity = 0;
            tryagain.HorizontalAlignment = HorizontalAlignment.Center;
            tryagain.FontSize = 26.667;
            tryagain.Content = "Play again!";
            tryagain.Tapped += Button_Tapped;
            tryagain.Background = new SolidColorBrush(Colors.Black);
            tryagain.Margin = new Thickness(0,150,0,0);

            Maingrid.Children.Add(winmsg);
            Maingrid.Children.Add(tryagain);

            Storyboard stb = new Storyboard();
            DoubleAnimation fade = new DoubleAnimation();
            DoubleAnimation appear = new DoubleAnimation();
            DoubleAnimation appear1 = new DoubleAnimation();
            fade.To = 0.2;
            fade.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
            Storyboard.SetTarget(fade, chessboard);
            Storyboard.SetTargetProperty(fade, "Opacity");

            appear.To = 1.0;
            appear.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
            Storyboard.SetTarget(appear, winmsg);
            Storyboard.SetTargetProperty(appear, "Opacity");

            appear1.To = 1.0;
            appear1.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
            Storyboard.SetTarget(appear1, tryagain);
            Storyboard.SetTargetProperty(appear1, "Opacity");

            stb.Children.Add(fade);
            stb.Children.Add(appear);
            stb.Children.Add(appear1);
            stb.Begin();
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

    }
}
