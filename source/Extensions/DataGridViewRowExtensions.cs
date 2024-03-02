namespace CastAuto
{
    public static class DataGridViewRowExtensions
    {
        public static string GetCellString(this DataGridViewRow row, int col)
        {
            return Convert.ToString(row.Cells[col].Value) ?? "";
        }
        public static string GetCellString(this DataGridViewRow row, string col)
        {
            return Convert.ToString(row.Cells[col].Value) ?? "";
        }
    }
}
