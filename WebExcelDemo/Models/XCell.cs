namespace Models
{
    public struct Loc
    {
        public Loc() { }

        public Loc(int row, int col) {
            Row = row;
            Column = col;
        }

        public int Row { get; set; }
        public int Column { get; set; }
    }
    public struct XCell
    {
        public XCell() { }

        public XCell(string name, int row, int col) { 
            this.Name = name;
            this.Location = new Loc(row, col);
        }

        public string Name { get; set; }
        public Loc Location { get; set; }
    }
}
