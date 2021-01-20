using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    class ChessboardBox
    {
        int dim;  //dimension of the Box
        public int X;  //starting X of the box
        public int Y;  //starting Y of the box
        int col;  //column of the box
        int row;  //row of the box

        public ChessboardBox(int i, int j)
        {
            col = i % 8;
            if(col == 0)
                col = 8;  //the last column has a result of 0, but it is the 8th
            row = i % 8 != 0 ? i / 8 + 1 : i / 8;  // 5/8=0, but it belongs to the first row. Each row is augmented by 1
            dim = j;

            X = (col - 1) * dim;
            Y = (row - 1) * dim;
        }

        public ChessboardBox()
        {
        }
    }
}
