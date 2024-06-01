namespace IMRepo.Services.Utilities
{
    public class JaosDataTools
    {
        #region display
        public string? display(int? val)
        {
            if (val == null)
                return null;
            else
                return val.HasValue ? val.Value.ToString("n0") : null;
        }

        public string? display(double? val, string format = "n2")
        {
            if (val == null)
                return null;
            else
                return val.HasValue ? val.Value.ToString(format) : null;
        }
        #endregion
        #region nullable maths
        //============================================
        //
        //           nullable maths
        //--------------------------------------------



        public int? add(int? value1, int? value2)
        {
            if (value1.HasValue)
                if (value2.HasValue)
                    return value1.Value + value2.Value;
                else
                    return value1;
            else
                return value2;
        }

        public double? add(double? value1, double? value2)
        {
            if (value1.HasValue)
                if (value2.HasValue)
                    return value1.Value + value2.Value;
                else
                    return value1;
            else
                return value2;
        }

        public int? substract(int? value1, int? value2)
        {
            if (value1.HasValue)
                if (value2.HasValue)
                    return value1.Value - value2.Value;
                else
                    return value1;
            else
                return value2.HasValue ? -value2.Value : null;
        }

        public double? substract(double? value1, double? value2)
        {
            if (value1.HasValue)
                if (value2.HasValue)
                    return value1.Value - value2.Value;
                else
                    return value1;
            else
                return value2.HasValue ? -value2.Value : null;
        }

        #endregion


    }
}
