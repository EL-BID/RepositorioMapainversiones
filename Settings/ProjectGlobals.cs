namespace IMRepo
{
    public class ProjectGlobals
    {
        //==============================================
        //                  Standard
        //
        //----------------------------------------------

        public static string defaultPassword = "Nuevo@123";

        public static int maxRowsBeforeSearch = 0;

        static public DateTime MinValidDate = new DateTime(1964, 01, 01);
        static public DateTime MaxValidDate = new DateTime(2064, 01, 01);
        static public string MinValidDateString = "1964/01/01";
        static public string MaxValidDateString = "2064/01/01";

        //==============================================
        //              Project Specifics
        //
        //----------------------------------------------
        public const string ProjectTitle = "Repositorio MapaInversiones";

        public static string defaultLatitude = "13.1939"; //"13.1939"; // Rafaela (Arg) -31.2516  // BuenosAires -34.61
        public static string defaultLongitude = "-59.5432"; //"-59.5432"; // Rafaela (Arg) -61.4917 // BuenosAires -58.38

        public static int MaxCellStringLength = 32767;

        public const string RoleAdmin = "Administrator";
        public const string RoleDireccion = "Direccion";
        public const string RoleOperacion = "Operacion";
        public const string RoleConsulta = "Consulta";
        public const string registeredRoles = "Administrator,Direccion,Operacion,Consulta";


        static public class TaskStage
        {
            public const int Presentada = 1;
            public const int Aprobada = 2;
        }

        static public class PaymentStage
        {
            public const int Presentado = 1;
            public const int Aprobado = 2;
            public const int Pagado = 3;
        }
        static public class ProjectStage
        {
            public const int Iniciar = 1;
            public const int Ejecucion = 2;
            public const int Finalizada = 3;
            public const int Rescindida = 4;
        }


    }
}
