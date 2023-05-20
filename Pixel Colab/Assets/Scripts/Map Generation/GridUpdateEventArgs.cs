using System;
using System.Collections.Generic;

public class GridUpdateEventArgs : EventArgs
{
    public int N { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int[,] Output { get; set; }
    public Dictionary<string, float[,]> Channels { get; set; }
}