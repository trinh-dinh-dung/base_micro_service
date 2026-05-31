using OfficeOpenXml;

namespace Application.Extensions
{
    public static class EPPlusExtension
    {
        /// <summary>
        /// Set style theo template có sẵn
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="styleId">Mã styleId lấy ra được từ template</param>
        /// <returns></returns>
        public static ExcelRange SetStyleTemplate(this ExcelRange cell, int styleId)
        {
            cell.StyleID = styleId;
            return cell;
        }

        /// <summary>
        ///  Format: if number = 0 -> set value = "" 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="number"></param>
        /// <param name="isZero"></param>
        /// <returns></returns>
        public static ExcelRange SetValueNumber(this ExcelRange cell, double? number, bool isZero = true)
        {
            if (isZero == true)
            {
                if (number == null || number == 0 || number == -1)
                    cell.Value = "";
                else
                    cell.Value = number;
            }
            else
            {
                if (number == null || number == -1)
                    cell.Value = "";
                else
                    cell.Value = number;
            }

            return cell;
        }

        /// <summary>
        ///  Format: if number = 0 -> set value = "-" 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="number"></param>
        /// <param name="isZero"></param>
        /// <returns></returns>
        public static ExcelRange SetValueNumber(this ExcelRange cell, decimal? number, bool isZero = true)
        {
            if (isZero == true)
            {
                if (number == null || number == 0 || number == -1)
                    cell.Value = "-";
                else
                    cell.Value = (double)number;
            }
            else
            {
                if (number == null || number == -1)
                    cell.Value = "";
                else
                    cell.Value = (double)number;
            }

            return cell;
        }
    }
}
